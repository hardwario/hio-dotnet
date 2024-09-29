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
        [JsonPropertyName("acceleration_x")]
        public double AccelerationX { get; set; } = 0.0;

        [JsonPropertyName("acceleration_y")]
        public double AccelerationY { get; set; } = 0.0;

        [JsonPropertyName("acceleration_z")]
        public double AccelerationZ { get; set; } = 0.0;

        [JsonPropertyName("orientation")]
        public int Orientation { get; set; } = 0;
    }
}
