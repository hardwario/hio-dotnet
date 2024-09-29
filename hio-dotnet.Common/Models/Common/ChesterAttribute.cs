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
        [JsonPropertyName("vendor_name")]
        public string VendorName { get; set; } = Defaults.Unknown;

        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = Defaults.Unknown;

        [JsonPropertyName("hw_variant")]
        public string HwVariant { get; set; } = Defaults.Unknown;

        [JsonPropertyName("hw_revision")]
        public string HwRevision { get; set; } = Defaults.Unknown;

        [JsonPropertyName("fw_name")]
        public string FwName { get; set; } = Defaults.Unknown;

        [JsonPropertyName("fw_version")]
        public string FwVersion { get; set; } = Defaults.Unknown;

        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; } = Defaults.UnknownSerialNumber;
    }
}
