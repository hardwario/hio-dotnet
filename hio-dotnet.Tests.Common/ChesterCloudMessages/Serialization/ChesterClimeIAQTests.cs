using hio_dotnet.Common.Models.CatalogApps.ClimeIAQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterClimeIAQTests
    {
        private string testJson = @"{
      ""message"": {
        ""version"": 1,
        ""sequence"": 0,
        ""timestamp"": 1668859482
      },
      ""attribute"": {
        ""vendor_name"": ""HARDWARIO"",
        ""product_name"": ""CHESTER-M"",
        ""hw_variant"": ""CGLS"",
        ""hw_revision"": ""R3.2"",
        ""fw_name"": ""CHESTER Clime"",
        ""fw_version"": ""v1.4.0"",
        ""serial_number"": ""2159018267""
      },
      ""system"": {
        ""uptime"": 680967,
        ""voltage_rest"": 3.7,
        ""voltage_load"": 3.66,
        ""current_load"": 36
      },
      ""backup"": {
          ""line_voltage"": 24.01,
          ""batt_voltage"": 4.09,
          ""state"": ""connected"",
          ""events"": [
              {
                  ""timestamp"": 1668858942,
                  ""type"": ""connected""
              }
          ]
      },
      ""network"": {
        ""imei"": 351358815180770,
        ""imsi"": 901288910018982,
        ""parameter"": {
          ""eest"": 7,
          ""ecl"": 0,
          ""rsrp"": -90,
          ""rsrq"": -8,
          ""snr"": 9,
          ""plmn"": 23003,
          ""cid"": 939040,
          ""band"": 20,
          ""earfcn"": 6447
        }
      },
      ""iaq_sensor"": {
        ""temperature"": {
          ""measurements"": [
            {
              ""timestamp"": 1668857742,
              ""min"": 22.5,
              ""max"": 22.54,
              ""avg"": 22.51,
              ""mdn"": 22.52
            }
          ]
        },
        ""humidity"": {
          ""measurements"": [
            {
              ""timestamp"": 1668857742,
              ""min"": 55.19,
              ""max"": 55.91,
              ""avg"": 55.52,
              ""mdn"": 55.53
            }
          ]
        }
      },
      ""hygrometer"": {
        ""temperature"": {
          ""measurements"": [
            {
              ""timestamp"": 1668857742,
              ""min"": 22.07,
              ""max"": 22.25,
              ""avg"": 22.17,
              ""mdn"": 22.16
            }
          ]
        },
        ""humidity"": {
          ""measurements"": [
            {
              ""timestamp"": 1668857742,
              ""min"": 54.78,
              ""max"": 55.31,
              ""avg"": 55.1,
              ""mdn"": 55.12
            }
          ]
        }
      },
      ""ble_tags"": [
        {
          ""addr"": ""1234567890AB"",
          ""rssi"": -81,
          ""voltage"": 3.11,
          ""humidity"": {
            ""measurements"": [
              {
                ""timestamp"": 1668857742,
                ""min"": 54.78,
                ""max"": 55.31,
                ""avg"": 55.1,
                ""mdn"": 55.12
              }
            ]
          },
          ""temperature"": {
            ""measurements"": [
              {
                ""timestamp"": 1668857742,
                ""min"": 22.18,
                ""max"": 22.25,
                ""avg"": 22.23,
                ""mdn"": 22.25
              }
            ]
          }
        }
      ]
    }";

        [Fact]
        public void ShouldDeserializeChesterClimeIAQCloudMessage()
        {
            // Act
            ChesterClimeIAQCloudMessage? message = JsonSerializer.Deserialize<ChesterClimeIAQCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.NotNull(message.Message);
            Assert.Equal(1, message.Message.Version);
            Assert.Equal("HARDWARIO", message.Attribute.VendorName);
            Assert.Equal("CHESTER-M", message.Attribute.ProductName);

            // Test IAQ_Sensor
            Assert.NotNull(message.IAQ_Sensor);
            Assert.Single(message.IAQ_Sensor.Temperature.Measurements);
            Assert.Equal(22.5, message.IAQ_Sensor.Temperature.Measurements[0].Min);

            // Test Hygrometer
            Assert.NotNull(message.Hygrometer);
            Assert.Single(message.Hygrometer.Temperature.Measurements);
            Assert.Equal(22.07, message.Hygrometer.Temperature.Measurements[0].Min);

            // Test BLE Tags
            Assert.NotNull(message.BLE_Tags);
            Assert.Single(message.BLE_Tags);
            Assert.Equal("1234567890AB", message.BLE_Tags[0].Addr);
            Assert.Equal(-81, message.BLE_Tags[0].Rssi);
            Assert.Single(message.BLE_Tags[0].Temperature.Measurements);
            Assert.Equal(22.18, message.BLE_Tags[0].Temperature.Measurements[0].Min);
        }
    }
}
