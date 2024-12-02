using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.wMBus
{
    public class ChesterWMBusCloudMessage : ChesterCommonCloudMessage
    {
        [JsonPropertyName("wmbus")]
        public WMBusData WMBus { get; set; } = new WMBusData();
    }
}
