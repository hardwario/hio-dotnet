using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class BLE_Tag_SimulationTests
    {
        [Fact]
        public void Test_BLE_Tag_Simulation_LoweringValue()
        {
            var bletag = new BLE_Tag();
            BaseSimulator.Simulate(bletag);
            Assert.NotEqual(bletag.Rssi, 0.0);

            var bletag1 = new BLE_Tag();
            BaseSimulator.Simulate(bletag1, bletag);
            //check if bletag.Voltage is higher than bletag1.Voltage
            Assert.True(bletag.Voltage > bletag1.Voltage);

            var bletag2 = new BLE_Tag();
            BaseSimulator.Simulate(bletag2, bletag1);
            //check if bletag.Voltage is higher than bletag1.Voltage
            Assert.True(bletag.Voltage > bletag1.Voltage);

        }

    }
}
