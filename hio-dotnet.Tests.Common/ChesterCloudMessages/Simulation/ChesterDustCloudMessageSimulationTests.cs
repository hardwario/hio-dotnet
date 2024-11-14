using hio_dotnet.Common.Models.CatalogApps.Dust;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterDustCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterDustCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.DustSensor.Num_PM1.Measurements.Count, 0);

            var message1 = new ChesterDustCloudMessage();
            BaseSimulator.GetSimulatedData(message1, message);
            Assert.NotEqual(message1.DustSensor.Num_PM1.Measurements.Count, 0);
            Assert.True(message1.DustSensor.Num_PM1.Measurements[0].Avg != message.DustSensor.Num_PM1.Measurements[0].Avg);
        }
    }
}
