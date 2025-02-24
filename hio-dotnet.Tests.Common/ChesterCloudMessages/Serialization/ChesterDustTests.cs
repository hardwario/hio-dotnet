using hio_dotnet.Common.Models.CatalogApps.Dust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common.ChesterCloudMessages.Serialization
{
    public class ChesterDustTests
    {
        private string testJson = @"{
            ""accelerometer"": {
                ""accel_x"": 0.22,
                ""accel_y"": 9.88,
                ""accel_z"": -0.69,
                ""orientation"": 3
            },
            ""attribute"": {
                ""fw_name"": ""CHESTER Dust"",
                ""fw_version"": ""v2.0.0"",
                ""hw_revision"": ""R3.4"",
                ""hw_variant"": ""CS"",
                ""product_name"": ""CHESTER-M"",
                ""serial_number"": ""2159019709"",
                ""vendor_name"": ""HARDWARIO""
            },
            ""ble_tags"": [],
            ""message"": {
                ""sequence"": 339,
                ""timestamp"": 1730911513,
                ""version"": 2
            },
            ""network"": {
                ""parameter"": {
                    ""band"": 20,
                    ""cid"": 401184,
                    ""earfcn"": 6447,
                    ""ecl"": 0,
                    ""eest"": 8,
                    ""plmn"": 23003,
                    ""rsrp"": -66,
                    ""rsrq"": -7,
                    ""snr"": 18
                }
            },
            ""soil_sensors"": [],
            ""sps30"": {
                ""num_pm0_5"": {
                    ""measurements"": [
                        {""avg"": 76720.24, ""max"": 77018.25, ""mdn"": 76701.64, ""min"": 76241.41, ""timestamp"": 1730909742},
                        {""avg"": 76881.44, ""max"": 77455.47, ""mdn"": 77078.58, ""min"": 76349.33, ""timestamp"": 1730910042},
                        {""avg"": 77085.37, ""max"": 77346.5, ""mdn"": 77262.69, ""min"": 76709.6, ""timestamp"": 1730910342},
                        {""avg"": 77317.62, ""max"": 77984, ""mdn"": 77250.89, ""min"": 76802.36, ""timestamp"": 1730910642},
                        {""avg"": 77747.6, ""max"": 78036.83, ""mdn"": 77802.73, ""min"": 77261.64, ""timestamp"": 1730910942},
                        {""avg"": 77860.27, ""max"": 78341.22, ""mdn"": 77939.28, ""min"": 77512.8, ""timestamp"": 1730911242}
                    ]
                },
                ""num_pm1"": {
                    ""measurements"": [
                        {""avg"": 97791.53, ""max"": 98315.52, ""mdn"": 97985.4, ""min"": 97145.82, ""timestamp"": 1730909742},
                        {""avg"": 98167.51, ""max"": 98826.48, ""mdn"": 98232.84, ""min"": 97542.27, ""timestamp"": 1730910042},
                        {""avg"": 98484.88, ""max"": 98871.84, ""mdn"": 98402.04, ""min"": 98214.52, ""timestamp"": 1730910342},
                        {""avg"": 98146.52, ""max"": 98507.17, ""mdn"": 98049.32, ""min"": 97901.3, ""timestamp"": 1730910642},
                        {""avg"": 98887.46, ""max"": 99099.94, ""mdn"": 98950.29, ""min"": 98555.45, ""timestamp"": 1730910942},
                        {""avg"": 99066.66, ""max"": 99346.49, ""mdn"": 99098.65, ""min"": 98872.42, ""timestamp"": 1730911242}
                    ]
                },
                ""num_pm10"": {
                    ""measurements"": [
                        {""avg"": 104967.51, ""max"": 105930.1, ""mdn"": 105044.75, ""min"": 103836.79, ""timestamp"": 1730909742},
                        {""avg"": 105491.04, ""max"": 106485.2, ""mdn"": 105642.37, ""min"": 104460.48, ""timestamp"": 1730910042},
                        {""avg"": 105872.09, ""max"": 106797.62, ""mdn"": 105543.15, ""min"": 105157.51, ""timestamp"": 1730910342},
                        {""avg"": 105062.57, ""max"": 105250.48, ""mdn"": 105088.91, ""min"": 104917.91, ""timestamp"": 1730910642},
                        {""avg"": 105993.58, ""max"": 106288.04, ""mdn"": 105845.85, ""min"": 105791.7, ""timestamp"": 1730910942},
                        {""avg"": 106210.97, ""max"": 106637.35, ""mdn"": 106196.31, ""min"": 105836.5, ""timestamp"": 1730911242}
                    ]
                },
                ""num_pm2_5"": {
                    ""measurements"": [
                        {""avg"": 103643.88, ""max"": 104524, ""mdn"": 103743.15, ""min"": 102604.34, ""timestamp"": 1730909742},
                        {""avg"": 104139.72, ""max"": 105070.91, ""mdn"": 104298.62, ""min"": 103248.54, ""timestamp"": 1730910042},
                        {""avg"": 104508.87, ""max"": 105333.05, ""mdn"": 104226.26, ""min"": 103877.89, ""timestamp"": 1730910342},
                        {""avg"": 103788.01, ""max"": 103918.54, ""mdn"": 103763.13, ""min"": 103678.07, ""timestamp"": 1730910642},
                        {""avg"": 104683.43, ""max"": 104949.3, ""mdn"": 104575.39, ""min"": 104495.67, ""timestamp"": 1730910942},
                        {""avg"": 104893.67, ""max"": 105292.54, ""mdn"": 104862.36, ""min"": 104595.78, ""timestamp"": 1730911242}
                    ]
                },
                ""num_pm4"": {
                    ""measurements"": [
                        {""avg"": 104720.17, ""max"": 105667.99, ""mdn"": 104801.33, ""min"": 103605.8, ""timestamp"": 1730909742},
                        {""avg"": 105238.72, ""max"": 106221.56, ""mdn"": 105391.38, ""min"": 104233.07, ""timestamp"": 1730910042},
                        {""avg"": 105617.62, ""max"": 106525.03, ""mdn"": 105296.95, ""min"": 104917.98, ""timestamp"": 1730910342},
                        {""avg"": 104823.95, ""max"": 105001.64, ""mdn"": 104841.18, ""min"": 104687.46, ""timestamp"": 1730910642},
                        {""avg"": 105748.52, ""max"": 106037.87, ""mdn"": 105607.88, ""min"": 105550.55, ""timestamp"": 1730910942},
                        {""avg"": 105964.62, ""max"": 106386.06, ""mdn"": 105951.54, ""min"": 105603.82, ""timestamp"": 1730911242}
                    ]
                },
                ""pm1"": {
                    ""measurements"": [
                        {""avg"": 13032.99, ""max"": 13146.25, ""mdn"": 13044.63, ""min"": 12899.44, ""timestamp"": 1730909742},
                        {""avg"": 13096.11, ""max"": 13215.06, ""mdn"": 13115.75, ""min"": 12979.41, ""timestamp"": 1730910042},
                        {""avg"": 13142.8, ""max"": 13249.73, ""mdn"": 13105.71, ""min"": 13060.71, ""timestamp"": 1730910342},
                        {""avg"": 13049.27, ""max"": 13067.7, ""mdn"": 13048.01, ""min"": 13035.22, ""timestamp"": 1730910642},
                        {""avg"": 13162.74, ""max"": 13197.09, ""mdn"": 13147.76, ""min"": 13139.39, ""timestamp"": 1730910942},
                        {""avg"": 13189.34, ""max"": 13240.31, ""mdn"": 13186.42, ""min"": 13149.24, ""timestamp"": 1730911242}
                    ]
                },
                ""pm2_5"": {
                    ""measurements"": [
                        {""avg"": 18996.38, ""max"": 19450.83, ""mdn"": 19018.8, ""min"": 18485.27, ""timestamp"": 1730909742},
                        {""avg"": 19175.14, ""max"": 19583.33, ""mdn"": 19164.79, ""min"": 18485.09, ""timestamp"": 1730910042},
                        {""avg"": 19272.39, ""max"": 19797.2, ""mdn"": 19044.75, ""min"": 18845.86, ""timestamp"": 1730910342},
                        {""avg"": 18813.31, ""max"": 19066.52, ""mdn"": 18786.91, ""min"": 18580.32, ""timestamp"": 1730910642},
                        {""avg"": 19076.83, ""max"": 19229.23, ""mdn"": 19101.57, ""min"": 18899.41, ""timestamp"": 1730910942},
                        {""avg"": 19133.7, ""max"": 19299.13, ""mdn"": 19243.12, ""min"": 18779.64, ""timestamp"": 1730911242}
                    ]
                },
                ""pm4"": {
                    ""measurements"": [
                        {""avg"": 23238.81, ""max"": 23965.55, ""mdn"": 23318.23, ""min"": 22426.75, ""timestamp"": 1730909742},
                        {""avg"": 23508.69, ""max"": 24199.58, ""mdn"": 23473.02, ""min"": 22357.65, ""timestamp"": 1730910042},
                        {""avg"": 23644.9, ""max"": 24504.67, ""mdn"": 23263.98, ""min"": 22941.97, ""timestamp"": 1730910342},
                        {""avg"": 22892.78, ""max"": 23336.15, ""mdn"": 22856.84, ""min"": 22460.98, ""timestamp"": 1730910642},
                        {""avg"": 23273.08, ""max"": 23519.91, ""mdn"": 23295.33, ""min"": 22964.19, ""timestamp"": 1730910942},
                        {""avg"": 23353.34, ""max"": 23609.49, ""mdn"": 23554.29, ""min"": 22745.72, ""timestamp"": 1730911242}
                    ]
                },
                ""pm10"": {
                    ""measurements"": [
                        {""avg"": 25364.67, ""max"": 26227.86, ""mdn"": 25472.64, ""min"": 24401.82, ""timestamp"": 1730909742},
                        {""avg"": 25680.22, ""max"": 26512.77, ""mdn"": 25631.87, ""min"": 24298.17, ""timestamp"": 1730910042},
                        {""avg"": 25835.96, ""max"": 26863.57, ""mdn"": 25378.22, ""min"": 24994.51, ""timestamp"": 1730910342},
                        {""avg"": 24937, ""max"": 25475.65, ""mdn"": 24896.27, ""min"": 24405.57, ""timestamp"": 1730910642},
                        {""avg"": 25375.8, ""max"": 25669.95, ""mdn"": 25396.8, ""min"": 25001.04, ""timestamp"": 1730910942},
                        {""avg"": 25467.78, ""max"": 25769.4, ""mdn"": 25714.6, ""min"": 24733.1, ""timestamp"": 1730911242}
                    ]
                }
            },
            ""system"": {
                ""current_load"": 35,
                ""uptime"": 610810,
                ""voltage_load"": 3.54,
                ""voltage_rest"": 3.58
            },
            ""thermometer"": {
                ""temperature"": 2.81
            },
            ""w1_thermometers"": []
        }";

        [Fact]
        public void ShouldDeserializeChesterDustSps30CloudMessage()
        {
            // Act
            ChesterDustSps30CloudMessage? message = JsonSerializer.Deserialize<ChesterDustSps30CloudMessage>(testJson);

            // Assert
            Assert.NotNull(message);
            Assert.Equal("v2.0.0", message.Attribute.FwVersion);
            Assert.Equal("HARDWARIO", message.Attribute.VendorName);
            Assert.Equal("CHESTER Dust", message.Attribute.FwName);

            // Test Dust Sensor Data
            Assert.NotNull(message.DustSensor);

            // Num_PM_0_5
            var num_pm_0_5 = message.DustSensor.Num_PM_0_5.Measurements;
            Assert.Equal(6, num_pm_0_5.Count);
            Assert.Equal(76720.24, num_pm_0_5[0].Avg);
            Assert.Equal(77018.25, num_pm_0_5[0].Max);

            // Num_PM1
            var num_pm1 = message.DustSensor.Num_PM1.Measurements;
            Assert.Equal(6, num_pm1.Count);
            Assert.Equal(97791.53, num_pm1[0].Avg);
            Assert.Equal(98315.52, num_pm1[0].Max);

            // Num_PM_2_5
            var num_pm_2_5 = message.DustSensor.Num_PM_2_5.Measurements;
            Assert.Equal(6, num_pm_2_5.Count);
            Assert.Equal(103643.88, num_pm_2_5[0].Avg);
            Assert.Equal(104524, num_pm_2_5[0].Max);

            // Num_PM4
            var num_pm4 = message.DustSensor.Num_PM4.Measurements;
            Assert.Equal(6, num_pm4.Count);
            Assert.Equal(104720.17, num_pm4[0].Avg);
            Assert.Equal(105667.99, num_pm4[0].Max);

            // Num_PM_10
            var num_pm_10 = message.DustSensor.Num_PM_10.Measurements;
            Assert.Equal(6, num_pm_10.Count);
            Assert.Equal(104967.51, num_pm_10[0].Avg);
            Assert.Equal(105930.1, num_pm_10[0].Max);

            // PM_1
            var pm_1 = message.DustSensor.PM_1.Measurements;
            Assert.Equal(6, pm_1.Count);
            Assert.Equal(13032.99, pm_1[0].Avg);
            Assert.Equal(13146.25, pm_1[0].Max);

            // PM_2_5
            var pm_2_5 = message.DustSensor.PM_2_5.Measurements;
            Assert.Equal(6, pm_2_5.Count);
            Assert.Equal(18996.38, pm_2_5[0].Avg);
            Assert.Equal(19450.83, pm_2_5[0].Max);

            // PM_4
            var pm_4 = message.DustSensor.PM_4.Measurements;
            Assert.Equal(6, pm_4.Count);
            Assert.Equal(23238.81, pm_4[0].Avg);
            Assert.Equal(23965.55, pm_4[0].Max);

            // PM_10
            var pm_10 = message.DustSensor.PM_10.Measurements;
            Assert.Equal(6, pm_10.Count);
            Assert.Equal(25364.67, pm_10[0].Avg);
            Assert.Equal(26227.86, pm_10[0].Max);




        }
    }
}
