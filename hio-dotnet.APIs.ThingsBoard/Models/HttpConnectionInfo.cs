﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class HttpConnectionInfo
    {
        [JsonPropertyName("http")]
        public string Http { get; set; } = string.Empty;
    }
}
