using hio_dotnet.Common.Models.CatalogApps.Current;
using hio_dotnet.Common.Models.CatalogApps.Range;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterCurrentCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterCurrentCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.AnalogChannels.Count, 0);
        }
    }
}
