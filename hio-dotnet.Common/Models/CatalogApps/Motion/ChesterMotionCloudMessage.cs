using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps.Motion
{
    public class ChesterMotionCloudMessage : ChesterCommonCloudMessage
    {
        [SimulationAttribute(false)]
        public MotionStates? MotionStates { get; set; }
    }
}
