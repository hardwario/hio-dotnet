using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public class WMBusMetersHcaBase : WMBusMetersCommon
    {
        [JsonPropertyName("current_hca")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentHca { get; set; }
        
        [JsonPropertyName("previous_hca")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? PreviousHca { get; set; }

        [JsonPropertyName("consumption_at_set_date_hca")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? ConsumptionAtSetDateHCA { get; set; }

        [JsonPropertyName("current_consumption_hca")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentConsumptionHca { get; set; }

        [JsonPropertyName("current_room_temp_c")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentRoomTempC { get; set; }

        [JsonPropertyName("current_temp_c")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentTempC { get; set; }

        [JsonPropertyName("device_date_time")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DeviceDateTimeStr { get; set; }

        //[JsonIgnore]
        //public DateTime? DeviceDateTime { get => DateTime.ParseExact(DeviceDateTimeStr ?? string.Empty, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); }

        [JsonPropertyName("set_date")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SetDateStr { get; set; }

        //[JsonIgnore]
        //public DateTime? SetDateTime { get => DateTime.ParseExact(DeviceDateTimeStr ?? string.Empty, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture); }

        [JsonIgnore]
        public DateTime? DeviceDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(DeviceDateTimeStr))
                {
                    return DateTime.MinValue;
                }
                else
                {
                    try
                    {
                        var dt = WMBusHelpers.ParseToUtc(DeviceDateTimeStr);//DateTime.ParseExact(DeviceDateTimeStr ?? "2000-01-01", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                    catch (Exception)
                    {
                        return DateTime.MinValue;
                    }
                }
            }
        }

        [JsonIgnore]
        public DateTime? SetDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(SetDateStr))
                {
                    return DateTime.MinValue;
                }
                else
                {
                    try
                    {
                        var dt = WMBusHelpers.ParseToUtc(SetDateStr);//DateTime.ParseExact(SetDateStr ?? "2000-01-01", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
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
