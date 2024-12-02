using hio_dotnet.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public class SimulatedMessage<T> where T : class
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long Timestamp { get; set; } = TimeHelpers.DateTimeToUnixTimestamp(DateTime.UtcNow);
        public T? Message { get; set; }
    }
}
