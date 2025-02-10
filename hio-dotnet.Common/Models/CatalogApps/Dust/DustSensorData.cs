using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Dust
{
    public class DustSensorData : TimestampState
    {

        [SimulationAttribute(false, 10, 100, false, false, 0.02)]
        [JsonPropertyName("pm1")]
        public int PM_1 { get; set; } = 0;

        [SimulationAttribute(false, 10, 100, false, false, 0.02)]
        [JsonPropertyName("pm2_5")]
        public int PM_2_5 { get; set; } = 0;

        [SimulationAttribute(false, 10, 100, false, false, 0.02)]
        [JsonPropertyName("pm10")]
        public int PM_10 { get; set; } = 0;

        [SimulationAttribute(false, 40000, 80000, false, false, 0.02)]
        [JsonPropertyName("q0_3um")]
        public int Q_0_3_um { get; set; } = 0;

        [SimulationAttribute(false, 500, 4000, false, false, 0.02)]
        [JsonPropertyName("q0_5um")]
        public int Q_0_5_um { get; set; } = 0;

        [SimulationAttribute(false, 22300.0, 24000.0, false, false, 0.02)]
        [JsonPropertyName("q1_0um")]
        public int Q_1_0_um { get; set; } = 0;

        [SimulationAttribute(false, 24300.0, 26300.0, false, false, 0.02)]
        [JsonPropertyName("q2_5um")]
        public int Q_2_5_um { get; set; } = 0;

        [SimulationAttribute(false, 24300.0, 26300.0, false, false, 0.02)]
        [JsonPropertyName("q5_0um")]
        public int Q_5_0_um { get; set; } = 0;

        [SimulationAttribute(false, 24300.0, 26300.0, false, false, 0.02)]
        [JsonPropertyName("q10um")]
        public int Q_10_um { get; set; } = 0;

        [SimulationAttribute(false, 5, 50, false, false, 0.02)]
        [JsonPropertyName("tsp")]
        public int Tsp { get; set; } = 0;
    }
}
