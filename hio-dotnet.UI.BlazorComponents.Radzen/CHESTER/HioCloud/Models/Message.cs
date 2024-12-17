using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.Radzen.CHESTER.HioCloud.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DeviceId { get; set; } = Guid.NewGuid();
        public Guid SpaceId { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = "Message";
        public string SpaceName { get; set; } = "Space";
        public string DeviceName { get; set; } = "Device";
    }
}
