using hio_dotnet.Common.Config;
using hio_dotnet.Common.Enums;
using hio_dotnet.Common.Enums.LoRaWAN;
using Xunit;
namespace hio_dotnet.Tests.Common
{
    public class LoRaConfigTests
    {
        [Fact]
        public void ParseLine_ShouldParseValidBoolAdrConfig()
        {
            var config = new LoRaWANConfig();
            config.ParseLine("lrw config adr true");

            Assert.True(config.Adr);
        }

        [Fact]
        public void ParseLine_ShouldParseValidEnumClassConfig()
        {
            var config = new LoRaWANConfig();
            config.ParseLine("lrw config class c");

            Assert.Equal(LoRaWANClass.C, config.Class);
        }

        [Fact]
        public void ParseLine_ShouldParseValidStringDevAddrConfig()
        {
            var config = new LoRaWANConfig();
            config.ParseLine("lrw config devaddr 12345678");

            Assert.Equal("12345678", config.DevAddr);
        }

        [Fact]
        public void ParseLine_ShouldThrowExceptionOnInvalidLine()
        {
            var config = new LoRaWANConfig();

            Assert.Throws<ArgumentException>(() => config.ParseLine("lte config test true"));
        }

        [Fact]
        public void FluentInterface_ShouldSetPropertiesCorrectly()
        {
            var config = new LoRaWANConfig()
                .WithDevAddr("12345678")
                .WithDevEui("ABCDEF1234567890")
                .WithJoinEui("1234567890ABCDEF")
                .WithAppKey("1234567890ABCDEF1234567890ABCDEF")
                .WithAppSKey("ABCDEF1234567890ABCDEF1234567890")
                .WithNwkSKey("0987654321FEDCBA0987654321FEDCBA")
                .WithAdr(true)
                .WithDataRate(5);

            Assert.Equal("12345678", config.DevAddr);
            Assert.Equal("ABCDEF1234567890", config.DevEui);
            Assert.Equal("1234567890ABCDEF", config.JoinEui);
            Assert.Equal("1234567890ABCDEF1234567890ABCDEF", config.AppKey);
            Assert.True(config.Adr);
            Assert.Equal(5, config.DataRate);
        }

        [Fact]
        public void ParseLine_ShouldHandleEmptyString()
        {
            var config = new LoRaWANConfig();
            Assert.Throws<ArgumentNullException>(() => config.ParseLine(""));
        }

        [Fact]
        public void ParseLine_ShouldHandleNullString()
        {
            var config = new LoRaWANConfig();
            Assert.Throws<ArgumentNullException>(() => config.ParseLine(null));
        }

        [Fact]
        public void ParseLine_ShouldHandleInvalidEnumValueGracefully()
        {
            var config = new LoRaWANConfig();
            Assert.Throws<InvalidOperationException>(() => config.ParseLine("lrw config class unknownvalue"));
        }

        [Fact]
        public void ParseLine_ShouldHandleInvalidBooleanValueGracefully()
        {
            var config = new LoRaWANConfig();
            Assert.Throws<FormatException>(() => config.ParseLine("lrw config adr notabool"));
        }

        [Fact]
        public void ParseLine_ShouldHandleInvalidIntegerValueGracefully()
        {
            var config = new LoRaWANConfig();
            Assert.Throws<FormatException>(() => config.ParseLine("lrw config datarate notanint"));
        }

        [Fact]
        public void GetWholeConfig_ShouldHandleDefaultValues()
        {
            var config = new LoRaWANConfig();
            var expectedConfig = "lrw config devaddr 00000000" + Environment.NewLine +
                                 "lrw config deveui 0000000000000000" + Environment.NewLine +
                                 "lrw config joineui 0000000000000000" + Environment.NewLine +
                                 "lrw config appkey 00000000000000000000000000000000" + Environment.NewLine +
                                 "lrw config appskey 00000000000000000000000000000000" + Environment.NewLine +
                                 "lrw config nwkskey 00000000000000000000000000000000" + Environment.NewLine +
                                 "lrw config antenna internal" + Environment.NewLine +
                                 "lrw config band eu868" + Environment.NewLine +
                                 "lrw config mode otaa" + Environment.NewLine +
                                 "lrw config nwk private" + Environment.NewLine +
                                 "lrw config class a" + Environment.NewLine +
                                 "lrw config adr true" + Environment.NewLine +
                                 "lrw config test false" + Environment.NewLine +
                                 "lrw config dutycycle false" + Environment.NewLine +
                                 "lrw config datarate 0";

            Assert.Equal(expectedConfig, config.GetWholeConfig().Trim());
        }

        [Fact]
        public void GetWholeConfig_ShouldGenerateCorrectConfigString()
        {
            var config = new LoRaWANConfig()
                .WithDevAddr("12345678")
                .WithDevEui("87654321")
                .WithJoinEui("1111111111")
                .WithAppKey("2222222222")
                .WithAppSKey("3333333333")
                .WithNwkSKey("4444444444")
                .WithAntenna(AntennaType.External)
                .WithBand(LoRaWANBand.US915)
                .WithMode(LoRaWANMode.ABP)
                .WithNetwork(LoRaWANNetwork.Public)
                .WithClass(LoRaWANClass.C)
                .WithDutyCycle(true)
                .WithTest(true)
                .WithAdr(true)
                .WithDataRate(3);

            var expectedConfig = "lrw config devaddr 12345678" + Environment.NewLine +
                                 "lrw config deveui 87654321" + Environment.NewLine +
                                 "lrw config joineui 1111111111" + Environment.NewLine +
                                 "lrw config appkey 2222222222" + Environment.NewLine +
                                 "lrw config appskey 3333333333" + Environment.NewLine +
                                 "lrw config nwkskey 4444444444" + Environment.NewLine +
                                 "lrw config antenna external" + Environment.NewLine +
                                 "lrw config band us915" + Environment.NewLine +
                                 "lrw config mode abp" + Environment.NewLine +
                                 "lrw config nwk public" + Environment.NewLine +
                                 "lrw config class c" + Environment.NewLine +
                                 "lrw config adr true" + Environment.NewLine +
                                 "lrw config test true" + Environment.NewLine +
                                 "lrw config dutycycle true" + Environment.NewLine +
                                 "lrw config datarate 3";

            Assert.Equal(expectedConfig, config.GetWholeConfig().Trim());
        }
    }
}