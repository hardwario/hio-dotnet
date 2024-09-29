using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class ChesterSystemTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var chesterSystem = new ChesterSystem();

            // Act & Assert
            Assert.Equal(0, chesterSystem.Uptime);
            Assert.Equal(0.0, chesterSystem.VoltageRest);
            Assert.Equal(0.0, chesterSystem.VoltageLoad);
            Assert.Equal(0, chesterSystem.CurrentLoad);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var chesterSystem = new ChesterSystem
            {
                Uptime = 123456,
                VoltageRest = 3.7,
                VoltageLoad = 3.5,
                CurrentLoad = 150
            };

            // Act & Assert
            Assert.Equal(123456, chesterSystem.Uptime);
            Assert.Equal(3.7, chesterSystem.VoltageRest);
            Assert.Equal(3.5, chesterSystem.VoltageLoad);
            Assert.Equal(150, chesterSystem.CurrentLoad);
        }
    }
}
