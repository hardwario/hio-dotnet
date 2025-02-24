using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Server
{
    public class WSSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid User1Id { get; set; } = Guid.Empty;

        public WebSocket? User1Socket { get; set; }

        public Guid User2Id { get; set; } = Guid.Empty;

        public WebSocket? User2Socket { get; set; }

        public DateTime LastCommunication { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
