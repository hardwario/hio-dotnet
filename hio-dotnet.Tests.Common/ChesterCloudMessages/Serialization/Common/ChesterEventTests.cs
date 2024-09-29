using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class ChesterEventTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var chesterEvent = new ChesterDoubleEvent();

            // Act & Assert
            Assert.InRange(chesterEvent.Timestamp, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1);
            Assert.Equal(Defaults.UnknownEventType, chesterEvent.Type);
            Assert.Equal(0.0, chesterEvent.Value);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var chesterEvent = new ChesterDoubleEvent
            {
                Timestamp = timestamp,
                Type = "EventType",
                Value = 123.45
            };

            // Act & Assert
            Assert.Equal(timestamp, chesterEvent.Timestamp);
            Assert.Equal("EventType", chesterEvent.Type);
            Assert.Equal(123.45, chesterEvent.Value);
        }
    }
}
