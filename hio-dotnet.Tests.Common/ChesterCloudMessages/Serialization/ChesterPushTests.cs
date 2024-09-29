using hio_dotnet.Common.Models.CatalogApps.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterPushTests
    {
        private string testJson = @"{
      ""message"": {
        ""version"": 1,
        ""sequence"": 1,
        ""timestamp"": 1672910024
      },
      ""attribute"": {
        ""vendor_name"": ""HARDWARIO"",
        ""product_name"": ""CHESTER-M"",
        ""hw_variant"": ""CDGLS"",
        ""hw_revision"": ""R3.2"",
        ""fw_name"": ""CHESTER Push"",
        ""fw_version"": ""v1.4.0"",
        ""serial_number"": ""2159018247""
      },
      ""system"": {
        ""uptime"": 173,
        ""voltage_rest"": 3.96,
        ""voltage_load"": 3.86,
        ""current_load"": 38
      },
      ""backup"": {
        ""line_voltage"": 0.01,
        ""batt_voltage"": 3.43,
        ""state"": ""disconnected"",
        ""events"": [
          {
            ""timestamp"": 1672910010,
            ""type"": ""disconnected""
          }
        ]
      },
      ""network"": {
        ""imei"": 351358815178303,
        ""imsi"": 901288003957939,
        ""parameter"": {
          ""eest"": 7,
          ""ecl"": 0,
          ""rsrp"": -87,
          ""rsrq"": -6,
          ""snr"": 13,
          ""plmn"": 23003,
          ""cid"": 939040,
          ""band"": 20,
          ""earfcn"": 6447
        }
      },
      ""thermometer"": {
        ""temperature"": 21.56
      },
      ""accelerometer"": {
        ""acceleration_x"": -0.31,
        ""acceleration_y"": 0.15,
        ""acceleration_z"": 9.88,
        ""orientation"": 2
      },
      ""button_x"": {
        ""count_click"": 0,
        ""count_hold"": 0,
        ""events"": []
      },
      ""button_1"": {
        ""count_click"": 3,
        ""count_hold"": 1,
        ""events"": [
          {
            ""timestamp"": 1672910020,
            ""type"": ""held""
          }
        ]
      },
      ""button_2"": {
        ""count_click"": 12,
        ""count_hold"": 0,
        ""events"": [
          {
            ""timestamp"": 1672910023,
            ""type"": ""clicked""
          }
        ]
      },
      ""button_3"": {
        ""count_click"": 0,
        ""count_hold"": 0,
        ""events"": []
      },
      ""button_4"": {
        ""count_click"": 0,
        ""count_hold"": 0,
        ""events"": []
      }
    }";

        [Fact]
        public void ShouldDeserializeChesterPushCloudMessage()
        {
            // Act
            ChesterPushCloudMessage? message = JsonSerializer.Deserialize<ChesterPushCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.NotNull(message.Message);
            Assert.Equal(1, message.Message.Version);
            Assert.Equal("HARDWARIO", message.Attribute.VendorName);
            Assert.Equal("CHESTER Push", message.Attribute.FwName);
            Assert.Equal("v1.4.0", message.Attribute.FwVersion);

            // Test Button X
            Assert.NotNull(message.ButtonX);
            Assert.Equal(0, message.ButtonX.CountClick);
            Assert.Equal(0, message.ButtonX.CountHold);
            Assert.Empty(message.ButtonX.Events);

            // Test Button 1
            Assert.NotNull(message.Button_1);
            Assert.Equal(3, message.Button_1.CountClick);
            Assert.Equal(1, message.Button_1.CountHold);
            Assert.Single(message.Button_1.Events);
            Assert.Equal(1672910020, message.Button_1.Events[0].Timestamp);
            Assert.Equal("held", message.Button_1.Events[0].Type);

            // Test Button 2
            Assert.NotNull(message.Button_2);
            Assert.Equal(12, message.Button_2.CountClick);
            Assert.Equal(0, message.Button_2.CountHold);
            Assert.Single(message.Button_2.Events);
            Assert.Equal(1672910023, message.Button_2.Events[0].Timestamp);
            Assert.Equal("clicked", message.Button_2.Events[0].Type);

            // Test Button 3
            Assert.NotNull(message.Button_3);
            Assert.Equal(0, message.Button_3.CountClick);
            Assert.Equal(0, message.Button_3.CountHold);
            Assert.Empty(message.Button_3.Events);

            // Test Button 4
            Assert.NotNull(message.Button_4);
            Assert.Equal(0, message.Button_4.CountClick);
            Assert.Equal(0, message.Button_4.CountHold);
            Assert.Empty(message.Button_4.Events);
        }
    }
}
