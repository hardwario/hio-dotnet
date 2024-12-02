using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class NetworkTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var network = new Network();

            // Act & Assert
            Assert.Equal(0, network.Imei);
            Assert.Equal(0, network.Imsi);
            Assert.NotNull(network.Parameter);
            Assert.Equal(0, network.Parameter.Eest);
            Assert.Equal(0, network.Parameter.Ecl);
            Assert.Equal(-120, network.Parameter.Rsrp);
            Assert.Equal(-20, network.Parameter.Rsrq);
            Assert.Equal(0, network.Parameter.Snr);
            Assert.Equal(0, network.Parameter.Plmn);
            Assert.Equal(0, network.Parameter.Cid);
            Assert.Equal(0, network.Parameter.Band);
            Assert.Equal(0, network.Parameter.Earfcn);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var networkParameter = new NetworkParameter
            {
                Eest = 1,
                Ecl = 2,
                Rsrp = 3,
                Rsrq = 4,
                Snr = 5,
                Plmn = 6,
                Cid = 7,
                Band = 8,
                Earfcn = 9
            };

            var network = new Network
            {
                Imei = 123456789012345,
                Imsi = 987654321098765,
                Parameter = networkParameter
            };

            // Act & Assert
            Assert.Equal(123456789012345, network.Imei);
            Assert.Equal(987654321098765, network.Imsi);
            Assert.Equal(networkParameter, network.Parameter);
            Assert.Equal(1, network.Parameter.Eest);
            Assert.Equal(2, network.Parameter.Ecl);
            Assert.Equal(3, network.Parameter.Rsrp);
            Assert.Equal(4, network.Parameter.Rsrq);
            Assert.Equal(5, network.Parameter.Snr);
            Assert.Equal(6, network.Parameter.Plmn);
            Assert.Equal(7, network.Parameter.Cid);
            Assert.Equal(8, network.Parameter.Band);
            Assert.Equal(9, network.Parameter.Earfcn);
        }
    }
}
