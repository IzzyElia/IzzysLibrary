using System.Collections;
using System.Collections.Generic;

namespace Izzy
{
	/// <summary>
	/// A generic class for managing a dictionary with any number of values per key
	/// </summary>
	[System.Serializable]
	public class HashsetDictionary<TKey, TValueType>
	{
		Dictionary<TKey, HashSet<TValueType>> dictionary;
		public HashsetDictionary() 
		{
			dictionary = new Dictionary<TKey, HashSet<TValueType>>();
		}
		public TValueType[] Values
		{
			get
			{
				List<TValueType> values = new List<TValueType>();
				foreach(HashSet<TValueType> hashSet in dictionary.Values)
				{
					foreach(TValueType value in hashSet)
					{
						values.Add(value);
					}
				}
				return values.ToArray();
			}
		}
		public void EnsureKey(TKey key)
		{
			if(!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new HashSet<TValueType>());
			}
		}
		public void Add(TKey key, TValueType value)
		{
			if (dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				values.Add(value);
			}
			else
			{
				HashSet<TValueType> newValueSet = new HashSet<TValueType>();
				newValueSet.Add(value);
				dictionary.Add(key, newValueSet);
			}
		}
		/// <summary>
		/// Adds <paramref name="value"/> to <paramref name="key"/> without checking to make sure the key exists
		/// This is faster than <see cref="Add(TKey, TValueType)"/>. Make sure they key has previously been instantiated with <see cref="EnsureKey(TKey)"/>!
		/// </summary>
		public void Add_CertainOfKey(TKey key, TValueType value)
		{
			try
			{
				dictionary[key].Add(value);
			}
			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException($"The key '{key}' is not present in the dictionary");
			}
		}
		public void Remove(TKey key, TValueType value)
		{
			if (dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				//if (values.Contains(value))
					values.Remove(value);
			}
		}
		public void Remove_CertainOfKey (TKey key, TValueType value)
		{
			try
			{
				dictionary[key].Remove(value);
			}
			catch (KeyNotFoundException)
			{
				throw new KeyNotFoundException($"Either the key '{key}' or the value '{value}' is not present in the dictionary");
			}
			
		}
		/// <summary>
		/// Removes all instances of the value 'value' in the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void RemoveAllOfValue(TValueType value)
		{
			foreach(KeyValuePair<TKey, HashSet<TValueType>> kvp in dictionary)
			{
				kvp.Value.Remove(value);
			}
		}
		public void RemoveAllFromKey(TKey key)
		{
			if (dictionary.ContainsKey(key)) { dictionary[key].Clear(); }
		}
		public void DestroyKey (TKey key)
        {
			dictionary.Remove(key);
        }
		public void Clear ()
		{
			dictionary.Clear();
		}
		public void Clear_KeepKeys ()
		{
			foreach(HashSet<TValueType> hashset in dictionary.Values)
			{
				hashset.Clear();
			}
		}
		public TValueType[] Get_CertainOfKey (TKey key)
		{
			HashSet<TValueType> values = dictionary[key];
			TValueType[] outValues = new TValueType[values.Count];
			values.CopyTo(outValues);
			return outValues;
		}
		public TValueType[] Get (TKey key)
		{
			if (dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				TValueType[] outValues = new TValueType[values.Count];
				values.CopyTo(outValues);
				return outValues;
			}
			else
			{
				return new TValueType[0];
			}
		}
		public int Count(TKey key)
		{
            if (dictionary.TryGetValue(key, out HashSet<TValueType> values))
            {
                return values.Count;
            }
            else
            {
                return 0;
            }
        }
        public int Count_CertainOfKey(TKey key)
        {
			return dictionary[key].Count;
        }
        public bool Contains (TKey key, TValueType value)
		{
			if (dictionary.TryGetValue(key, out HashSet<TValueType> values))
			{
				return values.Contains(value);
			}
			return false;
		}
		public bool Contains_CertainOfKey (TKey key, TValueType value)
		{
			return dictionary[key].Contains(value);
		}
		public override string ToString()
		{
			string str = $"Hashset Dictionary ({typeof(TKey).Name}, {typeof(TValueType).Name}):\n";
			foreach(TKey key in dictionary.Keys)
			{
				str += $"\t{key.ToString()}:\n";
				foreach (TValueType value in dictionary[key])
				{
					str += $"\t\t{value.ToString()}\n";
				}
			}
			return str;
		}
	}
}


# if DEBUG
namespace Izzy.UnitTests
{
	using Izzy.UnitTesting;
    public class HashsetDictionaryTests
    {
		[Test]
        static TestResult TestManipulation()
        {
            HashsetDictionary<string, string> testHashsetDictionary = new HashsetDictionary<string, string>();
            testHashsetDictionary.Add("key1", "value_a");
            testHashsetDictionary.Add("key1", "value_b");
            testHashsetDictionary.Add("key2", "value_a");
            testHashsetDictionary.Add("key2", "value_c");
            if (testHashsetDictionary.Get("key1").Length != 2)
            { return new TestResult(false, "Failstate #1 - Issue adding items"); } // Failstate #1

            testHashsetDictionary.Remove("key1", "value_b");
            if (testHashsetDictionary.Get("key1").Length != 1 ||
                testHashsetDictionary.Get("key2").Length != 2)
            { return new TestResult(false, "Failstate #2 - Issue removing existing item"); } // Failstate #2

            testHashsetDictionary.Remove("key1", "value_c");
            if (testHashsetDictionary.Get("key1").Length != 1 ||
                testHashsetDictionary.Get("key2").Length != 2) { return new TestResult(false, "Failstate #3 - Issue removing nonexisting item"); }

            testHashsetDictionary.RemoveAllOfValue("value_a");
            if (
                testHashsetDictionary.Get("key1").Length != 0 ||
                testHashsetDictionary.Get("key2").Length != 1)
            { return new TestResult(false, "Failstate #4 - Issue removing all instances of value"); } // Failstate #4
            testHashsetDictionary.Add("key2", "value_d");
            testHashsetDictionary.RemoveAllFromKey("key2");
            if (testHashsetDictionary.Get("key2").Length != 0) { return new TestResult(false, "Failstate #5 - Issue clearing key"); } // Failstate #5

            return new TestResult(true, "No issues detected"); // Success
        }
    }
}
#endif