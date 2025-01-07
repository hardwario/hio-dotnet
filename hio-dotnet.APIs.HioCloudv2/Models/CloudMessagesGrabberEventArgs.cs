using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud.Models
{
    public class CloudMessagesGrabberEventArgs
    {
        public Guid GrabberId { get; set; } = Guid.Empty;

        public HioCloudMessage? Message { get; set; }
    }
}
