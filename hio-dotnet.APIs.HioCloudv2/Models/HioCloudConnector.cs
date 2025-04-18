﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud.Models
{

    public class HioCloudConnector
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
        public List<HioCloudTag>? Tags { get; set; } = new List<HioCloudTag>();

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

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SpaceId { get; set; }
        public HioCloudConnector WithTrigger(string trigger)
        {
            if (trigger != "data" && trigger != "session" && trigger != "config" && trigger != "stats" && trigger != "codec")
                throw new ArgumentException("Invalid direction. Expected values are data, session, config, stats or codec.");

            if (Triggers == null)
                Triggers = new List<string>();

            if (!Triggers.Contains(trigger))
                Triggers.Add(trigger);

            return this;
        }
        public HioCloudConnector WithDirection(string dir)
        {
            if (dir != "up" && dir != "down")
                throw new ArgumentException("Invalid direction. Expected values are up, down.");

            Direction = dir;
            return this;
        }
        public HioCloudConnector WithName(string name)
        {
            Name = name;
            return this;
        }
        public HioCloudConnector WithThingsBoardConnectionToken(string connectionToken)
        {
            Transformation = GetConnectorString(connectionToken);
            return this;
        }

        public HioCloudConnector WithTag(HioCloudTag tag)
        {
            if (Tags == null)
                Tags = new List<HioCloudTag>();

            Tags.Add(tag);
            return this;
        }

        public static string GetConnectorString(string connectionToken, string basedomain = "https://thingsboard.hardwario.com/")
        {
            var url = $"{basedomain}api/v1/{connectionToken}";

            return "function main(job) {\r\n" + 
                   "    let body = job.message.body\r\n" +
                   "    body.uptime = job.message.body.system.uptime;\r\n" +
                   "    body.voltage_load = job.message.body.system.voltage_load;\r\n" +
                   "    body.voltage_rest = job.message.body.system.voltage_rest;\r\n" +
                   "    \r\n" +
                   "        return {\r\n" + 
                   "            \"method\": \"POST\",\r\n"+
                   "            \"url\": \"" + url + "/telemetry\",\r\n" +
                   "            \"header\": { \r\n" + 
                   "                \"Content-Type\": \"application/json\" \r\n" + 
                   "            },\r\n" + 
                   "            \"data\": body\r\n" + 
                   "        }\r\n" + 
                   "}";
        }

        public static string GetConnectorStringMultipleDevices(Dictionary<string,string> connectionTokens, Dictionary<string,string> sndata = null, string basedomain = "https://thingsboard.hardwario.com/")
        {
            var burl = basedomain;
            if (!burl.EndsWith("/"))
            {
                burl += "/";
            }

            var url = $"{burl}api/v1/";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\r\n");
            if (sndata != null)
            {
                stringBuilder.AppendLine("\tvar new_body ={};\r\n");
                stringBuilder.AppendLine("\t\r\n");
                var index = 0;
                foreach (var sn in sndata)
                {
                    if (index == 0)
                    {
                        stringBuilder.AppendLine($"\tif (sn == {sn.Key}) {{\r\n");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"\telse if (sn == {sn.Key}) {{\r\n");
                    }
                    var data = sn.Value.Replace("var data =", "new_body =");
                    stringBuilder.AppendLine($"\t\t{data}\r\n");

                    stringBuilder.AppendLine("\t}\r\n");
                }
            }

            stringBuilder.AppendLine("\r\n");
            stringBuilder.AppendLine("\tvar snToAccessTokenDict ={\r\n");
            var item = 0;
            foreach (var (key, value) in connectionTokens)
            {
                if (item < connectionTokens.Count - 1)
                    stringBuilder.Append($"\t\t\"{key}\": \"{value}\",\r\n");
                else
                    stringBuilder.Append($"\t\t\"{key}\": \"{value}\"\r\n");
                item++;
            }
            stringBuilder.AppendLine("\t};\r\n");

            var dictString = stringBuilder.ToString();

            var bodyname = "body";

            if (sndata != null)
            {
                bodyname = "new_body";
            }

            return "function main(job) {\r\n" +
                   "    let body = job.message.body\r\n" +
                   "    body.uptime = job.message.body.system.uptime;\r\n" +
                   "    body.voltage_load = job.message.body.system.voltage_load;\r\n" +
                   "    body.voltage_rest = job.message.body.system.voltage_rest;\r\n" +
                   "    var accesstoken = '';\r\n" +
                        dictString     +
                   "    accesstoken = snToAccessTokenDict[job.device.serial_number];\r\n" +
                   "    var url = \"" + url + "\" + accesstoken + '/telemetry';\r\n" +
                   "    \r\n" +
                   "        return {\r\n" +
                   "            \"method\": \"POST\",\r\n" +
                   "            \"url\": url,\r\n" +
                   "            \"header\": { \r\n" +
                   "                \"Content-Type\": \"application/json\" \r\n" +
                   "            },\r\n" +
                   "            \"data\": " + bodyname + "\r\n" +
                   "        }\r\n" +
                   "}";
        }


        public static string GetConnectorStringMultipleDevices_FromActiveJSCode(Dictionary<string, string> connectionTokens, Dictionary<string, string> sndata = null, string basedomain = "https://thingsboard.hardwario.com/")
        {
            var burl = basedomain;
            if (!burl.EndsWith("/"))
            {
                burl += "/";
            }

            var url = $"{burl}api/v1/";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\r\n");
            if (sndata != null)
            {
                stringBuilder.AppendLine("\t\r\n");
                var index = 0;
                foreach (var sn in sndata)
                {
                    if (index == 0)
                    {
                        stringBuilder.AppendLine($"\tif (sn == '{sn.Key}') {{\r\n");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"\telse if (sn == '{sn.Key}') {{\r\n");
                    }
                    var data = sn.Value;
                    stringBuilder.AppendLine($"\t\t{data}\r\n");

                    stringBuilder.AppendLine("\t}\r\n");
                }
            }

            stringBuilder.AppendLine("\r\n");
            stringBuilder.AppendLine("\tvar snToAccessTokenDict ={\r\n");
            var item = 0;
            foreach (var (key, value) in connectionTokens)
            {
                if (item < connectionTokens.Count - 1)
                    stringBuilder.Append($"\t\t\"{key}\": \"{value}\",\r\n");
                else
                    stringBuilder.Append($"\t\t\"{key}\": \"{value}\"\r\n");
                item++;
            }
            stringBuilder.AppendLine("\t};\r\n");

            var dictString = stringBuilder.ToString();

            var bodyname = "body";

            if (sndata != null)
            {
                bodyname = "data";
            }

            return "function main(job) {\r\n" +
                   "    let body = job.message.body\r\n" +
                   "    var data = [];\r\n" +
                   "    const sharedtimestamp = new Date(job.message.created_at).getTime() * 1000;\r\n" +
                   "    var accesstoken = '';\r\n" +
                   "    var sn = job.device.serial_number;\r\n" +
                        dictString +
                   "    accesstoken = snToAccessTokenDict[job.device.serial_number];\r\n" +
                   "    var url = \"" + url + "\" + accesstoken + '/telemetry';\r\n" +
                   "    \r\n" +
                   "        return {\r\n" +
                   "            \"method\": \"POST\",\r\n" +
                   "            \"url\": url,\r\n" +
                   "            \"header\": { \r\n" +
                   "                \"Content-Type\": \"application/json\" \r\n" +
                   "            },\r\n" +
                   "            \"data\": " + bodyname + "\r\n" +
                   "        }\r\n" +
                   "}";
        }
    }
}
