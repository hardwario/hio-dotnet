using hio_dotnet.Common.Models.CatalogApps.Scale;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterScaleCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterScaleCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.Weights.Count, 0);

            var message1 = new ChesterScaleCloudMessage();
            BaseSimulator.GetSimulatedData(message1, message);
            var weight = message.Weights[0];
            var weight1 = message1.Weights[0];
            Assert.True(weight1.A2 > weight.A2);
        }
    }
}
