using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class MeasurementTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var measurement = new Measurement();

            // Act & Assert
            Assert.InRange(measurement.Timestamp, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1);
            Assert.Equal(0.0, measurement.Min);
            Assert.Equal(0.0, measurement.Max);
            Assert.Equal(0.0, measurement.Avg);
            Assert.Equal(0.0, measurement.Mdn);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var measurement = new Measurement
            {
                Timestamp = timestamp,
                Min = 1.1,
                Max = 5.5,
                Avg = 3.3,
                Mdn = 4.4
            };

            // Act & Assert
            Assert.Equal(timestamp, measurement.Timestamp);
            Assert.Equal(1.1, measurement.Min);
            Assert.Equal(5.5, measurement.Max);
            Assert.Equal(3.3, measurement.Avg);
            Assert.Equal(4.4, measurement.Mdn);
        }
    }
}
