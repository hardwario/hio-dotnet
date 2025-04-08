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
    public class ChesterClimeIAQCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationAttribute(false)]
        [JsonPropertyName("iaq_sensor")]
        public IAQ_Sensor IAQ_Sensor { get; set; } = new IAQ_Sensor();

        [SimulationAttribute(false)]
        [JsonPropertyName("hygrometer")]
        public Hygrometer? Hygrometer { get; set; }

        [SimulationMeasurementAttribute(false, 25, 35, false, false, 0, 3)]
        [JsonPropertyName("w1_thermometers")]
        public List<W1_Thermometer>? W1_Thermometers { get; set; }

        [SimulationMeasurementAttribute(false, 25, 35, false, false, 0, 5)]
        [JsonPropertyName("rtd_thermometer")]
        public List<RTD_Thermometer>? RTD_Thermometers { get; set; }

        [SimulationMeasurementAttribute(false, numberOfInsideItems: 5)]
        [JsonPropertyName("ble_tags")]
        public List<BLE_Tag>? BLE_Tags { get; set; }
    }
}
