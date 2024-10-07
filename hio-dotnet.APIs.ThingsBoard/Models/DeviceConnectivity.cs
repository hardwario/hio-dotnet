﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class DeviceConnectivity
    {
        [JsonPropertyName("coap")]
        public CoapConnectionInfo Coap { get; set; } = new CoapConnectionInfo();
        [JsonPropertyName("mqtt")]
        public MQTTConnectionInfo Mqtt { get; set; } = new MQTTConnectionInfo();
        [JsonPropertyName("http")]
        public HttpConnectionInfo Http { get; set; } = new HttpConnectionInfo();
    }
}
