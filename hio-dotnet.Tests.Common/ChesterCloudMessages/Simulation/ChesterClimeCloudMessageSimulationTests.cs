using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterClimeCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterClimeCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.W1_Thermometers.Count, 0);
        }
    }
}
