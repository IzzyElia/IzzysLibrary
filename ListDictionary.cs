using System.Collections;
using System.Collections.Generic;
using Izzy.UnitTesting;

namespace Izzy.DataStructures.HashsetDictionary
{
	/// <summary>
	/// A generic class for managing a dictionary with any number of ORDERED values per key
	/// </summary>
	[System.Serializable]
	public class ListDictionary<TKey, TValueType>
	{
		Dictionary<TKey, List<TValueType>> dictionary;
		public ListDictionary() 
		{
			dictionary = new Dictionary<TKey, List<TValueType>>();
		}
		public TValueType[] Values
		{
			get
			{
				List<TValueType> values = new List<TValueType>();
				foreach(List<TValueType> list in dictionary.Values)
				{
					foreach(TValueType value in list)
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
				dictionary.Add(key, new List<TValueType>());
			}
		}
		public void Add(TKey key, TValueType value)
		{
			if (dictionary.TryGetValue(key, out List<TValueType> values))
			{
				values.Add(value);
			}
			else
			{
				List<TValueType> newValueSet = new List<TValueType>();
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
			if (dictionary.TryGetValue(key, out List<TValueType> values))
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
			foreach(KeyValuePair<TKey, List<TValueType>> kvp in dictionary)
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
			foreach(List<TValueType> list in dictionary.Values)
			{
				list.Clear();
			}
		}
		public TValueType[] Get_CertainOfKey (TKey key)
		{
			List<TValueType> values = dictionary[key];
			TValueType[] outValues = new TValueType[values.Count];
			values.CopyTo(outValues);
			return outValues;
		}
		public TValueType[] Get (TKey key)
		{
			if (dictionary.TryGetValue(key, out List<TValueType> values))
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
            if (dictionary.TryGetValue(key, out List<TValueType> values))
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
			if (dictionary.TryGetValue(key, out List<TValueType> values))
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
			string str = $"ListDictionary ({typeof(TKey).Name}, {typeof(TValueType).Name}):\n";
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