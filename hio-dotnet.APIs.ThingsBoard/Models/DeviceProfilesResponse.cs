using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class DeviceProfilesResponse
    {
        [JsonPropertyName("data")]
        public List<DeviceProfile> Data { get; set; } = new List<DeviceProfile>();
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; } = 0;
        [JsonPropertyName("totalElements")]
        public long TotalElements { get; set; } = 0;
        [JsonPropertyName("hasNext")]
        public bool HasNext { get; set; } = false;
    }
}
