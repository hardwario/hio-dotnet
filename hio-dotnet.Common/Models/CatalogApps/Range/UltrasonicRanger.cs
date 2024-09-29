using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Range
{
    public class UltrasonicRanger
    {
        [JsonPropertyName("distance")]
        public DistanceMeasurements Distance { get; set; } = new DistanceMeasurements();
    }
}
