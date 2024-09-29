using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Meteo
{
    public class WeatherStation
    {
        [JsonPropertyName("wind_speed")]
        public WindSpeedMeasurements WindSpeed { get; set; } = new WindSpeedMeasurements();

        [JsonPropertyName("wind_direction")]
        public WindDirectionMeasurements WindDirection { get; set; } = new WindDirectionMeasurements();

        [JsonPropertyName("rainfall")]
        public RainfallMeasurements Rainfall { get; set; } = new RainfallMeasurements();
    }
}
