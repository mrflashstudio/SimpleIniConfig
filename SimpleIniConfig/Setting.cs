using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleIniConfig
{
    public class Setting
    {
        public string Name { get; private set; }

        public string RawValue { get; set; }

        public Setting(string name, string rawValue)
        {
            Name = name;
            RawValue = rawValue;
        }

        public Setting(string rawLine)
        {
            string[] split = rawLine.Split(new char[] { '=' }, 2);

            Name = split.First().Trim();
            RawValue = split.Last().Trim();
        }

        public T AsEnum<T>()
        {
            int intValue = 0;
            if (int.TryParse(RawValue, out intValue))
                return (T)(object)intValue;
            else
                return (T)Enum.Parse(typeof(T), RawValue, true);
        }

        public T AsEnumArray<T>()
        {
            string enumAssemblyQualifiedName = typeof(T).AssemblyQualifiedName.Replace("[]", string.Empty);
            var enumType = Type.GetType(enumAssemblyQualifiedName);

            string[] enumNames = ((IEnumerable)Enum.GetValues(enumType)).Cast<object>().Select(e => e.ToString()).ToArray();
            int[] enumValues = (int[])Enum.GetValues(enumType);

            return (T)(object)Array.ConvertAll(RawValue.Split(','), e => enumValues[Array.IndexOf(enumNames, e)]);
        }

        public string AsString()
        {
            return RawValue.ToString().Trim('"');
        }

        public string[] AsStringArray()
        {
            Match match = Regex.Match(RawValue, "[\\\"][^\\\"]*[\\\"][,]*");

            List<string> elements = new List<string>();

            while (match.Success)
            {
                string element = match.Value.TrimEnd(',');
                elements.Add(element.Trim('"'));
                match = match.NextMatch();
            }

            return elements.ToArray();
        }

        public bool AsBoolean()
        {
            return RawValue.ToLower() == "true" || RawValue == "1";
        }

        public bool[] AsBooleanArray()
        {
            return Array.ConvertAll(RawValue.Split(','), b => b.ToLower() == "true" || b == "1");
        }

        public int AsInt()
        {
            return int.Parse(RawValue);
        }

        public int[] AsIntArray()
        {
            return Array.ConvertAll(RawValue.Split(','), int.Parse);
        }

        public float AsFloat()
        {
            return float.Parse(RawValue, Config.NumberFormat);
        }

        public float[] AsFloatArray()
        {
            return Array.ConvertAll(RawValue.Split(','), f => float.Parse(f, Config.NumberFormat));
        }

        public double AsDouble()
        {
            return double.Parse(RawValue, Config.NumberFormat);
        }

        public double[] AsDoubleArray()
        {
            return Array.ConvertAll(RawValue.Split(','), f => double.Parse(f, Config.NumberFormat));
        }

        public override string ToString() => $"{Name} = {RawValue}";
    }
}