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

            var message2 = new ChesterScaleCloudMessage();
            BaseSimulator.GetSimulatedData(message2, message1);

            var message3 = new ChesterScaleCloudMessage();
            BaseSimulator.GetSimulatedData(message3, message2);

            var message4 = new ChesterScaleCloudMessage();
            BaseSimulator.GetSimulatedData(message4, message3);

            var weight = message.Weights[0];
            var weight4 = message4.Weights[0];
            Assert.True(weight4.A2 > weight.A2);
        }
    }
}
