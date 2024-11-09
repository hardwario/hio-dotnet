using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{
    public class HioCloudv2Device
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? Id { get; set; }

        [JsonPropertyName("space_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? SpaceId { get; set; }

        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("comment")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Comment { get; set; }

        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("last_seen")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? LastSeen { get; set; }

        [JsonPropertyName("dashboard_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DashboardType { get; set; }

        [JsonPropertyName("config_hash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ConfigHash { get; set; }

        [JsonPropertyName("decoder_hash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DecoderHash { get; set; }

        [JsonPropertyName("encoder_hash")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? EncoderHash { get; set; }

        [JsonPropertyName("external_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ExternalId { get; set; }

        [JsonPropertyName("label")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Label { get; set; }

        [JsonPropertyName("serial_number")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("session_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SessionId { get; set; }

        [JsonPropertyName("tags")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<HioCloudv2Tag>? Tags { get; set; }

        [JsonPropertyName("token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Token { get; set; }


        public HioCloudv2Device WithName(string name)
        {
            Name = name;
            return this;
        }
        public HioCloudv2Device WithSerialNumber(string sn)
        {
            SerialNumber = sn;
            return this;
        }
        public HioCloudv2Device WithTag(HioCloudv2Tag tag)
        {
            if (Tags == null)
                Tags = new List<HioCloudv2Tag>();

            Tags.Add(tag);
            return this;
        }
        public HioCloudv2Device WithToken(string token)
        {
            Token = token;
            return this;
        }

        public HioCloudv2Device WithSpaceId(Guid spaceid)
        {
            SpaceId = spaceid;
            return this;
        }

        public static string GenerateClaimToken()
        {
            byte[] randomNumber = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return BitConverter.ToString(randomNumber).Replace("-", string.Empty).ToLower();
        }

    }
}
