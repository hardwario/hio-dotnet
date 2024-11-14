using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Push
{
    public class ChesterPushCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationMeasurementAttribute(false, numberOfInsideItems:4)]
        [JsonPropertyName("buttons")]
        public List<PushButtonsStates>? ButtonStates { get; set; }

        [SimulationAttribute(false)]
        [JsonPropertyName("button_x")]
        public PushButtonsStates? ButtonX { get; set; }

        [SimulationAttribute(false)]
        [JsonPropertyName("button_1")]
        public PushButtonsStates? Button_1 { get; set; }

        [SimulationAttribute(false)]
        [JsonPropertyName("button_2")]
        public PushButtonsStates? Button_2 { get; set; }

        [SimulationAttribute(false)]
        [JsonPropertyName("button_3")]
        public PushButtonsStates? Button_3 { get; set; }

        [SimulationAttribute(false)]
        [JsonPropertyName("button_4")]
        public PushButtonsStates? Button_4 { get; set; }

    }
}
