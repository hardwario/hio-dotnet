using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class States
    {
        [JsonPropertyName("default")]
        public DefaultState Default { get; set; } = new DefaultState();
    }
}
