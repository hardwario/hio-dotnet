using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class BackupTests
    {
        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange
            var backup = new Backup();

            // Act & Assert
            Assert.Equal(0.0, backup.LineVoltage);
            Assert.Equal(0.0, backup.BattVoltage);
            Assert.Null(backup.State);
            Assert.Empty(backup.Events);
        }

        [Fact]
        public void SetValues_ShouldBeAssignedCorrectly()
        {
            // Arrange
            var events = new List<ChesterEvent>
            {
                new ChesterEvent { Timestamp = 1627849200, Type = "Event1", Value = 1.1 },
                new ChesterEvent { Timestamp = 1627849300, Type = "Event2", Value = 2.2 }
            };

            var backup = new Backup
            {
                LineVoltage = 120.5,
                BattVoltage = 12.7,
                State = "Active",
                Events = events
            };

            // Act & Assert
            Assert.Equal(120.5, backup.LineVoltage);
            Assert.Equal(12.7, backup.BattVoltage);
            Assert.Equal("Active", backup.State);
            Assert.Equal(events, backup.Events);
        }

        [Fact]
        public void Events_ShouldBeEmptyByDefault()
        {
            // Arrange
            var backup = new Backup();

            // Act & Assert
            Assert.Empty(backup.Events);
        }

        [Fact]
        public void AddEvent_ShouldAddEventToList()
        {
            // Arrange
            var backup = new Backup();
            var chesterEvent = new ChesterEvent { Timestamp = 1627849200, Type = "Event1", Value = 1.1 };

            // Act
            backup.Events.Add(chesterEvent);

            // Assert
            Assert.Single(backup.Events);
            Assert.Equal(chesterEvent, backup.Events[0]);
        }
    }
}
