﻿using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class SimpleTimeDoubleMeasurement : TimestampState
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("value")]
        public double Value { get; set; } = 0.0;
    }
}
