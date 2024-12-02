using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Input
{
    public class ChesterInputCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("trigger")]
        public ChesterTriggerStates Trigger { get; set; } = new ChesterTriggerStates();

        [JsonPropertyName("counter")]
        public InputCounterStates Counter { get; set; } = new InputCounterStates();

        [JsonPropertyName("voltage")]
        public InputVoltageMeasurements Voltage { get; set; } = new InputVoltageMeasurements();

        [JsonPropertyName("current")]
        public InputCurrentMeasurements Current { get; set; } = new InputCurrentMeasurements();

        [JsonPropertyName("hygrometer")]
        public Hygrometer? Hygrometer { get; set; }
    }
}
