using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class ChesterAttribute
    {
        [SimulationAttribute(true)]
        [JsonPropertyName("vendor_name")]
        public string VendorName { get; set; } = Defaults.Unknown;

        [SimulationAttribute(true)]
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = Defaults.Unknown;

        [SimulationAttribute(true)]
        [JsonPropertyName("hw_variant")]
        public string HwVariant { get; set; } = Defaults.Unknown;

        [SimulationAttribute(true)]
        [JsonPropertyName("hw_revision")]
        public string HwRevision { get; set; } = Defaults.Unknown;

        [SimulationAttribute(true)]
        [JsonPropertyName("fw_name")]
        public string FwName { get; set; } = Defaults.Unknown;

        [SimulationAttribute(true)]
        [JsonPropertyName("fw_version")]
        public string FwVersion { get; set; } = Defaults.Unknown;

        [SimulationAttribute(true)]
        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; } = Defaults.UnknownSerialNumber;
    }
}
