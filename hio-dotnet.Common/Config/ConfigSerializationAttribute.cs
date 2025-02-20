using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigSerializationAttribute : Attribute
    {
        public ConfigSerializationAttribute(bool isConfigParam, bool isStatusParam, string configcategory = "", string configParamName = "", string configParamHelp = "")
        {
            IsConfigParam = isConfigParam;
            ConfigParamName = configParamName;
            IsStatusParam = isStatusParam;
            ConfigCategory = configcategory;
            ConfigParamHelp = configParamHelp;
        }

        public bool IsConfigParam { get; set; } = false;
        public bool IsStatusParam { get; set; } = false;
        public string ConfigParamName { get; set; } = string.Empty;
        public string ConfigCategory { get; set; } = string.Empty;
        public string ConfigParamHelp { get; set; } = string.Empty;
    }
}
