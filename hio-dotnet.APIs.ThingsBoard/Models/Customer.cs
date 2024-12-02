using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Customer
    {
        [JsonPropertyName("id")]
        public EntityId Id { get; set; } = new EntityId();
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        [JsonPropertyName("address2")]
        public string Address2 { get; set; } = string.Empty;
        [JsonPropertyName("zip")]
        public string Zip { get; set; } = string.Empty;
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("tenantId")]
        public EntityId TenantId { get; set; } = new EntityId();
        [JsonPropertyName("version")]
        public long Version { get; set; } = 0;
        [JsonPropertyName("additionalInfo")]
        public object? AdditionalInfo { get; set; } = null;
    }
}
