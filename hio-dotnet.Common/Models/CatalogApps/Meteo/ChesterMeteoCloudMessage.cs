using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Meteo
{
    public class ChesterMeteoCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("weather_station")]
        public WeatherStation WeatherStation { get; set; } = new WeatherStation();

        [JsonPropertyName("barometer")]
        public BarometerMeasurements Barometer { get; set; } = new BarometerMeasurements();

        [JsonPropertyName("hygrometer")]
        public Hygrometer Hygrometer { get; set; } = new Hygrometer();

        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer> W1Thermometers { get; set; } = new List<W1_Thermometer>();

        [JsonPropertyName("soil_sensors")]
        public List<SoilMeasurements> SoilSensors { get; set; } = new List<SoilMeasurements>();

    }
}
