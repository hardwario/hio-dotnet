using hio_dotnet.Common.Models.CatalogApps.Boiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterBoilerTests
    {
        private string testJson = @"{
        ""accelerometer"": {
            ""accel_x"": -9.5,
            ""accel_y"": -0.08,
            ""accel_z"": 0.07,
            ""orientation"": 1
        },
        ""attribute"": {
            ""fw_name"": ""CHESTER EKOTERM"",
            ""fw_version"": ""v2.0.0"",
            ""hw_revision"": ""R3.4"",
            ""hw_variant"": ""CS"",
            ""product_name"": ""CHESTER-M"",
            ""serial_number"": ""2159019709"",
            ""vendor_name"": ""HARDWARIO""
        },
        ""current_4_20ma"": {
            ""current"": 0.44,
            ""current_avg"": 0.17,
            ""current_max"": 0.44,
            ""current_mdn"": 0,
            ""current_min"": 0,
            ""sample_count"": 17,
            ""timestamp"": 1727196115
        },
        ""input_1"": {
            ""active"": true,
            ""counter"": 1,
            ""timestamp"": 1717496190
        },
        ""input_2"": {
            ""active"": false,
            ""counter"": 0,
            ""timestamp"": 1717496190
        },
        ""line_status"": {
            ""line_present"": true
        },
        ""message"": {
            ""sequence"": 10786,
            ""timestamp"": 1727197095,
            ""version"": 1
        },
        ""network"": {
            ""parameter"": {
                ""band"": -2021165166,
                ""cid"": 245809,
                ""earfcn"": 1794177621,
                ""ecl"": 536887984,
                ""eest"": 0,
                ""plmn"": 536887984,
                ""rsrp"": 384823,
                ""rsrq"": 508,
                ""snr"": 0
            }
        },
        ""system"": {
            ""current_load"": 45,
            ""uptime"": 9701004,
            ""voltage_load"": 4.58,
            ""voltage_rest"": 4.7
        },
        ""thermometer"": {
            ""temperature"": 28.06
        },
        ""voltage_0_10v"": {
            ""sample_count"": 17,
            ""timestamp"": 1727196115,
            ""voltage"": 0,
            ""voltage_avg"": 0.03,
            ""voltage_max"": 0.12,
            ""voltage_mdn"": 0,
            ""voltage_min"": 0
        },
        ""w1_thermometers"": [
            {
                ""sample_count"": 17,
                ""serial_number"": 216670135,
                ""temperature"": 29.5,
                ""temperature_avg"": 29.45,
                ""temperature_max"": 29.5,
                ""temperature_mdn"": 29.43,
                ""temperature_min"": 29.37,
                ""timestamp"": 1727196115
            },
            {
                ""sample_count"": 17,
                ""serial_number"": 222757381,
                ""temperature"": 56.81,
                ""temperature_avg"": 56.56,
                ""temperature_max"": 56.87,
                ""temperature_mdn"": 56.56,
                ""temperature_min"": 56.25,
                ""timestamp"": 1727196116
            }
        ]
    }";

        [Fact]
        public void ShouldDeserializeChesterBoilerCloudMessage()
        {
            // Act
            ChesterBoilerCloudMessage? message = JsonSerializer.Deserialize<ChesterBoilerCloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);

            // Test Current Loop
            Assert.NotNull(message.CurrentLoop);
            Assert.Equal(0.44, message.CurrentLoop.Current);
            Assert.Equal(0.17, message.CurrentLoop.Current_Avg);
            Assert.Equal(0.44, message.CurrentLoop.Current_Max);
            Assert.Equal(0.0, message.CurrentLoop.Current_Mdn);
            Assert.Equal(0.0, message.CurrentLoop.Current_Min);
            Assert.Equal(17, message.CurrentLoop.SampleCount);
            Assert.Equal(1727196115, message.CurrentLoop.Timestamp);

            // Test Input_1
            Assert.NotNull(message.Input_1);
            Assert.True(message.Input_1.Active);
            Assert.Equal(1, message.Input_1.Counter);
            Assert.Equal(1717496190, message.Input_1.Timestamp);

            // Test Input_2
            Assert.NotNull(message.Input_2);
            Assert.False(message.Input_2.Active);
            Assert.Equal(0, message.Input_2.Counter);
            Assert.Equal(1717496190, message.Input_2.Timestamp);

            // Test Line Status
            Assert.NotNull(message.LineStatus);
            Assert.True(message.LineStatus.LinePresent);

            // Test Voltage Input
            Assert.NotNull(message.VoltageInput);
            Assert.Equal(0, message.VoltageInput.Voltage);
            Assert.Equal(0.03, message.VoltageInput.Voltage_Avg);
            Assert.Equal(0.12, message.VoltageInput.Voltage_Max);
            Assert.Equal(0.0, message.VoltageInput.Voltage_Mdn);
            Assert.Equal(0.0, message.VoltageInput.Voltage_Min);
            Assert.Equal(17, message.VoltageInput.SampleCount);
            Assert.Equal(1727196115, message.VoltageInput.Timestamp);

            // Test W1Thermometers
            Assert.NotNull(message.W1Thermometers);
            Assert.Equal(2, message.W1Thermometers.Count);

            var firstThermometer = message.W1Thermometers[0];
            Assert.Equal(216670135, firstThermometer.SerialNumber);
            Assert.Equal(29.5, firstThermometer.Temperature);
            Assert.Equal(29.45, firstThermometer.Temperature_Avg);
            Assert.Equal(29.5, firstThermometer.Temperature_Max);
            Assert.Equal(29.43, firstThermometer.Temperature_Mdn);
            Assert.Equal(29.37, firstThermometer.Temperature_Min);
            Assert.Equal(17, firstThermometer.SampleCount);
            Assert.Equal(1727196115, firstThermometer.Timestamp);

            var secondThermometer = message.W1Thermometers[1];
            Assert.Equal(222757381, secondThermometer.SerialNumber);
            Assert.Equal(56.81, secondThermometer.Temperature);
            Assert.Equal(56.56, secondThermometer.Temperature_Avg);
            Assert.Equal(56.87, secondThermometer.Temperature_Max);
            Assert.Equal(56.56, secondThermometer.Temperature_Mdn);
            Assert.Equal(56.25, secondThermometer.Temperature_Min);
            Assert.Equal(17, secondThermometer.SampleCount);
            Assert.Equal(1727196116, secondThermometer.Timestamp);
        }
    }
}
