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
    public class ChesterMeteoCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("weather_station")]
        public WeatherStation WeatherStation { get; set; } = new WeatherStation();

        [SimulationAttribute(false)]
        [JsonPropertyName("barometer")]
        public BarometerMeasurements Barometer { get; set; } = new BarometerMeasurements();

        [SimulationAttribute(false)]
        [JsonPropertyName("hygrometer")]
        public Hygrometer Hygrometer { get; set; } = new Hygrometer();

        [SimulationMeasurementAttribute(false, 25, 35, true, false, 0, 3)]
        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer> W1Thermometers { get; set; } = new List<W1_Thermometer>();

        [SimulationMeasurementAttribute(false, numberOfInsideItems: 3)]
        [JsonPropertyName("soil_sensors")]
        public List<SoilMeasurements> SoilSensors { get; set; } = new List<SoilMeasurements>();

    }
}
