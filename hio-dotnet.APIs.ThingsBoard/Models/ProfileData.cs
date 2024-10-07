using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class ProfileData
    {
        [JsonPropertyName("configuration")]
        public Configuration Configuration { get; set; } = new Configuration();
        [JsonPropertyName("transportConfiguration")]
        public TransportConfiguration TransportConfiguration { get; set; } = new TransportConfiguration();
        [JsonPropertyName("provisionConfiguration")]
        public ProvisionConfiguration ProvisionConfiguration { get; set; } = new ProvisionConfiguration();
        [JsonPropertyName("alarms")]
        public List<Alarm> Alarms { get; set; } = new List<Alarm>();
    }
}
