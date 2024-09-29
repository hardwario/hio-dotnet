using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.ClimeIAQ
{
    public class IAQ_Sensor
    {
        [JsonPropertyName("temperature")]
        public Temperature Temperature { get; set; } = new Temperature();

        [JsonPropertyName("humidity")]
        public Humidity Humidity { get; set; } = new Humidity();

        [JsonPropertyName("illuminance")]
        public Illuminance Illuminance { get; set; } = new Illuminance();

        [JsonPropertyName("altitude")]
        public Altitude Altitude { get; set; } = new Altitude();

        [JsonPropertyName("pressure")]
        public Pressure Pressure { get; set; } = new Pressure();

        [JsonPropertyName("co2_conc")]
        public CO2_Concentration CO2_Concentration { get; set; } = new CO2_Concentration();

        [JsonPropertyName("motion_count")]
        public MotionCount MotionCount { get; set; } = new MotionCount();

        [JsonPropertyName("press_count")]
        public PressCount PressCount { get; set; } = new PressCount();
    }
}
