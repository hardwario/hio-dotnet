using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Dust
{
    public class DustSensorSps30Data
    {
        [SimulationMeasurementAttribute(false, 76000.0, 77100.0, false, false, 0.02, 3)]
        [JsonPropertyName("num_pm0_5")]
        public MeasurementGroup Num_PM_0_5 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 97000.0, 98200.0, false, false, 0.02, 3)]
        [JsonPropertyName("num_pm1")]
        public MeasurementGroup Num_PM1 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 103000.0, 105000.0, false, false, 0.02, 3)]
        [JsonPropertyName("num_pm2_5")]
        public MeasurementGroup Num_PM_2_5 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 104000.0, 106300.0, false, false, 0.02, 3)]
        [JsonPropertyName("num_pm4")]
        public MeasurementGroup Num_PM4 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 104200.0, 106500.0, false, false, 0.02, 3)]
        [JsonPropertyName("num_pm10")]
        public MeasurementGroup Num_PM_10 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 12600.0, 13200.0, false, false, 0.02, 3)]
        [JsonPropertyName("pm1")]
        public MeasurementGroup PM_1 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 18400.0, 19500.0, false, false, 0.02, 3)]
        [JsonPropertyName("pm2_5")]
        public MeasurementGroup PM_2_5 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 22300.0, 24000.0, false, false, 0.02, 3)]
        [JsonPropertyName("pm4")]
        public MeasurementGroup PM_4 { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 24300.0, 26300.0, false, false, 0.02, 3)]
        [JsonPropertyName("pm10")]
        public MeasurementGroup PM_10 { get; set; } = new MeasurementGroup();
    }
}
