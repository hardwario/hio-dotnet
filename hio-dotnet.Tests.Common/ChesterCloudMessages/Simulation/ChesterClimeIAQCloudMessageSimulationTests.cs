using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.CatalogApps.ClimeIAQ;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterClimeIAQCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterClimeIAQCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.W1_Thermometers.Count, 0);
        }
    }
}
