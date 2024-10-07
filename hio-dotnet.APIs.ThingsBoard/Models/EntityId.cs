using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class EntityId
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.Empty;
        [JsonPropertyName("entityType")]
        public string EntityType { get; set; } = string.Empty;
    }
}
