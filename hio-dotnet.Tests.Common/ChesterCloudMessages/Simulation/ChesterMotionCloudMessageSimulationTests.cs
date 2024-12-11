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

            var message1 = new ChesterMotionCloudMessage();
            BaseSimulator.GetSimulatedData(message1, message);

            var message2 = new ChesterMotionCloudMessage();
            BaseSimulator.GetSimulatedData(message2, message1);

            var message3 = new ChesterMotionCloudMessage();
            BaseSimulator.GetSimulatedData(message3, message2);

            var state = message.MotionStates?.CountLeft;
            var state3 = message3.MotionStates?.CountLeft;
            Assert.True(state3 > state);
        }
    }
}
