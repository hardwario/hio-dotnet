using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class Dashboard
    {
        [JsonPropertyName("id")]
        public EntityId Id { get; set; } = new EntityId();

        [JsonPropertyName("createdTime")]
        public long CreatedTime { get; set; } = 0;

        [JsonPropertyName("customerId")]
        public EntityId CustomerId { get; set; } = new EntityId();

        [JsonPropertyName("tenantId")]
        public EntityId TenantId { get; set; } = new EntityId();

        [JsonPropertyName("title")]
        public string? Title { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public object? Image { get; set; } = null;

        [JsonPropertyName("mobileHide")]
        public bool MobileHide { get; set; } = false;

        [JsonPropertyName("mobileOrder")]
        public object? MobileOrder { get; set; } = null;

        [JsonPropertyName("configuration")]
        public Configuration Configuration { get; set; } = new Configuration();

        [JsonPropertyName("name")]
        public string? Name { get; set; } = string.Empty;
    }
}
