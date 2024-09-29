using hio_dotnet.Common.Models.CatalogApps.wMBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterWMBusTests
    {
        private string testJson = @"{
        ""accelerometer"": {
            ""accel_x"": 0.22,
            ""accel_y"": 0.07,
            ""accel_z"": 9.42,
            ""orientation"": 2
        },
        ""battery"": {
            ""current_load"": null,
            ""voltage_load"": null,
            ""voltage_rest"": null
        },
        ""frame"": {
            ""protocol"": 3,
            ""sequence"": 0,
            ""timestamp"": 1698660040
        },
        ""network"": {
            ""parameter"": {
                ""band"": 1184866148,
                ""cid"": 248833,
                ""earfcn"": -2121962691,
                ""ecl"": 536882852,
                ""eest"": 0,
                ""plmn"": 536882852,
                ""rsrp"": 384479,
                ""rsrq"": 508,
                ""snr"": 0
            }
        },
        ""state"": {
            ""uptime"": 47
        },
        ""thermometer"": {
            ""temperature"": 22.31
        },
        ""wmbus"": {
            ""cycle"": 1,
            ""devices"": 1,
            ""packets"": [
                {
                    ""data"": ""32446850003076816980a0919f2b06007007000061087c08000000000000000000000000010101020100000000000000000000"",
                    ""rssi"": -65
                },
                {
                    ""data"": ""32446850003076816980a0919f2b06007007000061087c08000000000000000000000000010101020100000000000000000000"",
                    ""rssi"": -72
                }
            ],
            ""part"": 0,
            ""received"": 1,
            ""scan_time"": 2
        }
    }";

        [Fact]
        public void ShouldDeserializeChesterWMBusCloudMessage()
        {
            // Act
            ChesterWMBusCloudMessage? message = JsonSerializer.Deserialize<ChesterWMBusCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);

            // Test WMBusData
            Assert.NotNull(message.WMBus);
            Assert.Equal(1, message.WMBus.Cycle);
            Assert.Equal(1, message.WMBus.Devices);
            Assert.Equal(0, message.WMBus.Part);
            Assert.Equal(1, message.WMBus.Received);
            Assert.Equal(2, message.WMBus.ScanTime);

            // Test WMBus Packets
            Assert.NotNull(message.WMBus.Packets);
            Assert.Equal(2, message.WMBus.Packets.Count);

            // First packet
            var firstPacket = message.WMBus.Packets[0];
            Assert.Equal("32446850003076816980a0919f2b06007007000061087c08000000000000000000000000010101020100000000000000000000", firstPacket.Data);
            Assert.Equal(-65, firstPacket.Rssi);

            // Second packet
            var secondPacket = message.WMBus.Packets[1];
            Assert.Equal("32446850003076816980a0919f2b06007007000061087c08000000000000000000000000010101020100000000000000000000", secondPacket.Data);
            Assert.Equal(-72, secondPacket.Rssi);
        }
    }
}
