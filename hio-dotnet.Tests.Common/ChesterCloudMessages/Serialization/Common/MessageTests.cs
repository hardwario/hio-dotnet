using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class MessageTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var message = new Message();

            // Act & Assert
            Assert.Equal(0, message.Version);
            Assert.Equal(0, message.Sequence);
            Assert.InRange(message.Timestamp, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 1, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 1);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var message = new Message
            {
                Version = 1,
                Sequence = 42,
                Timestamp = timestamp
            };

            // Act & Assert
            Assert.Equal(1, message.Version);
            Assert.Equal(42, message.Sequence);
            Assert.Equal(timestamp, message.Timestamp);
        }
    }
}
