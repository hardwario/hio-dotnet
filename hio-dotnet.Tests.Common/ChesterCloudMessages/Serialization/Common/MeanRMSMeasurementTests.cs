using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class MeanRMSMeasurementTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var measurement = new MeanRMSMeasurement();

            // Act & Assert
            Assert.InRange(measurement.Timestamp, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1);
            Assert.Equal(0.0, measurement.MeanMin);
            Assert.Equal(0.0, measurement.MeanMax);
            Assert.Equal(0.0, measurement.MeanAvg);
            Assert.Equal(0.0, measurement.MeanMdn);
            Assert.Equal(0.0, measurement.RmsMin);
            Assert.Equal(0.0, measurement.RmsMax);
            Assert.Equal(0.0, measurement.RmsAvg);
            Assert.Equal(0.0, measurement.RmsMdn);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var measurement = new MeanRMSMeasurement
            {
                Timestamp = timestamp,
                MeanMin = 1.1,
                MeanMax = 5.5,
                MeanAvg = 3.3,
                MeanMdn = 4.4,
                RmsMin = 2.2,
                RmsMax = 6.6,
                RmsAvg = 4.4,
                RmsMdn = 5.5
            };

            // Act & Assert
            Assert.Equal(timestamp, measurement.Timestamp);
            Assert.Equal(1.1, measurement.MeanMin);
            Assert.Equal(5.5, measurement.MeanMax);
            Assert.Equal(3.3, measurement.MeanAvg);
            Assert.Equal(4.4, measurement.MeanMdn);
            Assert.Equal(2.2, measurement.RmsMin);
            Assert.Equal(6.6, measurement.RmsMax);
            Assert.Equal(4.4, measurement.RmsAvg);
            Assert.Equal(5.5, measurement.RmsMdn);
        }
    }
}
