using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models.Dashboards
{
    public class DefaultState
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; } = string.Empty;

        [JsonPropertyName("root")]
        public bool Root { get; set; } = true;

        [JsonPropertyName("layouts")]
        public Layouts Layouts { get; set; } = new Layouts();
    }
}
