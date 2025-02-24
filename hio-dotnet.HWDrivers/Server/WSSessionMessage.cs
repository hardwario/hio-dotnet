using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Server
{
    public class WSSessionMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SessionId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; } = Guid.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
