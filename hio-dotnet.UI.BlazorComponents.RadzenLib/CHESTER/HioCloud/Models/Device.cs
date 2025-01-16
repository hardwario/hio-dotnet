using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.CHESTER.HioCloud.Models
{
    public class Device
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SpaceId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Device";
        public string SpaceName { get; set; } = "Device";
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
