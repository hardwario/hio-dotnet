using hio_dotnet.Common.Models.CatalogApps.Counter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization.Common
{
    public class ChesterCounterStatesTest
    {
        private string testJson = @"{
        ""channel_1_total"": 5,
        ""channel_1_delta"": 2,
        ""channel_2_total"": 5,
        ""channel_2_delta"": 3,
        ""channel_3_total"": 5,
        ""channel_3_delta"": 0,
        ""channel_4_total"": 7,
        ""channel_4_delta"": 0,
        ""channel_5_total"": null,
        ""channel_5_delta"": null,
        ""channel_6_total"": null,
        ""channel_6_delta"": null,
        ""channel_7_total"": null,
        ""channel_7_delta"": null,
        ""channel_8_total"": null,
        ""channel_8_delta"": null
    }";

        [Fact]
        public void ShouldDeserializeCounterCorrectly()
        {
            // Act
            ChesterCounterStates? counter = JsonSerializer.Deserialize<ChesterCounterStates>(testJson);

            // Assert
            Assert.NotNull(counter);
            Assert.Equal(5, counter.Channel1Total);
            Assert.Equal(2, counter.Channel1Delta);
            Assert.Equal(5, counter.Channel2Total);
            Assert.Equal(3, counter.Channel2Delta);
            Assert.Equal(5, counter.Channel3Total);
            Assert.Equal(0, counter.Channel3Delta);
            Assert.Equal(7, counter.Channel4Total);
            Assert.Equal(0, counter.Channel4Delta);
            Assert.Null(counter.Channel5Total);
            Assert.Null(counter.Channel5Delta);
            Assert.Null(counter.Channel6Total);
            Assert.Null(counter.Channel6Delta);
            Assert.Null(counter.Channel7Total);
            Assert.Null(counter.Channel7Delta);
            Assert.Null(counter.Channel8Total);
            Assert.Null(counter.Channel8Delta);
        }

        [Fact]
        public void DefaultValues_ShouldBeNull()
        {
            // Arrange
            var counter = new ChesterCounterStates();

            // Assert
            Assert.Null(counter.Channel1Total);
            Assert.Null(counter.Channel1Delta);
            Assert.Null(counter.Channel2Total);
            Assert.Null(counter.Channel2Delta);
            Assert.Null(counter.Channel3Total);
            Assert.Null(counter.Channel3Delta);
            Assert.Null(counter.Channel4Total);
            Assert.Null(counter.Channel4Delta);
            Assert.Null(counter.Channel5Total);
            Assert.Null(counter.Channel5Delta);
            Assert.Null(counter.Channel6Total);
            Assert.Null(counter.Channel6Delta);
            Assert.Null(counter.Channel7Total);
            Assert.Null(counter.Channel7Delta);
            Assert.Null(counter.Channel8Total);
            Assert.Null(counter.Channel8Delta);
        }
    }
}
