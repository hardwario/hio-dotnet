using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Device
    {
        [JsonPropertyName("id")]
        public EntityId Id { get; set; } = new EntityId();

        [JsonPropertyName("createdTime")]
        public long CreatedTime { get; set; } = 0;

        [JsonPropertyName("customerId")]
        public EntityId CustomerId { get; set; } = new EntityId();

        [JsonPropertyName("tenantId")]
        public EntityId TenantId { get; set; } = new EntityId();

        [JsonPropertyName("deviceProfileId")]
        public EntityId DeviceProfileId { get; set; } = new EntityId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("firmwareId")]
        public string? FirmwareId { get; set; }
        [JsonPropertyName("softwareId")]
        public string? SoftwareId { get; set; }

        [JsonPropertyName("externalId")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("additionalInfo")]
        public object? AdditionalInfo { get; set; }

        [JsonPropertyName("deviceData")]
        public DeviceData DeviceData { get; set; } = new DeviceData();

    }
}
