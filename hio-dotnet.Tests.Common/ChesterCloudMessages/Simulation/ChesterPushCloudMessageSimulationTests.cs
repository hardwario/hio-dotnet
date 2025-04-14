using hio_dotnet.Common.Models.CatalogApps.Push;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterPushCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterPushCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.Equal(message.ButtonStates.Count, 4);

            var message1 = new ChesterPushCloudMessage();
            BaseSimulator.GetSimulatedData(message1, message);
            var button = message.ButtonStates[0];
            var button1 = message1.ButtonStates[0];
            Assert.True(button1.CountClick != button.CountClick);
        }
    }
}
