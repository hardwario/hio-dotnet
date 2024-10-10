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
        [SimulationMeasurementAttribute(false, 25.0, 35.0, true, false, 0.02)]
        [JsonPropertyName("temperature")]
        public Temperature Temperature { get; set; } = new Temperature();

        [SimulationMeasurementAttribute(false, 0.0, 100.0, true, false, 0.02)]
        [JsonPropertyName("humidity")]
        public MeasurementGroup Humidity { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 500.0, 1000.0, true, false, 0.02)]
        [JsonPropertyName("illuminance")]
        public MeasurementGroup Illuminance { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 50.0, 250.0, true, false, 0.02)]
        [JsonPropertyName("altitude")]
        public MeasurementGroup Altitude { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 1000.0, 5000.0, true, false, 0.02)]
        [JsonPropertyName("pressure")]
        public MeasurementGroup Pressure { get; set; } = new MeasurementGroup();

        [SimulationMeasurementAttribute(false, 100.0, 1000.0, true, false, 0.02)]
        [JsonPropertyName("co2_conc")]
        public MeasurementGroup CO2_Concentration { get; set; } = new MeasurementGroup();

        [JsonPropertyName("motion_count")]
        public MotionCount MotionCount { get; set; } = new MotionCount();

        [JsonPropertyName("press_count")]
        public PressCount PressCount { get; set; } = new PressCount();
    }
}
