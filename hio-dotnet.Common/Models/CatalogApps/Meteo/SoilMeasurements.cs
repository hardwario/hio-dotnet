using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Meteo
{
    public class SoilMeasurements
    {
        [JsonPropertyName("serial_number")]
        public long SerialNumber { get; set; } = 0;

        [JsonPropertyName("temperature")]
        public Temperature Temperature { get; set; } = new Temperature();

        [JsonPropertyName("moisture")]
        public MeasurementGroup Moisture { get; set; } = new MeasurementGroup();
    }
}
