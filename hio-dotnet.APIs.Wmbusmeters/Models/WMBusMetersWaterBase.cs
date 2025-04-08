using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public class WMBusMetersWaterBase : WMBusMetersCommon
    {
        [JsonPropertyName("total_m3")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TotalM3 { get; set; }

        [JsonPropertyName("flow_m3h")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? FlowM3h { get; set; }


        [JsonPropertyName("meter_datetime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? MeterDateTimeStr { get; set; }

        [JsonIgnore]
        public DateTime? MeterDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(MeterDateTimeStr))
                {
                    return DateTime.MinValue;
                }
                else
                {
                    try
                    {
                        return DateTime.ParseExact(MeterDateTimeStr ?? "2000-01-01", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return DateTime.MinValue;
                    }
                }
            }
        }
    }
}
