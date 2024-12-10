using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Config
{
    public class ConfigCommon
    {
        #region HelperParsingFunctions
        public T? GetEnumParameter<T>(string line) where T : struct, Enum
        {
            var param = GetStringParameter(line);
            if (param != null)
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), param, true);
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException($"Value '{param}' does not fit for enum '{typeof(T).Name}'.");
                }
            }
            return null;
        }

        public bool GetBoolParameter(string line)
        {
            var param = GetStringParameter(line);
            if (param != null)
            {
                try
                {
                    return bool.Parse(param);
                }
                catch 
                { 
                    throw new FormatException("Wrong input format. Bool is expected. (true/false)");
                }
            }
            return false;
        }

        public int? GetIntParameter(string line)
        {
            var param = GetStringParameter(line);
            if (param != null)
            {
                try
                {
                    return int.Parse(param);
                }
                catch 
                {
                    throw new FormatException("Wrong input format. Int is expected.");
                }
            }
            return null;
        }

        public string? GetStringParameter(string line)
        {
            var split = line.Split(' ');
            if (split.Length > 1)
            {
                return split[1].Trim();
            }
            return null;
        }

        public (string Property, string Value) ParseStringProperty(string configLine)
        {
            int spaceIndex = configLine.IndexOf(' ');

            if (spaceIndex == -1)
            {
                return (configLine, string.Empty);
            }
            string property = configLine.Substring(0, spaceIndex);
            string value = configLine.Substring(spaceIndex + 1);

            return (property, value);
        }
        #endregion

        public string GetEnumString(Enum enumValue)
        {
            return enumValue.ToString().ToLowerInvariant();
        } 
        public string GetEnumString<T>(T enumValue) where T : Enum
        {
            return enumValue.ToString().ToLowerInvariant();
        }

        public string GetBoolString(bool value)
        {
            return value.ToString().ToLowerInvariant();
        }

        public string GetIntString(int value)
        {
            return value.ToString();
        }

        public void ParseLineToProp(string line)
        {
            var split = ParseStringProperty(line);

            if (!string.IsNullOrEmpty(split.Property))
            {
                var key = split.Property.Trim().ToLower();
                var value = split.Value.Trim();

                #region SpecificParsingPreparation
                if (key.Contains("antenna"))
                {
                    if (value.Contains("int") && !value.Contains("internal"))
                        value = value.Replace("int", "internal");
                    if (value.Contains("ext") && !value.Contains("external"))
                        value = value.Replace("ext", "external");
                }
                #endregion

                var props = this.GetType().GetProperties();
                foreach (var prop in props)
                {
                    var attr = prop.GetCustomAttribute<ConfigSerializationAttribute>();
                    if (attr != null && (attr.IsConfigParam || attr.IsStatusParam) && attr.ConfigParamName == key)
                    {
                        if (prop.PropertyType == typeof(bool))
                        {
                            prop.SetValue(this, bool.Parse(value));
                        }
                        else if (prop.PropertyType == typeof(int))
                        {
                            prop.SetValue(this, int.Parse(value));
                        }
                        else if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(this, value);
                        }
                        else if (prop.PropertyType.IsEnum)
                        {
                            try
                            {
                                var enumValue = Enum.Parse(prop.PropertyType, value, true);
                                prop.SetValue(this, enumValue);
                            }
                            catch (ArgumentException)
                            {
                                throw new InvalidOperationException($"Value '{value}' does not fit for enum '{prop.PropertyType.Name}'.");
                            }
                        }
                        else
                        {
                            prop.SetValue(this, value);
                        }
                    }
                }
            }
        }

        #region GenerateFunctions
        public string GetWholeConfig()
        {
            var stringBuilder = new StringBuilder();

            var properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                var configLine = GetConfigLine(property.Name);

                if (!string.IsNullOrWhiteSpace(configLine))
                {
                    stringBuilder.AppendLine(configLine);
                }
            }

            return stringBuilder.ToString();
        }

        public string GetConfigLine(string propertyName)
        {
            string tag = string.Empty;
            string paramValue = string.Empty;

            var propertyInfo = this.GetType().GetProperty(propertyName);

            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found in LTEConfig.");

            var value = propertyInfo.GetValue(this);

            var attr = propertyInfo.GetCustomAttribute<ConfigSerializationAttribute>();
            if (attr != null)
            {
                if (attr.IsConfigParam)
                {
                    tag = attr.ConfigParamName;

                    if (value is string stringValue)
                    {
                        paramValue = stringValue;
                    }
                    else if (value is bool boolValue)
                    {
                        paramValue = GetBoolString(boolValue);
                    }
                    else if (value is int intValue)
                    {
                        paramValue = GetIntString(intValue);
                    }
                    else if (value is Enum enumValue)
                    {
                        paramValue = GetEnumString(enumValue);
                    }
                    else
                    {
                        paramValue = value.ToString();
                    }

                    if (tag == "antenna")
                    {
                        if (paramValue.Contains("internal"))
                            paramValue = "int";
                        if (paramValue.Contains("external"))
                            paramValue = "ext";
                    }

                    return $"{attr.ConfigCategory} config {tag} {paramValue}";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
