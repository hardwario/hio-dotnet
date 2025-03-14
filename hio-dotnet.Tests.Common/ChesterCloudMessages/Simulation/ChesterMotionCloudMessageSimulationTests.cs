using hio_dotnet.Common.Models.CatalogApps.Motion;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterMotionCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterMotionCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotNull(message.MotionStates);

            ChesterMotionCloudMessage[] msgs = new ChesterMotionCloudMessage[10];
            msgs[0] = message;

            for (int i = 1; i < 10; i++)
            {
                msgs[i] = new ChesterMotionCloudMessage();
                BaseSimulator.GetSimulatedData(msgs[i], msgs[i - 1]);
            }

            var state = msgs[0].MotionStates?.CountLeft;
            var state1 = msgs[9].MotionStates?.CountLeft;
            Assert.True(state1 > state);
        }
    }
}
