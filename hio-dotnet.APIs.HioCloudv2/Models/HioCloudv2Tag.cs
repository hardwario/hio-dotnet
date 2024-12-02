using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{
    public class HioCloudv2Tag
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        public HioCloudv2Tag WithName(string name)
        {
            Name = name;
            return this;
        }
        
        public HioCloudv2Tag WithColor(string color)
        {
            if (!Regex.IsMatch(color, "^#[0-9A-Fa-f]{6}$"))
            {
                throw new ArgumentException("Invalid color format. Expected format is #RRGGBB.");
            }

            Color = color;
            return this;
        }
    }
}
