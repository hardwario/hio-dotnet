﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class MQTTConnectionInfo
    {
        [JsonPropertyName("mqtt")]
        public string Mqtt { get; set; } = string.Empty;
        [JsonPropertyName("docker")]
        public DockerInfo? Docker { get; set; }
    }
}
