using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using hio_dotnet.Common.Models.Common;

namespace hio_dotnet.Common.Models.CatalogApps.ClimeIAQ
{
    public class Pressure
    {
        [JsonPropertyName("measurements")]
        public List<Measurement> Measurements { get; set; } = new List<Measurement>();
    }
}
