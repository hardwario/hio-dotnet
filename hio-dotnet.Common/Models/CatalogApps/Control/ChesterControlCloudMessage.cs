using hio_dotnet.Common.Models.CatalogApps.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Control
{
    public class ChesterControlCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("counter")]
        public List<InputCounterStates> Counter { get; set; } = new List<InputCounterStates>();

        [JsonPropertyName("current")]
        public List<ControlCurrentMeasurements> Current { get; set; } = new List<ControlCurrentMeasurements>();

        [JsonPropertyName("voltage")]
        public List<InputVoltageMeasurements> Voltage { get; set; } = new List<InputVoltageMeasurements>();

        [JsonPropertyName("trigger")]
        public List<ChesterTriggerStates> Trigger { get; set; } = new List<ChesterTriggerStates>();

    }
}
