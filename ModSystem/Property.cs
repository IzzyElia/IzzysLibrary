using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Izzy;


namespace Izzy.ModSystem
{
    /// <summary>
    /// A property can store and retrive strings, floats, or arrays of those value types
    /// </summary>
    [Serializable()]
    public class Property
    {
        const string UncachedValueExpectionMessage = "Property has not calculated this value type. If it was originally set through a string, make sure to call CalculateOtherValueFormsFromStringValue to make them accessable";
        public string name { get; private set; }
        public string type { get; private set; }
        string[] _values;
        FloatColor?[] _colors;
        int?[] _ints;
        float?[] _floats;
        public void CalculateOtherValueFormsFromStringValue ()
        {
            _ints = new int?[_values.Length];
            _floats = new float?[_values.Length];
            _colors = new FloatColor?[_values.Length];
            for (int i = 0; i < _values.Length; i++)
            {
                if (int.TryParse(_values[i], out int _int)) _ints[i] = _int;
                else _ints[i] = null;
                if (float.TryParse(_values[i], out float _float)) _floats[i] = _float;
                else _floats[i] = null;
                if (FloatColor.TryParseHex(_values[i], out FloatColor color)) _colors[i] = color;
                else _colors[i] = null;
            }
        }
        public string ValueAsString
        {
            get
            {
                if (_values?.Length > 0) { return _values[0]; }
                else { return ""; }
            }
        }
        public float ValueAsFloat
        {
            get
            {
                try
                {
                    return (float)_floats[0];
                }
                catch (System.InvalidOperationException)
                {

                    throw new System.InvalidOperationException(UncachedValueExpectionMessage);
                }
            }
        }
        public int ValueAsInt
        {
            get
            {
                try
                {
                    return (int)_ints[0];
                }
                catch (System.InvalidOperationException)
                {

                    throw new System.InvalidOperationException(UncachedValueExpectionMessage);
                }
            }
        }
        public FloatColor ValueAsColor
        {
            get
            {
                try
                {
                    return (FloatColor)_colors[0];
                }
                catch (System.InvalidOperationException)
                {

                    throw new System.InvalidOperationException(UncachedValueExpectionMessage);
                }
            }
        }
        public float ValueAsFloatOrFallback(float fallback)
		{
            if (_floats[0] != null) return (float)_floats[0];
            else { return fallback; }
        }
        public int ValueAsIntOrFallback(int fallback)
        {
            if (_ints[0] != null) { return (int)_ints[0]; }
            else { return fallback; }
        }
        public FloatColor ValueAsColorOrFallback (FloatColor fallback)
        {
            if (_colors[0] != null) return (FloatColor)_colors[0];
            else return fallback;
        }
        public string[] ValueAsStringArray
        {
            get
            {
                string[] outStrings = new string[_values.Length];
                for (int i = 0; i < _colors.Length; i++)
                {
                    outStrings[i] = _values[i]; // WIP make this a try-catch once I know the error type (is it an invalid cast, or invalid operation, or something else)
                }
                return outStrings;
            }
        }
        public float[] ValueAsFloatArray
        {
            get
            {
                float[] outFloats = new float[_floats.Length];
                for (int i = 0; i < _floats.Length; i++)
                {
                    try
                    {
                        outFloats[i] = (float)_floats[i];
                    }
                    catch (InvalidOperationException)
                    {

                        throw new System.InvalidOperationException(UncachedValueExpectionMessage);
                    }
                }
                return outFloats;
            }
        }
        public int[] ValueAsIntArray
        {
            get
            {
                int[] outInts = new int[_ints.Length];
                for (int i = 0; i < _ints.Length; i++)
                {
                    try
                    {
                        outInts[i] = (int)_ints[i];
                    }
                    catch (InvalidOperationException)
                    {

                        throw new System.InvalidOperationException(UncachedValueExpectionMessage);
                    }
                }
                return outInts;
            }
        }
        public FloatColor[] ValueAsColorArray
        {
            get
            {
                FloatColor[] outColors = new FloatColor[_colors.Length];
                for (int i = 0; i < _colors.Length; i++)
                {
                    try
                    {
                        outColors[i] = (FloatColor)_colors[i];
                    }
                    catch (InvalidOperationException)
                    {

                        throw new System.InvalidOperationException(UncachedValueExpectionMessage);
                    }
                    
                }
                return outColors;
            }
        }
        public Property(string name, string type) : this(name, type, new string[0], false)
        {
        }
        public Property(string name, string type, string value, bool calculateOtherDataTypes = false) : this(name, type, new string[1] { value }, calculateOtherDataTypes)
        {
        }
        public Property(string name, string type, string[] values, bool calculateOtherDataTypes = false)
        {
            this.name = name;
            this.type = type;
            this._values = values;
            if (calculateOtherDataTypes)
            {
                CalculateOtherValueFormsFromStringValue();
            }
            else
            {
                this._floats = new float?[_values.Length];
                this._ints = new int?[_values.Length];
                this._colors = new FloatColor?[_values.Length];
            }
        }
        public Property (string name, string type, FloatColor color)
        {
            this.name = name;
            this.type = null;
            this._colors = new FloatColor?[1] { color };
            this._values = new string[_colors.Length];
            this._floats = new float?[_colors.Length];
            this._ints = new int?[_colors.Length];
        }
        public Property (Property property)
        {
            this.name = property.name;
            this.type = property.type;
            this._values = property._values;
            this._floats = property._floats;
            this._colors = property._colors;
            this._ints = property._ints;
        }
        public void SetValue(float value)
        {
            SetValue(value.ToString());
        }
        public void SetValue(int value)
        {
            SetValue(value.ToString());
        }
        public void SetValue(string value)
        {
            this._values = new string[1] { value };
        }
        public void SetValue(string[] values)
        {
            this._values = values;
        }
        /// <summary>
        /// Append a value to the end of the property's values
        /// </summary>
        /// <param name="value"></param>
        public void AppendValue(string value)
        {
            string[] n = new string[this._values.Length + 1];
            this._values.CopyTo(n, 0);
            n[n.Length - 1] = value;
            this._values = n;
        }
        /// <summary>
        /// Removes any/all copies of the given value from the property's values
        /// </summary>
        /// <param name="value"></param>
        public void RemoveValue(string value)
		{
            List<string> newValues = new List<string>();
			for (int i = 0; i < _values.Length; i++)
			{
                if (_values[i] != value) { newValues.Add(_values[i]); }
			}
            _values = newValues.ToArray();
		}
        public override string ToString()
		{
            string str = $"{this.type}:{this.name}=";
			for (int i = 0; i < _values.Length; i++)
			{
                str += _values[i];
                if (i != _values.Length - 1)
				{
                    str += ',';
				}
			}
            return str;
		}
        [Serializable]
        public class Collection : ICollection<Property>
        {
            public List<Property> properties = new List<Property>();

            public int Count { get { return properties.Count; } }

            public bool IsReadOnly { get { return false; } }

            public void Add(Property item)
            {
                if (!properties.Contains(item))
                {
                    properties.Add(item);
                }
            }

            public void Clear()
            {
                properties.Clear();
            }

            public bool Contains(Property item)
            {
                return properties.Contains(item);
            }

            public void CopyTo(Property[] array, int arrayIndex)
            {
				for (int i = 0; i < properties.Count; i++)
				{
                    array[i + arrayIndex] = properties[i];
				}
            }

            public bool Remove(Property item)
            {
                bool success = properties.Remove(item);
                return success;
            }

            public IEnumerator<Property> GetEnumerator()
            {
                return properties.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return properties.GetEnumerator();
            }
        }
    }
}