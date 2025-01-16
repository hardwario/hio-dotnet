using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class ListableResponse
    {
        [JsonPropertyName("totalPages")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalPages { get; set; }

        [JsonPropertyName("totalElements")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalElements { get; set; }

        [JsonPropertyName("hasNext")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? hasNext { get; set; }

    }
}
