﻿using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Dust
{
    public class ChesterDustCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("hygro")]
        public Hygro Hygro { get; set; } = new Hygro();

        [SimulationAttribute(false)]
        [JsonPropertyName("pm")]
        public DustSensorMeasurements DustSensor { get; set; } = new DustSensorMeasurements();

        [SimulationMeasurementAttribute(false, 25, 35, true, false, 0, 2)]
        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer> W1Thermometers { get; set; } = new List<W1_Thermometer>();

    }
}
