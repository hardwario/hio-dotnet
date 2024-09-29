using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Meteo
{
    public class RainfallMeasurements
    {
        [JsonPropertyName("measurements")]
        public List<SimpleTimeDoubleMeasurement> Measurements { get; set; } = new List<SimpleTimeDoubleMeasurement>();
    }
}
