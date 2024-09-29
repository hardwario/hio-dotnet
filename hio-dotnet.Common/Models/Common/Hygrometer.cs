using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public class Hygrometer
    {
        [JsonPropertyName("temperature")]
        public Temperature Temperature { get; set; } = new Temperature();

        [JsonPropertyName("humidity")]
        public Humidity Humidity { get; set; } = new Humidity();
    }
}
