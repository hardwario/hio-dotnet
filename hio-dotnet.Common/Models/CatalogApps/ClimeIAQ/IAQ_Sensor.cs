using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
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
        [SimulationMeasurementAttribute(false, 25.0, 35.0, false, false, 0.05)]
        [JsonPropertyName("temperature")]
        public Temperature Temperature { get; set; } = new Temperature();

        [SimulationMeasurementAttribute(false, 10.0, 80.0, false, false, 0.05)]
        [JsonPropertyName("humidity")]
        public MeasurementGroup Humidity { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 500.0, 1000.0, false, false, 0.02)]
        [JsonPropertyName("illuminance")]
        public MeasurementGroup Illuminance { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 50.0, 70.0, false, false, 0.01)]
        [JsonPropertyName("altitude")]
        public MeasurementGroup Altitude { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 1000.0, 2000.0, false, false, 0.05)]
        [JsonPropertyName("pressure")]
        public MeasurementGroup Pressure { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 100.0, 200.0, false, false, 0.07)]
        [JsonPropertyName("co2_conc")]
        public MeasurementGroup CO2_Concentration { get; set; } = new MeasurementGroup();

        [JsonPropertyName("motion_count")]
        public MotionCount MotionCount { get; set; } = new MotionCount();

        [JsonPropertyName("press_count")]
        public PressCount PressCount { get; set; } = new PressCount();
    }
}
