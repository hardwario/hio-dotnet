using hio_dotnet.Common.Models.CatalogApps.Meteo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterMeteoTests
    {
        private string testJson = @"{
      ""message"": {
        ""version"": 1,
        ""sequence"": 1,
        ""timestamp"": 1675784614
      },
      ""attribute"": {
        ""vendor_name"": ""HARDWARIO"",
        ""product_name"": ""CHESTER-M"",
        ""hw_variant"": ""CDGLS"",
        ""hw_revision"": ""R3.2"",
        ""fw_name"": ""CHESTER Meteo"",
        ""fw_version"": ""v2.0.0"",
        ""serial_number"": ""2159018267""
      },
      ""system"": {
        ""uptime"": 361,
        ""voltage_rest"": 3.6,
        ""voltage_load"": 3.56,
        ""current_load"": 35
      },
      ""backup"": {
        ""line_voltage"": 24.21,
        ""batt_voltage"": 3.41,
        ""state"": ""connected"",
        ""events"": [
          {
            ""timestamp"": 1675784338,
            ""type"": ""disconnected""
          },
          {
            ""timestamp"": 1675784518,
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
          ""rsrp"": -83,
          ""rsrq"": -4,
          ""snr"": 14,
          ""plmn"": 23003,
          ""cid"": 939040,
          ""band"": 20,
          ""earfcn"": 6447
        }
      },
      ""thermometer"": {
        ""temperature"": 21.37
      },
      ""accelerometer"": {
        ""acceleration_x"": -0.23,
        ""acceleration_y"": 0.07,
        ""acceleration_z"": 9.49,
        ""orientation"": 2
      },
      ""weather_station"": {
        ""wind_speed"": {
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""min"": 0,
              ""max"": 4,
              ""avg"": 2.78,
              ""mdn"": 2.8
            }
          ]
        },
        ""wind_direction"": {
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""value"": 0
            }
          ]
        },
        ""rainfall"": {
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""value"": 0
            }
          ]
        }
      },
      ""barometer"": {
        ""pressure"": {
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""min"": 98070,
              ""max"": 98075,
              ""avg"": 98072,
              ""mdn"": 98073
            }
          ]
        }
      },
      ""hygrometer"": {
        ""temperature"": {
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""min"": 21.53,
              ""max"": 21.67,
              ""avg"": 21.59,
              ""mdn"": 21.6
            }
          ]
        },
        ""humidity"": {
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""min"": 38.86,
              ""max"": 39.04,
              ""avg"": 38.95,
              ""mdn"": 38.95
            }
          ]
        }
      },
      ""w1_thermometers"": [
        {
          ""serial_number"": 170694685,
          ""measurements"": [
            {
              ""timestamp"": 1675784338,
              ""min"": 21.93,
              ""max"": 22.06,
              ""avg"": 22,
              ""mdn"": 22
            }
          ]
        }
      ],
      ""soil_sensors"": [
        {
          ""serial_number"": 203181,
          ""temperature"": {
            ""measurements"": [
              {
                ""timestamp"": 1675784338,
                ""min"": 22,
                ""max"": 22.06,
                ""avg"": 22.03,
                ""mdn"": 22.06
              }
            ]
          },
          ""moisture"": {
            ""measurements"": [
              {
                ""timestamp"": 1675784338,
                ""min"": 6256,
                ""max"": 6288,
                ""avg"": 6272,
                ""mdn"": 6272
              }
            ]
          }
        }
      ]
    }";

        [Fact]
        public void ShouldDeserializeChesterMeteoCloudMessage()
        {
            // Act
            ChesterMeteoCloudMessage? message = JsonSerializer.Deserialize<ChesterMeteoCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.Equal(1, message.Message.Version);
            Assert.Equal("CHESTER Meteo", message.Attribute.FwName);
            Assert.Equal("v2.0.0", message.Attribute.FwVersion);

            // Test Weather Station
            Assert.NotNull(message.WeatherStation);
            Assert.Single(message.WeatherStation.WindSpeed.Measurements);
            Assert.Equal(2.78, message.WeatherStation.WindSpeed.Measurements[0].Avg);

            // Test Barometer
            Assert.NotNull(message.Barometer);
            Assert.Single(message.Barometer.Pressure.Measurements);
            Assert.Equal(98070, message.Barometer.Pressure.Measurements[0].Min);

            // Test Hygrometer
            Assert.NotNull(message.Hygrometer);
            Assert.Single(message.Hygrometer.Temperature.Measurements);
            Assert.Equal(21.53, message.Hygrometer.Temperature.Measurements[0].Min);

            // Test W1 Thermometers
            Assert.NotNull(message.W1Thermometers);
            Assert.Single(message.W1Thermometers);
            Assert.Equal(170694685, message.W1Thermometers[0].SerialNumber);

            // Test Soil Sensors
            Assert.NotNull(message.SoilSensors);
            Assert.Single(message.SoilSensors);
            Assert.Equal(203181, message.SoilSensors[0].SerialNumber);
            Assert.Equal(22, message.SoilSensors[0].Temperature.Measurements[0].Min);
            Assert.Equal(6256, message.SoilSensors[0].Moisture.Measurements[0].Min);
        }
    }
}
