using hio_dotnet.Common.Models.CatalogApps.Radon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterRadonTests
    {
        private string testJson = @"{
        ""accelerometer"": {
            ""accel_x"": -0.16,
            ""accel_y"": 0,
            ""accel_z"": 9.57,
            ""orientation"": 2
        },
        ""attribute"": {
            ""fw_version"": ""v1.0.0"",
            ""hw_revision"": ""R3.2"",
            ""hw_variant"": ""CDGLS"",
            ""product_name"": ""CHESTER-M"",
            ""serial_number"": ""2159018266"",
            ""vendor_name"": ""HARDWARIO""
        },
        ""message"": {
            ""sequence"": 102,
            ""timestamp"": 1727633837,
            ""version"": 1
        },
        ""network"": {
            ""imei"": 425596776,
            ""imsi"": 2907855274
        },
        ""radon_probe"": {
            ""chamber_humidity"": {
                ""measurements"": [
                    {
                        ""avg"": 43,
                        ""max"": 43,
                        ""mdn"": 43,
                        ""min"": 43,
                        ""timestamp"": 1727632199
                    }
                ]
            },
            ""chamber_temperature"": {
                ""measurements"": [
                    {
                        ""avg"": 20,
                        ""max"": 20,
                        ""mdn"": 20,
                        ""min"": 20,
                        ""timestamp"": 1727632199
                    }
                ]
            },
            ""concentration_daily"": {
                ""measurements"": [
                    {
                        ""timestamp"": 1727632199,
                        ""value"": 1
                    }
                ]
            },
            ""concentration_hourly"": {
                ""measurements"": [
                    {
                        ""timestamp"": 1727632199,
                        ""value"": 17
                    }
                ]
            }
        },
        ""system"": {
            ""current_load"": 29,
            ""uptime"": 184458,
            ""voltage_load"": 2991,
            ""voltage_rest"": 3051
        },
        ""thermometer"": {
            ""temperature"": 19.18
        }
    }";

        [Fact]
        public void ShouldDeserializeChesterRadonCloudMessage()
        {
            // Act
            ChesterRadonCloudMessage? message = JsonSerializer.Deserialize<ChesterRadonCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.Equal("v1.0.0", message.Attribute.FwVersion);
            Assert.Equal("HARDWARIO", message.Attribute.VendorName);

            // Test Radon Probe Data
            Assert.NotNull(message.RadonProbe);

            // Chamber Humidity
            var chamberHumidity = message.RadonProbe.ChamberHumidity.Measurements;
            Assert.Single(chamberHumidity);
            Assert.Equal(43, chamberHumidity[0].Avg);
            Assert.Equal(43, chamberHumidity[0].Max);

            // Chamber Temperature
            var chamberTemperature = message.RadonProbe.ChamberTemperature.Measurements;
            Assert.Single(chamberTemperature);
            Assert.Equal(20, chamberTemperature[0].Avg);

            // Concentration Daily
            var concentrationDaily = message.RadonProbe.ConcentrationDaily.Measurements;
            Assert.Single(concentrationDaily);
            Assert.Equal(1, concentrationDaily[0].Value);

            // Concentration Hourly
            var concentrationHourly = message.RadonProbe.ConcentrationHourly.Measurements;
            Assert.Single(concentrationHourly);
            Assert.Equal(17, concentrationHourly[0].Value);
        }
    }
}
