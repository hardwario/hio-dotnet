using hio_dotnet.Common.Enums.LTE;
using hio_dotnet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hio_dotnet.Common.Enums.LoRaWAN;
using System.Reflection;

namespace hio_dotnet.Common.Config
{
    public class LTEConfig : ConfigCommon
    {
        #region ConnectionParams
        [ConfigSerializationAttribute(true, false, "lte", "test")]
        public bool Test { get; set; } = false;
        [ConfigSerializationAttribute(true, false, "lte", "nb-iot-mode")]
        public bool NbIotMode { get; set; } = false;
        [ConfigSerializationAttribute(true, false, "lte", "lte-m-mode")]
        public bool LteMMode { get; set; } = true;
        [ConfigSerializationAttribute(true, false, "lte", "autoconn")]
        public bool AutoConn { get; set; } = false;
        [ConfigSerializationAttribute(true, false, "lte", "clksync")]
        public bool ClkSync { get; set; } = false;
        [ConfigSerializationAttribute(true, false, "lte", "plmnid")]
        public int PlmnId { get; set; } = 0;
        [ConfigSerializationAttribute(true, false, "lte", "port")]
        public int Port { get; set; } = 0;
        [ConfigSerializationAttribute(true, false, "lte", "antenna")]
        public AntennaType Antenna { get; set; } = AntennaType.Internal;
        [ConfigSerializationAttribute(true, false, "lte", "auth")]
        public LTEAuthType Authorization { get; set; } = LTEAuthType.None;
        [ConfigSerializationAttribute(true, false, "lte", "apn")]
        public string Apn { get; set; } = "default_apn";
        [ConfigSerializationAttribute(true, false, "lte", "username")]
        public string Username { get; set; } = string.Empty;
        [ConfigSerializationAttribute(true, false, "lte", "password")]
        public string Password { get; set; } = string.Empty;
        [ConfigSerializationAttribute(true, false, "lte", "addr")]
        public string Address { get; set; } = "0.0.0.0";
        #endregion

        #region FluentInterface
        public LTEConfig WithTest(bool test)
        {
            Test = test;
            return this;
        }

        public LTEConfig WithAntenna(AntennaType antenna)
        {
            Antenna = antenna;
            return this;
        }

        public LTEConfig WithNbIotMode(bool nbIotMode)
        {
            NbIotMode = nbIotMode;
            return this;
        }

        public LTEConfig WithLteMMode(bool lteMMode)
        {
            LteMMode = lteMMode;
            return this;
        }

        public LTEConfig WithAutoConn(bool autoConn)
        {
            AutoConn = autoConn;
            return this;
        }

        public LTEConfig WithPlmnId(int plmnId)
        {
            PlmnId = plmnId;
            return this;
        }

        public LTEConfig WithClkSync(bool clkSync)
        {
            ClkSync = clkSync;
            return this;
        }

        public LTEConfig WithApn(string apn)
        {
            Apn = apn;
            return this;
        }

        public LTEConfig WithAuth(LTEAuthType auth)
        {
            Authorization = auth;
            return this;
        }

        public LTEConfig WithUsername(string username)
        {
            Username = username;
            return this;
        }

        public LTEConfig WithPassword(string password)
        {
            Password = password;
            return this;
        }

        public LTEConfig WithAddress(string address)
        {
            Address = address;
            return this;
        }

        public LTEConfig WithPort(int port)
        {
            Port = port;
            return this;
        }
        #endregion

        #region ParsingMethods

        public void ParseLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                throw new ArgumentNullException("LTE Parsing>> Cannot parse null or empty line");

            if (line.Contains("lrw "))
                throw new ArgumentException("LTE Parsing>> The config line is not for LTE. It belongs to LoRaWAN parsing.");
            if (line.Contains("ble "))
                throw new ArgumentException("LTE Parsing>> The config line is not for LTE. It belongs to BLE parsing.");
            if (line.Contains("app "))
                throw new ArgumentException("LTE Parsing>> The config line is not for LTE. It belongs to APP parsing.");

            if (line.Contains("lte config "))
                line = line.Replace("lte config ", string.Empty).ReplaceLineEndings();

            ParseLineToProp(line);
        }

        #endregion

    }
}
