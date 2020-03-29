using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleIniConfig
{
    public class Config
    {
        public static readonly NumberFormatInfo NumberFormat = new CultureInfo(@"en-US", false).NumberFormat;

        private string pathToConfig = @"config.ini";
        private List<Setting> localConfig = new List<Setting>();

        public Config(string pathToConfig = null)
        {
            if (!string.IsNullOrEmpty(pathToConfig))
                this.pathToConfig = pathToConfig;

            loadConfig();
        }

        ~Config() => saveConfig();

        public T GetValue<T>(string settingName, T defaultValue)
        {
            var foundSetting = localConfig.Find(s => s.Name == settingName);
            if (foundSetting != default)
            {
                var typeInfo = typeof(T).GetTypeInfo();
                string typeName = typeInfo.Name.Replace("[]", string.Empty);
                string enumAssemblyQualifiedName = typeInfo.AssemblyQualifiedName.Replace("[]", string.Empty);

                if (Type.GetType(enumAssemblyQualifiedName).IsEnum)
                {
                    if (typeInfo.IsArray)
                        return foundSetting.AsEnumArray<T>();

                    return foundSetting.AsEnum<T>();
                }

                switch (typeName)
                {
                    case "String":
                        if (typeInfo.IsArray)
                            return (T)(object)foundSetting.AsStringArray();

                        return (T)(object)foundSetting.AsString();
                    case "Boolean":
                        if (typeInfo.IsArray)
                            return (T)(object)foundSetting.AsBooleanArray();

                        return (T)(object)foundSetting.AsBoolean();
                    case "Int32":
                        if (typeInfo.IsArray)
                            return (T)(object)foundSetting.AsIntArray();

                        return (T)(object)foundSetting.AsInt();
                    case "Single":
                        if (typeInfo.IsArray)
                            return (T)(object)foundSetting.AsFloatArray();

                        return (T)(object)foundSetting.AsFloat();
                    case "Double":
                        if (typeInfo.IsArray)
                            return (T)(object)foundSetting.AsDoubleArray();

                        return (T)(object)foundSetting.AsDouble();
                    default:
                        return (T)(object)foundSetting.RawValue;
                }
            }
            else
                createNewEntry(settingName, defaultValue);

            return defaultValue;
        }

        public void SetValue<T>(string settingName, T newValue)
        {
            var foundSetting = localConfig.Find(s => s.Name == settingName);
            if (foundSetting != default)
            {
                foundSetting.RawValue = formatValue(newValue);
                saveConfig();
            }
            else
                createNewEntry(settingName, newValue);
        }

        private void createNewEntry<T>(string settingName, T value)
        {
            localConfig.Add(new Setting(settingName, formatValue(value)));
            saveConfig();
        }

        private string formatValue<T>(T value)
        {
            var typeInfo = typeof(T).GetTypeInfo();
            string typeName = typeInfo.Name.Replace("[]", string.Empty);

            switch (typeName)
            {
                case "String":
                    if (typeInfo.IsArray)
                        return $"\"{string.Join("\",\"", (string[])(object)value)}\"";

                    return $"\"{value}\"";
                case "Single":
                    if (typeInfo.IsArray)
                        return string.Join(",", ((float[])(object)value).Select(f => f.ToString(NumberFormat)));

                    return ((float)(object)value).ToString(NumberFormat);
                case "Double":
                    if (typeInfo.IsArray)
                        return string.Join(",", ((double[])(object)value).Select(d => d.ToString(NumberFormat)));

                    return ((double)(object)value).ToString(NumberFormat);
                default:
                    if (typeInfo.IsArray)
                        return string.Join(",", ((IEnumerable)value).Cast<object>().Select(x => x.ToString()));

                    return value.ToString();
            }
        }

        private void ensureConfigFileExists()
        {
            if (!File.Exists(pathToConfig))
                File.Create(pathToConfig).Close();
        }

        private void loadConfig()
        {
            ensureConfigFileExists();

            string[] lines = File.ReadAllLines(pathToConfig);
            foreach (string line in lines)
                localConfig.Add(new Setting(line));
        }

        private void saveConfig()
        {
            ensureConfigFileExists();
            File.WriteAllText(pathToConfig, string.Empty);

            foreach (var setting in localConfig)
                File.AppendAllText(pathToConfig, setting.ToString() + Environment.NewLine);
        }
    }
}