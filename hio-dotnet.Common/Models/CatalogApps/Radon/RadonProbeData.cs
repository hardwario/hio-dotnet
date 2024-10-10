using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationMeasurementAttribute(false, 0.0, 100.0, true, false, 0.02)]
        [JsonPropertyName("chamber_humidity")]
        public MeasurementGroup ChamberHumidity { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 25.0, 35.0, true, false, 0.02)]
        [JsonPropertyName("chamber_temperature")]
        public MeasurementGroup ChamberTemperature { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 20.0, 50.0, true, false, 0.02, 5)]
        [JsonPropertyName("concentration_daily")]
        public SimpleDoubleMeasurementGroup ConcentrationDaily { get; set; } = new SimpleDoubleMeasurementGroup();

        [SimulationMeasurementAttribute(false, 10.0, 60.0, true, false, 0.02, 5)]
        [JsonPropertyName("concentration_hourly")]
        public SimpleDoubleMeasurementGroup ConcentrationHourly { get; set; } = new SimpleDoubleMeasurementGroup();
    }
}
