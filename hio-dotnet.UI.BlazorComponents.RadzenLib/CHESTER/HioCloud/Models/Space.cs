using hio_dotnet.APIs.HioCloud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.CHESTER.HioCloud.Models
{
    public class Space
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Space";

        public List<HioCloudConnector> Connectors { get; set; } = new List<HioCloudConnector>();
        public List<Device> Devices { get; set; } = new List<Device>();

        public List<hio_dotnet.APIs.HioCloud.Models.HioCloudTag> Tags { get; set; } = new List<hio_dotnet.APIs.HioCloud.Models.HioCloudTag>();
    }
}
