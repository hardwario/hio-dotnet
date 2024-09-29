using hio_dotnet.Common.Models.CatalogApps.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterInputTests
    {
        private string testJson = @"{
      ""message"": {
        ""version"": 1,
        ""sequence"": 7,
        ""timestamp"": 1670580791
      },
      ""attribute"": {
        ""vendor_name"": ""HARDWARIO"",
        ""product_name"": ""CHESTER-M"",
        ""hw_variant"": ""CDGLS"",
        ""hw_revision"": ""R3.2"",
        ""fw_name"": ""CHESTER Input"",
        ""fw_version"": ""v1.0.0"",
        ""serial_number"": ""2159018247""
      },
      ""system"": {
        ""uptime"": 2058,
        ""voltage_rest"": 3.74,
        ""voltage_load"": 3.65,
        ""current_load"": 36
      },
      ""backup"": {
        ""line_voltage"": 24.21,
        ""batt_voltage"": 3.41,
        ""state"": ""connected"",
        ""events"": [
          {
            ""timestamp"": 1670580549,
            ""type"": ""disconnected""
          },
          {
            ""timestamp"": 1670580552,
            ""type"": ""connected""
          }
        ]
      },
      ""network"": {
        ""imei"": 351358815178303,
        ""imsi"": 901288003957939,
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
      ""thermometer"": {
        ""temperature"": 23.06
      },
      ""accelerometer"": {
        ""acceleration_x"": 0.07,
        ""acceleration_y"": 0.38,
        ""acceleration_z"": 9.88,
        ""orientation"": 2
      },
      ""trigger"": {
        ""state"": ""inactive"",
        ""events"": [
          {
            ""timestamp"": 1670580550,
            ""type"": ""activated""
          },
          {
            ""timestamp"": 1670580553,
            ""type"": ""deactivated""
          },
          {
            ""timestamp"": 1670580631,
            ""type"": ""activated""
          },
          {
            ""timestamp"": 1670580634,
            ""type"": ""deactivated""
          }
        ]
      },
      ""counter"": {
        ""value"": 12586,
        ""measurements"": [
          {
            ""timestamp"": 1670580548,
            ""value"": 12526
          },
          {
            ""timestamp"": 1670580698,
            ""value"": 12583
          }
        ]
      },
      ""voltage"": {
        ""measurements"": [
          {
            ""timestamp"": 1670580548,
            ""min"": 11.27,
            ""max"": 11.35,
            ""avg"": 11.31,
            ""mdn"": 11.35
          },
          {
            ""timestamp"": 1670580698,
            ""min"": 11.26,
            ""max"": 11.35,
            ""avg"": 11.29,
            ""mdn"": 11.27
          }
        ]
      },
      ""current"": {
        ""measurements"": [
          {
            ""timestamp"": 1670580548,
            ""min"": 10.55,
            ""max"": 10.91,
            ""avg"": 10.73,
            ""mdn"": 10.91
          },
          {
            ""timestamp"": 1670580698,
            ""min"": 10.51,
            ""max"": 10.91,
            ""avg"": 10.66,
            ""mdn"": 10.55
          }
        ]
      },
      ""hygrometer"": {
        ""temperature"": {
          ""measurements"": [
            {
              ""timestamp"": 1670580548,
              ""min"": 22.99,
              ""max"": 23.02,
              ""avg"": 23.01,
              ""mdn"": 23.02
            },
            {
              ""timestamp"": 1670580698,
              ""min"": 23.02,
              ""max"": 23.08,
              ""avg"": 23.05,
              ""mdn"": 23.06
            }
          ]
        },
        ""humidity"": {
          ""measurements"": [
            {
              ""timestamp"": 1670580548,
              ""min"": 49.66,
              ""max"": 49.74,
              ""avg"": 49.7,
              ""mdn"": 49.74
            },
            {
              ""timestamp"": 1670580698,
              ""min"": 49.62,
              ""max"": 50.07,
              ""avg"": 49.84,
              ""mdn"": 49.82
            }
          ]
        }
      }
    }";

        [Fact]
        public void ShouldDeserializeChesterInputCloudMessage()
        {
            // Act
            ChesterInputCloudMessage? message = JsonSerializer.Deserialize<ChesterInputCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.NotNull(message.Message);
            Assert.Equal(1, message.Message.Version);
            Assert.Equal("HARDWARIO", message.Attribute.VendorName);
            Assert.Equal("CHESTER-M", message.Attribute.ProductName);

            // Test Trigger
            Assert.NotNull(message.Trigger);
            Assert.Equal("inactive", message.Trigger.State);
            Assert.NotNull(message.Trigger.Events);
            Assert.Equal(4, message.Trigger.Events.Count);
            Assert.Equal(1670580550, message.Trigger.Events[0].Timestamp);

            // Test Counter
            Assert.NotNull(message.Counter);
            Assert.Equal(12586, message.Counter.Value);
            Assert.Equal(2, message.Counter.Measurements.Count);
            Assert.Equal(12526, message.Counter.Measurements[0].Value);

            // Test Voltage
            Assert.NotNull(message.Voltage);
            Assert.NotNull(message.Voltage.Measurements);
            Assert.Equal(2, message.Voltage.Measurements.Count);
            Assert.Equal(11.27, message.Voltage.Measurements[0].Min);

            // Test Current
            Assert.NotNull(message.Current);
            Assert.NotNull(message.Current.Measurements);
            Assert.Equal(2, message.Current.Measurements.Count);
            Assert.Equal(10.55, message.Current.Measurements[0].Min);

            // Test Hygrometer
            Assert.NotNull(message.Hygrometer);
            Assert.NotNull(message.Hygrometer.Temperature.Measurements);
            Assert.Equal(2, message.Hygrometer.Temperature.Measurements.Count);
            Assert.Equal(22.99, message.Hygrometer.Temperature.Measurements[0].Min);
        }
    }
}
