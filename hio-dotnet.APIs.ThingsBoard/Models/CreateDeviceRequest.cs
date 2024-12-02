using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class CreateDeviceRequest
    {
        [JsonPropertyName("customerId")]
        public EntityId CustomerId { get; set; } = new EntityId();

        [JsonPropertyName("deviceProfileId")]
        public EntityId DeviceProfileId { get; set; } = new EntityId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }
}
