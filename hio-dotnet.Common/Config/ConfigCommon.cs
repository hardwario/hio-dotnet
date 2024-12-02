using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
