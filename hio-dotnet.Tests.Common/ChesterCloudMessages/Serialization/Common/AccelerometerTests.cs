using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hio_dotnet.Common.Models.Common;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class AccelerometerTests
    {
        [Fact]
        public void DefaultValues_ShouldBeZero()
        {
            // Arrange
            var accelerometer = new Accelerometer();

            // Act & Assert
            Assert.Equal(0.0, accelerometer.AccelerationX);
            Assert.Equal(0.0, accelerometer.AccelerationY);
            Assert.Equal(0.0, accelerometer.AccelerationZ);
            Assert.Equal(0, accelerometer.Orientation);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var accelerometer = new Accelerometer
            {
                AccelerationX = 1.1,
                AccelerationY = 2.2,
                AccelerationZ = 3.3,
                Orientation = 1
            };

            // Act & Assert
            Assert.Equal(1.1, accelerometer.AccelerationX);
            Assert.Equal(2.2, accelerometer.AccelerationY);
            Assert.Equal(3.3, accelerometer.AccelerationZ);
            Assert.Equal(1, accelerometer.Orientation);
        }
    }
}
