﻿using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Radon
{
    public class ChamberHumidityMeasurements
    {
        [JsonPropertyName("measurements")]
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
