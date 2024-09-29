using hio_dotnet.Common.Models.CatalogApps.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterControlTests
    {
        private string testJson = @"{
        ""accelerometer"": {
            ""accel_x"": 0,
            ""accel_y"": 0,
            ""accel_z"": 9.57,
            ""orientation"": 2
        },
        ""counter"": [
            {
                ""channel"": 2,
                ""delta"": 6,
                ""measurements"": [
                    {
                        ""timestamp"": 1705328341,
                        ""value"": 7
                    }
                ],
                ""value"": 7
            }
        ],
        ""current"": [
            {
                ""channel"": 4,
                ""measurements"": [
                    {
                        ""avg"": 2,
                        ""max"": 5.03,
                        ""mdn"": 0,
                        ""min"": 0,
                        ""timestamp"": 1705328341
                    }
                ]
            }
        ],
        ""message"": {
            ""sequence"": 1,
            ""timestamp"": 1705328341,
            ""version"": 1
        },
        ""network"": {
            ""imei"": 351358816128174,
            ""imsi"": 901288910100358
        },
        ""thermometer"": {
            ""temperature"": 22.75
        },
        ""trigger"": [
            {
                ""channel"": 1,
                ""events"": [
                    {
                        ""timestamp"": 1705328233,
                        ""type"": ""activated""
                    },
                    {
                        ""timestamp"": 1705328233,
                        ""type"": ""deactivated""
                    },
                    {
                        ""timestamp"": 1705328233,
                        ""type"": ""activated""
                    },
                    {
                        ""timestamp"": 1705328233,
                        ""type"": ""deactivated""
                    },
                    {
                        ""timestamp"": 1705328234,
                        ""type"": ""activated""
                    },
                    {
                        ""timestamp"": 1705328234,
                        ""type"": ""deactivated""
                    },
                    {
                        ""timestamp"": 1705328234,
                        ""type"": ""activated""
                    },
                    {
                        ""timestamp"": 1705328235,
                        ""type"": ""deactivated""
                    }
                ],
                ""state"": ""inactive""
            }
        ],
        ""voltage"": [
            {
                ""channel"": 3,
                ""measurements"": [
                    {
                        ""avg"": 0.27,
                        ""max"": 1.35,
                        ""mdn"": 0,
                        ""min"": 0,
                        ""timestamp"": 1705328341
                    }
                ]
            }
        ]
    }";

        [Fact]
        public void ShouldDeserializeChesterControlCloudMessage()
        {
            // Act
            ChesterControlCloudMessage? message = JsonSerializer.Deserialize<ChesterControlCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);

            // Test Counter
            Assert.NotNull(message.Counter);
            Assert.Single(message.Counter);
            Assert.Equal(2, message.Counter[0].Channel);
            Assert.Equal(6, message.Counter[0].Delta);
            Assert.Equal(7, message.Counter[0].Value);
            Assert.Single(message.Counter[0].Measurements);
            Assert.Equal(1705328341, message.Counter[0].Measurements[0].Timestamp);
            Assert.Equal(7, message.Counter[0].Measurements[0].Value);

            // Test Current
            Assert.NotNull(message.Current);
            Assert.Single(message.Current);
            Assert.Equal(4, message.Current[0].Channel);
            Assert.Single(message.Current[0].Measurements);
            Assert.Equal(2, message.Current[0].Measurements[0].Avg);
            Assert.Equal(5.03, message.Current[0].Measurements[0].Max);
            Assert.Equal(0, message.Current[0].Measurements[0].Min);
            Assert.Equal(0, message.Current[0].Measurements[0].Mdn);
            Assert.Equal(1705328341, message.Current[0].Measurements[0].Timestamp);

            // Test Voltage
            Assert.NotNull(message.Voltage);
            Assert.Single(message.Voltage);
            Assert.Equal(3, message.Voltage[0].Channel);
            Assert.Single(message.Voltage[0].Measurements);
            Assert.Equal(0.27, message.Voltage[0].Measurements[0].Avg);
            Assert.Equal(1.35, message.Voltage[0].Measurements[0].Max);
            Assert.Equal(0, message.Voltage[0].Measurements[0].Min);
            Assert.Equal(0, message.Voltage[0].Measurements[0].Mdn);
            Assert.Equal(1705328341, message.Voltage[0].Measurements[0].Timestamp);

            // Test Trigger
            Assert.NotNull(message.Trigger);
            Assert.Single(message.Trigger);
            Assert.Equal(1, message.Trigger[0].Channel);
            Assert.Equal("inactive", message.Trigger[0].State);
            Assert.Equal(8, message.Trigger[0].Events.Count);
            Assert.Equal("activated", message.Trigger[0].Events[0].Type);
            Assert.Equal("deactivated", message.Trigger[0].Events[1].Type);
            Assert.Equal(1705328233, message.Trigger[0].Events[0].Timestamp);
            Assert.Equal(1705328235, message.Trigger[0].Events[7].Timestamp);
        }
    }
}
