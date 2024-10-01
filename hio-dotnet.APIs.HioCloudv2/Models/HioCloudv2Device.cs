using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{
    public class HioCloudv2Device
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("space_id")]
        public Guid SpaceId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("last_seen")]
        public DateTime? LastSeen { get; set; }

        [JsonPropertyName("dashboard_type")]
        public string? DashboardType { get; set; }

        [JsonPropertyName("config_hash")]
        public string? ConfigHash { get; set; }

        [JsonPropertyName("decoder_hash")]
        public string? DecoderHash { get; set; }

        [JsonPropertyName("encoder_hash")]
        public string? EncoderHash { get; set; }

        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("label")]
        public Dictionary<string, string>? Label { get; set; }

        [JsonPropertyName("serial_number")]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("session_id")]
        public int SessionId { get; set; }

        [JsonPropertyName("tags")]
        public List<HioCloudv2Tag>? Tags { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

    }
}
