using hio_dotnet.Common.Config;
using hio_dotnet.Common.Enums;
using hio_dotnet.Common.Enums.LTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Tests.Common
{
    public class LTEConfigTests
    {
        [Fact]
        public void ParseLine_ShouldParseValidBoolTestConfig()
        {
            var config = new LTEConfig();
            config.ParseLine("lte config test true");

            Assert.True(config.Test);
        }

        [Fact]
        public void ParseLine_ShouldParseValidEnumAntennaConfig()
        {
            var config = new LTEConfig();
            config.ParseLine("lte config antenna external");

            Assert.Equal(AntennaType.External, config.Antenna);

            config = new LTEConfig();
            config.ParseLine("lte config antenna ext");

            Assert.Equal(AntennaType.External, config.Antenna);

            config = new LTEConfig();
            config.Antenna = AntennaType.External;
            config.ParseLine("lte config antenna internal");

            Assert.Equal(AntennaType.Internal, config.Antenna);

            config = new LTEConfig();
            config.Antenna = AntennaType.External;
            config.ParseLine("lte config antenna int");

            Assert.Equal(AntennaType.Internal, config.Antenna);
        }

        [Fact]
        public void ParseLine_ShouldParseValidIntPlmnIdConfig()
        {
            var config = new LTEConfig();
            config.ParseLine("lte config plmnid 23003");

            Assert.Equal(23003, config.PlmnId);
        }

        [Fact]
        public void ParseLine_ShouldParseValidStringApnConfig()
        {
            var config = new LTEConfig();
            config.ParseLine("lte config apn testapn");

            Assert.Equal("testapn", config.Apn);
        }

        [Fact]
        public void ParseLine_ShouldThrowExceptionOnInvalidLine()
        {
            var config = new LTEConfig();

            Assert.Throws<ArgumentException>(() => config.ParseLine("lrw config adr true"));
        }

        [Fact]
        public void ParseLine_ShouldHandleEmptyString()
        {
            var config = new LTEConfig();
            Assert.Throws<ArgumentNullException>(() => config.ParseLine(""));
        }

        [Fact]
        public void ParseLine_ShouldHandleNullString()
        {
            var config = new LTEConfig();
            Assert.Throws<ArgumentNullException>(() => config.ParseLine(null));
        }

        [Fact]
        public void ParseLine_ShouldHandleInvalidEnumValueGracefully()
        {
            var config = new LTEConfig();
            Assert.Throws<InvalidOperationException>(() => config.ParseLine("lte config antenna unknownvalue"));
        }

        [Fact]
        public void ParseLine_ShouldHandleInvalidBooleanValueGracefully()
        {
            var config = new LTEConfig();
            Assert.Throws<FormatException>(() => config.ParseLine("lte config test notabool"));
        }

        [Fact]
        public void ParseLine_ShouldHandleInvalidIntegerValueGracefully()
        {
            var config = new LTEConfig();
            Assert.Throws<FormatException>(() => config.ParseLine("lte config plmnid notanint"));
        }

        [Fact]
        public void ParseAllLines()
        {
            var config = new LTEConfig();
            config.ParseLine("lte config test true");
            Assert.Equal(config.Test, true);

            config.ParseLine("lte config nb-iot-mode true");
            Assert.Equal(config.NbIotMode, true);

            config.ParseLine("lte config lte-m-mode true");
            Assert.Equal(config.LteMMode, true);

            config.ParseLine("lte config autoconn true");
            Assert.Equal(config.AutoConn, true);

            config.ParseLine("lte config clksync true");
            Assert.Equal(config.ClkSync, true);

            config.ParseLine("lte config plmnid 1234");
            Assert.Equal(config.PlmnId, 1234);

            config.ParseLine("lte config port 4321");
            Assert.Equal(config.Port, 4321);

            config.ParseLine("lte config antenna ext");
            Assert.Equal(config.Antenna, AntennaType.External);

            config.ParseLine("lte config auth chap");
            Assert.Equal(config.Authorization, LTEAuthType.CHAP);

            config.ParseLine("lte config apn hardwario");
            Assert.Equal(config.Apn, "hardwario");

            config.ParseLine("lte config username name");
            Assert.Equal(config.Username, "name");

            config.ParseLine("lte config password pass");
            Assert.Equal(config.Password, "pass");

            config.ParseLine("lte config addr 127.0.0.1");
            Assert.Equal(config.Address, "127.0.0.1");
        }

        [Fact]
        public void GetWholeConfig_ShouldHandleDefaultValues()
        {
            var config = new LTEConfig();
            var expectedConfig = "lte config test false" + Environment.NewLine +
                                 "lte config nb-iot-mode false" + Environment.NewLine +
                                 "lte config lte-m-mode true" + Environment.NewLine +
                                 "lte config autoconn false" + Environment.NewLine +
                                 "lte config clksync false" + Environment.NewLine +
                                 "lte config plmnid 0" + Environment.NewLine +
                                 "lte config port 0" + Environment.NewLine +
                                 "lte config antenna int" + Environment.NewLine +
                                 "lte config auth none" + Environment.NewLine +
                                 "lte config apn default_apn" + Environment.NewLine +
                                 "lte config username " + Environment.NewLine +
                                 "lte config password " + Environment.NewLine +
                                 "lte config addr 0.0.0.0";

            Assert.Equal(expectedConfig, config.GetWholeConfig().Trim());
        }

        [Fact]
        public void FluentInterface_ShouldSetPropertiesCorrectly()
        {
            var config = new LTEConfig()
                .WithTest(true)
                .WithAntenna(AntennaType.External)
                .WithNbIotMode(true)
                .WithLteMMode(false)
                .WithAutoConn(true)
                .WithPlmnId(23003)
                .WithApn("testapn")
                .WithAuth(LTEAuthType.CHAP)
                .WithUsername("testuser")
                .WithPassword("password123")
                .WithAddress("192.168.1.1")
                .WithPort(8080);

            Assert.True(config.Test);
            Assert.Equal(AntennaType.External, config.Antenna);
            Assert.True(config.NbIotMode);
            Assert.False(config.LteMMode);
            Assert.True(config.AutoConn);
            Assert.Equal(23003, config.PlmnId);
            Assert.Equal("testapn", config.Apn);
            Assert.Equal(LTEAuthType.CHAP, config.Authorization);
            Assert.Equal("testuser", config.Username);
            Assert.Equal("password123", config.Password);
            Assert.Equal("192.168.1.1", config.Address);
            Assert.Equal(8080, config.Port);
        }

        [Fact]
        public void GetWholeConfig_ShouldGenerateCorrectConfigString()
        {
            var config = new LTEConfig()
                .WithTest(true)
                .WithAntenna(AntennaType.External)
                .WithNbIotMode(true)
                .WithLteMMode(false)
                .WithAutoConn(true)
                .WithClkSync(true)
                .WithPlmnId(23003)
                .WithApn("testapn")
                .WithAuth(LTEAuthType.PAP)
                .WithUsername("user")
                .WithPassword("pass")
                .WithAddress("192.168.1.1")
                .WithPort(8080);

            var expectedConfig = "lte config test true" + Environment.NewLine +
                                 "lte config nb-iot-mode true" + Environment.NewLine +
                                 "lte config lte-m-mode false" + Environment.NewLine +
                                 "lte config autoconn true" + Environment.NewLine +
                                 "lte config clksync true" + Environment.NewLine +
                                 "lte config plmnid 23003" + Environment.NewLine +
                                 "lte config port 8080" + Environment.NewLine +
                                 "lte config antenna ext" + Environment.NewLine +
                                 "lte config auth pap" + Environment.NewLine +
                                 "lte config apn testapn" + Environment.NewLine +
                                 "lte config username user" + Environment.NewLine +
                                 "lte config password pass" + Environment.NewLine +
                                 "lte config addr 192.168.1.1";
                                 

            Assert.Equal(expectedConfig, config.GetWholeConfig().Trim());
        }
    }
}
