﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class TransportConfiguration
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("coapDeviceTypeConfiguration")]
        public CoapDeviceTypeConfiguration CoapDeviceTypeConfiguration { get; set; } = new CoapDeviceTypeConfiguration();
        [JsonPropertyName("clientSettings")]
        public ClientSettings ClientSettings { get; set; } = new ClientSettings();
    }
}
