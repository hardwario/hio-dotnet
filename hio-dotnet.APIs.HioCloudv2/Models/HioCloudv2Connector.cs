using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{

    public class HioCloudv2Connector
    {
        [JsonPropertyName("direction")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Direction { get; set; } = "up";

        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("retry_delays")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<int>? RetryDelays { get; set; } = new List<int>()
        {
                10,
                30,
                60,
                600,
                1800,
                3600,
                10800,
                21600,
                43200
        };

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? State { get; set; } = "enabled";

        [JsonPropertyName("tags")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<HioCloudv2Tag>? Tags { get; set; } = new List<HioCloudv2Tag>();

        [JsonPropertyName("transformation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Transformation { get; set; } = "" +
            "function main(job) {\r\n  let body = job.message.body\r\n  return {\r\n    \"method\": \"POST\",\r\n    \"url\": \"https://thingsboard.hardwario.com/api/v1/DEVICE_CONNECTION_TOKEN/telemetry\",\r\n    \"header\": { \r\n      \"Content-Type\": \"application/json\" \r\n    },\r\n    \"data\": body\r\n  }\r\n}";

        [JsonPropertyName("triggers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Triggers { get; set; } = new List<string>()
        {
            "data"
        };

        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; set; } = "webhook";


        public HioCloudv2Connector WithTrigger(string trigger)
        {
            if (trigger != "data" && trigger != "session" && trigger != "config" && trigger != "stats" && trigger != "codec")
                throw new ArgumentException("Invalid direction. Expected values are data, session, config, stats or codec.");

            if (Triggers == null)
                Triggers = new List<string>();

            if (!Triggers.Contains(trigger))
                Triggers.Add(trigger);

            return this;
        }
        public HioCloudv2Connector WithDirection(string dir)
        {
            if (dir != "up" && dir != "down")
                throw new ArgumentException("Invalid direction. Expected values are up, down.");

            Direction = dir;
            return this;
        }
        public HioCloudv2Connector WithName(string name)
        {
            Name = name;
            return this;
        }
        public HioCloudv2Connector WithThingsBoardConnectionToken(string connectionToken)
        {
            Transformation = GetConnectorString(connectionToken);
            return this;
        }

        public HioCloudv2Connector WithTag(HioCloudv2Tag tag)
        {
            if (Tags == null)
                Tags = new List<HioCloudv2Tag>();

            Tags.Add(tag);
            return this;
        }

        public static string GetConnectorString(string connectionToken, string basedomain = "https://thingsboard.hardwario.com/")
        {
            var url = $"{basedomain}api/v1/{connectionToken}/telemetry";

            return "function main(job) {\r\n  let body = job.message.body\r\n  return {\r\n    \"method\": \"POST\",\r\n    \"url\": \"" +
                url
                + "\",\r\n    \"header\": { \r\n      \"Content-Type\": \"application/json\" \r\n    },\r\n    \"data\": body\r\n  }\r\n}";
        }
    }
}
