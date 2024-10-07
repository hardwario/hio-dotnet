using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class DeviceProfile
    {
        [JsonPropertyName("id")]
        public EntityId Id { get; set; } = new EntityId();
        [JsonPropertyName("createdTime")]
        public long CreatedTime { get; set; } = 0;
        [JsonPropertyName("tenantId")]
        public EntityId TenantId { get; set; } = new EntityId();
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("transportType")]
        public string TransportType { get; set; } = string.Empty;
        [JsonPropertyName("provisionType")]
        public string ProvisionType { get; set; } = string.Empty;
        [JsonPropertyName("defaultRuleChainId")]
        public EntityId DefaultRuleChainId { get; set; } = new EntityId();
        [JsonPropertyName("defaultDashboardId")]
        public EntityId DefaultDashboardId { get; set; } = new EntityId();
        [JsonPropertyName("defaultQueueName")]
        public string DefaultQueueName { get; set; } = string.Empty;
        [JsonPropertyName("provisionDeviceKey")]
        public string ProvisionDeviceKey { get; set; } = string.Empty;
        [JsonPropertyName("firmwareId")]
        public EntityId FirmwareId { get; set; } = new EntityId();
        [JsonPropertyName("softwareId")]
        public EntityId SoftwareId { get; set; } = new EntityId();
        [JsonPropertyName("defaultEdgeRuleChainId")]
        public EntityId DefaultEdgeRuleChainId { get; set; } = new EntityId();
        [JsonPropertyName("version")]
        public long Version { get; set; } = 0;
        [JsonPropertyName("default")]
        public bool Default { get; set; } = false;
        [JsonPropertyName("profileData")]
        public ProfileData ProfileData { get; set; } = new ProfileData();
    }

}
