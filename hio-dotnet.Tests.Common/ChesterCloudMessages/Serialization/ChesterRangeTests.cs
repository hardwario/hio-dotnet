using hio_dotnet.Common.Models.CatalogApps.Range;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterRangeTests
    {
        private string testJson = @"{
      ""message"": {
        ""version"": 1,
        ""sequence"": 1,
        ""timestamp"": 1685093572
      },
      ""attribute"": {
        ""vendor_name"": ""HARDWARIO"",
        ""product_name"": ""CHESTER-M"",
        ""hw_variant"": ""CDGLS"",
        ""hw_revision"": ""R3.4"",
        ""fw_name"": ""(unset)"",
        ""fw_version"": ""(unset)"",
        ""serial_number"": ""2159019054""
      },
      ""system"": {
        ""uptime"": 49,
        ""voltage_rest"": null,
        ""voltage_load"": null,
        ""current_load"": null
      },
      ""network"": {
        ""imei"": 351358816140765,
        ""imsi"": 901288910018953,
        ""parameter"": {
          ""eest"": 7,
          ""ecl"": 0,
          ""rsrp"": -78,
          ""rsrq"": -5,
          ""snr"": 8,
          ""plmn"": 23003,
          ""cid"": 1011233,
          ""band"": 20,
          ""earfcn"": 6447
        }
      },
      ""thermometer"": {
        ""temperature"": 24.93
      },
      ""accelerometer"": {
        ""acceleration_x"": 0,
        ""acceleration_y"": -0.23,
        ""acceleration_z"": 9.65,
        ""orientation"": 2
      },
      ""ultrasonic_ranger"": {
        ""distance"": {
          ""measurements"": [
            {
              ""timestamp"": 1685093569,
              ""min"": 2.004,
              ""max"": 2.009,
              ""avg"": 2.008,
              ""mdn"": 2.008
            }
          ]
        }
      },
      ""hygrometer"": {
        ""temperature"": {
          ""events"": [],
          ""measurements"": [
            {
              ""timestamp"": 1685093569,
              ""min"": 24.9,
              ""max"": 25.03,
              ""avg"": 24.99,
              ""mdn"": 25.01
            }
          ]
        },
        ""humidity"": {
          ""measurements"": [
            {
              ""timestamp"": 1685093569,
              ""min"": 35.18,
              ""max"": 35.81,
              ""avg"": 35.45,
              ""mdn"": 35.36
            }
          ]
        }
      },
      ""w1_thermometers"": [
        {
          ""serial_number"": 222768959,
          ""measurements"": [
            {
              ""timestamp"": 1685093569,
              ""min"": 24.31,
              ""max"": 24.31,
              ""avg"": 24.31,
              ""mdn"": 24.31
            }
          ]
        },
        {
          ""serial_number"": 222690915,
          ""measurements"": [
            {
              ""timestamp"": 1685093569,
              ""min"": 27,
              ""max"": 27.43,
              ""avg"": 27.22,
              ""mdn"": 27.25
            }
          ]
        }
      ]
    }";

        [Fact]
        public void ShouldDeserializeChesterRangeCloudMessage()
        {
            // Act
            ChesterRangeCloudMessage? message = JsonSerializer.Deserialize<ChesterRangeCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.Equal(1, message.Message.Version);
            Assert.Equal("(unset)", message.Attribute.FwName);
            Assert.Equal("(unset)", message.Attribute.FwVersion);
            Assert.Equal("2159019054", message.Attribute.SerialNumber);

            // Test Ultrasonic Ranger
            Assert.NotNull(message.UltrasonicRanger);
            Assert.Single(message.UltrasonicRanger.Distance.Measurements);
            Assert.Equal(2.004, message.UltrasonicRanger.Distance.Measurements[0].Min);
            Assert.Equal(2.008, message.UltrasonicRanger.Distance.Measurements[0].Avg);
            Assert.Equal(2.009, message.UltrasonicRanger.Distance.Measurements[0].Max);
            Assert.Equal(2.008, message.UltrasonicRanger.Distance.Measurements[0].Mdn);

            // Test Hygrometer
            Assert.NotNull(message.Hygrometer);
            Assert.Single(message.Hygrometer.Temperature.Measurements);
            Assert.Equal(24.9, message.Hygrometer.Temperature.Measurements[0].Min);
            Assert.Equal(25.01, message.Hygrometer.Temperature.Measurements[0].Mdn);

            Assert.Single(message.Hygrometer.Humidity.Measurements);
            Assert.Equal(35.18, message.Hygrometer.Humidity.Measurements[0].Min);
            Assert.Equal(35.36, message.Hygrometer.Humidity.Measurements[0].Mdn);

            // Test W1 Thermometers
            Assert.NotNull(message.W1Thermometers);
            Assert.Equal(2, message.W1Thermometers.Count);
            Assert.Equal(222768959, message.W1Thermometers[0].SerialNumber);
            Assert.Equal(24.31, message.W1Thermometers[0].Measurements[0].Min);

            Assert.Equal(222690915, message.W1Thermometers[1].SerialNumber);
            Assert.Equal(27, message.W1Thermometers[1].Measurements[0].Min);
            Assert.Equal(27.25, message.W1Thermometers[1].Measurements[0].Mdn);
        }
    }
}
