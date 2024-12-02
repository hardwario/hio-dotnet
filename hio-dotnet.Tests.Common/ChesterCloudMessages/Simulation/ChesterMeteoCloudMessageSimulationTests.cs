using hio_dotnet.Common.Models.CatalogApps.Current;
using hio_dotnet.Common.Models.CatalogApps.Meteo;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterMeteoCloudMessageSimulationTests
    {
        [Fact]
        public void BasicTest()
        {
            // Arrange
            var message = new ChesterMeteoCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.NotEqual(message.Barometer.Pressure.Measurements.Count, 0);
        }
    }
}
