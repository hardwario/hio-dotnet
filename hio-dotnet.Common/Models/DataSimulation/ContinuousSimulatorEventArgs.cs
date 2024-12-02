using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public class ContinuousSimulatorEventArgs
    {
        public Guid SimulatorId { get; set; } = Guid.Empty;
        public string SimulatorName { get; set; } = string.Empty;
        public Guid MessageId { get; set; } = Guid.Empty;
        public string Message { get; set; } = string.Empty;
        public long Timestamp { get; set; } = 0;

    }
}
