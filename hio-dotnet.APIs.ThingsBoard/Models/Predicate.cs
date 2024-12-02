﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Predicate
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("operation")]
        public string Operation { get; set; } = string.Empty;
        [JsonPropertyName("value")]
        public Value Value { get; set; } = new Value();
    }
}
