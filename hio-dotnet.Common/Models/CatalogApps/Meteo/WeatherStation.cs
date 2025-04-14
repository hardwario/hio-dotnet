using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationMeasurementAttribute(false, 1.0, 7.0, false, false, 0.02, 3)]
        [JsonPropertyName("wind_speed")]
        public MeasurementGroup WindSpeed { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 10.0, 300.0, false, false, 0.02, 3)]
        [JsonPropertyName("wind_direction")]
        public SimpleIntMeasurementGroup WindDirection { get; set; } = new SimpleIntMeasurementGroup();

        [SimulationMeasurementAttribute(false, 10.0, 40.0, false, false, 0.02, 3)]
        [JsonPropertyName("rainfall")]
        public SimpleDoubleMeasurementGroup Rainfall { get; set; } = new SimpleDoubleMeasurementGroup();
    }
}
