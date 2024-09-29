using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class NetworkParameterTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var networkParameter = new NetworkParameter();

            // Act & Assert
            Assert.Equal(0, networkParameter.Eest);
            Assert.Equal(0, networkParameter.Ecl);
            Assert.Equal(-120, networkParameter.Rsrp);
            Assert.Equal(-20, networkParameter.Rsrq);
            Assert.Equal(0, networkParameter.Snr);
            Assert.Equal(0, networkParameter.Plmn);
            Assert.Equal(0, networkParameter.Cid);
            Assert.Equal(0, networkParameter.Band);
            Assert.Equal(0, networkParameter.Earfcn);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var networkParameter = new NetworkParameter
            {
                Eest = 1,
                Ecl = 2,
                Rsrp = -110,
                Rsrq = -15,
                Snr = 5,
                Plmn = 310,
                Cid = 12345,
                Band = 12,
                Earfcn = 100
            };

            // Act & Assert
            Assert.Equal(1, networkParameter.Eest);
            Assert.Equal(2, networkParameter.Ecl);
            Assert.Equal(-110, networkParameter.Rsrp);
            Assert.Equal(-15, networkParameter.Rsrq);
            Assert.Equal(5, networkParameter.Snr);
            Assert.Equal(310, networkParameter.Plmn);
            Assert.Equal(12345, networkParameter.Cid);
            Assert.Equal(12, networkParameter.Band);
            Assert.Equal(100, networkParameter.Earfcn);
        }
    }
}
