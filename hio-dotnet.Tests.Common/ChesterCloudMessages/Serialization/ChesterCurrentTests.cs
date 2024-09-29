using hio_dotnet.Common.Models.CatalogApps.Current;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterCurrentTests
    {
        private string testJson = @"{
      ""message"": {
        ""version"": 1,
        ""sequence"": 1,
        ""timestamp"": 1673272805
      },
      ""attribute"": {
        ""vendor_name"": ""HARDWARIO"",
        ""product_name"": ""CHESTER-M"",
        ""hw_variant"": ""CDGLS"",
        ""hw_revision"": ""R3.2"",
        ""fw_name"": ""CHESTER Current"",
        ""fw_version"": ""v1.5.0"",
        ""serial_number"": ""2159018247""
      },
      ""system"": {
        ""uptime"": 131,
        ""voltage_rest"": 4.73,
        ""voltage_load"": 4.67,
        ""current_load"": 46
      },
      ""backup"": {
        ""line_voltage"": 9.51,
        ""batt_voltage"": 3.45,
        ""state"": ""connected"",
        ""events"": []
      },
      ""network"": {
        ""imei"": 351358815178303,
        ""imsi"": 901288003957939,
        ""parameter"": {
          ""eest"": 7,
          ""ecl"": 0,
          ""rsrp"": -87,
          ""rsrq"": -4,
          ""snr"": 16,
          ""plmn"": 23003,
          ""cid"": 939040,
          ""band"": 20,
          ""earfcn"": 6447
        }
      },
      ""thermometer"": {
        ""temperature"": 22.68
      },
      ""accelerometer"": {
        ""acceleration_x"": -0.16,
        ""acceleration_y"": 0.07,
        ""acceleration_z"": 9.88,
        ""orientation"": 2
      },
      ""analog_channels"": [
        {
          ""channel"": 1,
          ""measurements"": [
            {
              ""timestamp"": 1673272718,
              ""mean_min"": 1037,
              ""mean_max"": 1039,
              ""mean_avg"": 1038,
              ""mean_mdn"": 1038,
              ""rms_min"": 1037,
              ""rms_max"": 1039,
              ""rms_avg"": 1038,
              ""rms_mdn"": 1038
            }
          ]
        }
      ],
      ""w1_thermometers"": [
        {
          ""serial_number"": 170783697,
          ""measurements"": [
            {
              ""timestamp"": 1673272718,
              ""min"": 21.25,
              ""max"": 21.25,
              ""avg"": 21.25,
              ""mdn"": 21.25
            }
          ]
        }
      ]
    }";

        [Fact]
        public void ShouldDeserializeChesterCurrentCloudMessage()
        {
            // Act
            ChesterCurrentCloudMessage? message = JsonSerializer.Deserialize<ChesterCurrentCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.NotNull(message.Message);
            Assert.Equal(1, message.Message.Version);
            Assert.Equal("HARDWARIO", message.Attribute.VendorName);
            Assert.Equal("CHESTER-M", message.Attribute.ProductName);
            Assert.Equal("CHESTER Current", message.Attribute.FwName);
            Assert.Equal("v1.5.0", message.Attribute.FwVersion);

            // Test AnalogChannels
            Assert.NotNull(message.AnalogChannels);
            Assert.Single(message.AnalogChannels);
            Assert.Equal(1, message.AnalogChannels[0].Channel);
            Assert.Single(message.AnalogChannels[0].Measurements);
            Assert.Equal(1037, message.AnalogChannels[0].Measurements[0].MeanMin);
            Assert.Equal(1038, message.AnalogChannels[0].Measurements[0].MeanAvg);
            Assert.Equal(1039, message.AnalogChannels[0].Measurements[0].MeanMax);

            // Test W1Thermometers
            Assert.NotNull(message.W1Thermometers);
            Assert.Single(message.W1Thermometers);
            Assert.Equal(170783697, message.W1Thermometers[0].SerialNumber);
            Assert.Single(message.W1Thermometers[0].Measurements);
            Assert.Equal(21.25, message.W1Thermometers[0].Measurements[0].Min);
        }
    }
}
