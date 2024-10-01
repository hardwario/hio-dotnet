using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{
    public class HioCloudv2Message
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("codec_hash")]
        public string CodecHash { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }

        [JsonPropertyName("device_name")]
        public string DeviceName { get; set; }

        [JsonPropertyName("device_serial_number")]
        public string DeviceSerialNumber { get; set; }

        [JsonPropertyName("direction")]
        public string Direction { get; set; }

        [JsonPropertyName("down_attempt")]
        public int DownAttempt { get; set; }

        [JsonPropertyName("down_scheduled_at")]
        public string DownScheduledAt { get; set; }

        [JsonPropertyName("down_state")]
        public string DownState { get; set; }

        [JsonPropertyName("down_state_at")]
        public string DownStateAt { get; set; }

        [JsonPropertyName("raw")]
        public string Raw { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
