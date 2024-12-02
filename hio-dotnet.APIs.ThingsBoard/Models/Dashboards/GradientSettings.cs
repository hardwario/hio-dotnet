using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class GradientSettings
    {
        [JsonPropertyName("start")]
        public int Start { get; set; } = 100;

        [JsonPropertyName("end")]
        public int End { get; set; } = 0;
    }
}
