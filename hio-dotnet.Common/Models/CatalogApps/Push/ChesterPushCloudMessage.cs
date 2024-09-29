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
        [JsonPropertyName("button_x")]
        public PushButtonsStates ButtonX { get; set; } = new PushButtonsStates();

        [JsonPropertyName("button_1")]
        public PushButtonsStates Button_1 { get; set; } = new PushButtonsStates();

        [JsonPropertyName("button_2")]
        public PushButtonsStates Button_2 { get; set; } = new PushButtonsStates();

        [JsonPropertyName("button_3")]
        public PushButtonsStates Button_3 { get; set; } = new PushButtonsStates();

        [JsonPropertyName("button_4")]
        public PushButtonsStates Button_4 { get; set; } = new PushButtonsStates();

    }
}
