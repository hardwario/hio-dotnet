using hio_dotnet.Common.Models;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Simulation
{
    public class ChesterCommonCloudMessageSimulationTests
    {
        [Fact]
        public void CatalogAppsSimulatedNames()
        {
            var message_clime = new hio_dotnet.Common.Models.CatalogApps.Clime.ChesterClimeCloudMessage();
            BaseSimulator.GetSimulatedData(message_clime);
            Assert.Equal(message_clime.Attribute.FwName, "CHESTER Clime");

            var message_boiler = new hio_dotnet.Common.Models.CatalogApps.Boiler.ChesterBoilerCloudMessage();
            BaseSimulator.GetSimulatedData(message_boiler);
            Assert.Equal(message_boiler.Attribute.FwName, "CHESTER Boiler");

            var message_iaq = new hio_dotnet.Common.Models.CatalogApps.ClimeIAQ.ChesterClimeIAQCloudMessage();
            BaseSimulator.GetSimulatedData(message_iaq);
            Assert.Equal(message_iaq.Attribute.FwName, "CHESTER Clime IAQ");

            var message_control = new hio_dotnet.Common.Models.CatalogApps.Control.ChesterControlCloudMessage();
            BaseSimulator.GetSimulatedData(message_control);
            Assert.Equal(message_control.Attribute.FwName, "CHESTER Control");

            var message_current = new hio_dotnet.Common.Models.CatalogApps.Current.ChesterCurrentCloudMessage();
            BaseSimulator.GetSimulatedData(message_current);
            Assert.Equal(message_current.Attribute.FwName, "CHESTER Current");

            var message_counter = new hio_dotnet.Common.Models.CatalogApps.Counter.ChesterCounterCloudMessage();
            BaseSimulator.GetSimulatedData(message_counter);
            Assert.Equal(message_counter.Attribute.FwName, "CHESTER Counter");

            var message_input = new hio_dotnet.Common.Models.CatalogApps.Input.ChesterInputCloudMessage();
            BaseSimulator.GetSimulatedData(message_input);
            Assert.Equal(message_input.Attribute.FwName, "CHESTER Input");

            var message_meteo = new hio_dotnet.Common.Models.CatalogApps.Meteo.ChesterMeteoCloudMessage();
            BaseSimulator.GetSimulatedData(message_meteo);
            Assert.Equal(message_meteo.Attribute.FwName, "CHESTER Meteo");

            var message_push = new hio_dotnet.Common.Models.CatalogApps.Push.ChesterPushCloudMessage();
            BaseSimulator.GetSimulatedData(message_push);
            Assert.Equal(message_push.Attribute.FwName, "CHESTER Push");

            var message_radon = new hio_dotnet.Common.Models.CatalogApps.Radon.ChesterRadonCloudMessage();
            BaseSimulator.GetSimulatedData(message_radon);
            Assert.Equal(message_radon.Attribute.FwName, "CHESTER Radon");

            var message_range = new hio_dotnet.Common.Models.CatalogApps.Range.ChesterRangeCloudMessage();
            BaseSimulator.GetSimulatedData(message_range);
            Assert.Equal(message_range.Attribute.FwName, "CHESTER Range");

            var message_wmbus = new hio_dotnet.Common.Models.CatalogApps.wMBus.ChesterWMBusCloudMessage();
            BaseSimulator.GetSimulatedData(message_wmbus);
            Assert.Equal(message_wmbus.Attribute.FwName, "CHESTER wM-Bus");

        }

        [Fact]
        public void CheckBasicStaticParams()
        {
            // Arrange
            var message = new ChesterCommonCloudMessage();
            BaseSimulator.GetSimulatedData(message);
            Assert.Equal(message.Attribute.ProductName, "CHESTER-M");
            Assert.Equal(message.Attribute.VendorName, "HARDWARIO");
            Assert.Equal(message.Attribute.FwVersion, "3.4.0");
            Assert.Equal(message.Attribute.HwVariant, "CGLS");
            Assert.Equal(message.Attribute.HwRevision, "R3.2");
        }
    }
}
