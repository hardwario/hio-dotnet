using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Boiler
{
    public class BoilerLineStatus
    {
        [JsonPropertyName("line_present")]
        public bool LinePresent { get; set; } = false;
    }
}
