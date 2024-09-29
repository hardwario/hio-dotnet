using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Radon
{
    public class RadonProbeData
    {
        [JsonPropertyName("chamber_humidity")]
        public ChamberHumidityMeasurements ChamberHumidity { get; set; } = new ChamberHumidityMeasurements();
        
        [JsonPropertyName("chamber_temperature")]
        public ChamberTemperatureMeasurements ChamberTemperature { get; set; } = new ChamberTemperatureMeasurements();

        [JsonPropertyName("concentration_daily")]
        public ConcentrationMeasurements ConcentrationDaily { get; set; } = new ConcentrationMeasurements();

        [JsonPropertyName("concentration_hourly")]
        public ConcentrationMeasurements ConcentrationHourly { get; set; } = new ConcentrationMeasurements();
    }
}
