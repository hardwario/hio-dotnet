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

            ChesterScaleCloudMessage[] msgs = new ChesterScaleCloudMessage[10];
            msgs[0] = message;

            for (int i = 1; i < 10; i++)
            {
                msgs[i] = new ChesterScaleCloudMessage();
                BaseSimulator.GetSimulatedData(msgs[i], msgs[i-1]);
            }

            var weight = msgs[0].Weights[0];
            var weight1 = msgs[9].Weights[0];
            Assert.True(weight1.A2 > weight.A2);
        }
    }
}
