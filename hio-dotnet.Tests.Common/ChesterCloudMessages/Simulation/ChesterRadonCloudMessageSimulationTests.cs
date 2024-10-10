using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.CatalogApps.Radon;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterRadonCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterRadonCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.RadonProbe.ChamberTemperature.Measurements.Count, 0);
        }
    }
}
