using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Accelerometer
    {
        [SimulationAttribute(false, 0.01, 0.1)]
        [JsonPropertyName("accel_x")]
        public double AccelerationX { get; set; } = 0.0;

        [SimulationAttribute(false, 0.01, 0.1)]
        [JsonPropertyName("accel_y")]
        public double AccelerationY { get; set; } = 0.0;

        [SimulationAttribute(false, 0.01, 0.1)]
        [JsonPropertyName("accel_z")]
        public double AccelerationZ { get; set; } = 0.0;

        [SimulationAttribute(false, 0, 6)]
        [JsonPropertyName("orientation")]
        public int Orientation { get; set; } = 0;
    }
}
