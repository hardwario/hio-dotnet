using hio_dotnet.Common.Enums.LTE;
using hio_dotnet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hio_dotnet.Common.Enums.LoRaWAN;

namespace hio_dotnet.Common.Config
{
    public class LTEConfig : ConfigCommon
    {
        #region ConnectionParams
        public bool Test { get; set; } = false;
        public bool NbIotMode { get; set; } = false;
        public bool LteMMode { get; set; } = true;
        public bool AutoConn { get; set; } = false;
        public bool ClkSync { get; set; } = false;
        public int PlmnId { get; set; } = 0;
        public int Port { get; set; } = 0;
        public AntennaType Antenna { get; set; } = AntennaType.Internal;
        public LTEAuthType Authorization { get; set; } = LTEAuthType.None;
        public string Apn { get; set; } = "default_apn";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
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

            if (line.Contains("test "))
            {
                Test = GetBoolParameter(line);
            }
            else if (line.Contains("antenna "))
            {
                if (line.Contains("int") && !line.Contains("internal"))
                    line = line.Replace("int", "internal");
                if (line.Contains("ext") && !line.Contains("external"))
                    line = line.Replace("ext", "external");

                Antenna = GetEnumParameter<AntennaType>(line) ?? AntennaType.Internal;
            }
            else if (line.Contains("nb-iot-mode "))
            {
                NbIotMode = GetBoolParameter(line);
            }
            else if (line.Contains("lte-m-mode "))
            {
                LteMMode = GetBoolParameter(line);
            }
            else if (line.Contains("autoconn "))
            {
                AutoConn = GetBoolParameter(line);
            }
            else if (line.Contains("plmnid "))
            {
                PlmnId = GetIntParameter(line) ?? 0;
            }
            else if (line.Contains("clksync "))
            {
                ClkSync = GetBoolParameter(line);
            }
            else if (line.Contains("apn "))
            {
                Apn = GetStringParameter(line) ?? "default_apn";
            }
            else if (line.Contains("auth "))
            {
                Authorization = GetEnumParameter<LTEAuthType>(line) ?? LTEAuthType.None;
            }
            else if (line.Contains("username "))
            {
                Username = GetStringParameter(line) ?? string.Empty;
            }
            else if (line.Contains("password "))
            {
                Password = GetStringParameter(line) ?? string.Empty;
            }
            else if (line.Contains("addr "))
            {
                Address = GetStringParameter(line) ?? "0.0.0.0";
            }
            else if (line.Contains("port "))
            {
                Port = GetIntParameter(line) ?? 0;
            }
        }

        #endregion

        #region GenerateFunctions
        public string GetWholeConfig()
        {
            var stringBuilder = new StringBuilder();

            var properties = this.GetType().GetProperties();

            foreach (var property in properties)
            {
                var configLine = GetConfigLine(property.Name);

                if (!string.IsNullOrWhiteSpace(configLine))
                {
                    stringBuilder.AppendLine(configLine);
                }
            }

            return stringBuilder.ToString();
        }

        public string GetConfigLine(string propertyName)
        {
            string tag = string.Empty;
            string paramValue = string.Empty;

            var propertyInfo = this.GetType().GetProperty(propertyName);

            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found in LTEConfig.");

            var value = propertyInfo.GetValue(this);

            if (value is bool boolValue)
            {
                paramValue = GetBoolString(boolValue);

                if (Equals(propertyName, nameof(Test))) tag = "test";
                else if (Equals(propertyName, nameof(NbIotMode))) tag = "nb-iot-mode";
                else if (Equals(propertyName, nameof(LteMMode))) tag = "lte-m-mode";
                else if (Equals(propertyName, nameof(AutoConn))) tag = "autoconn";
                else if (Equals(propertyName, nameof(ClkSync))) tag = "clksync";
            }
            else if (value is int intValue)
            {
                paramValue = GetIntString(intValue);
                
                if (Equals(propertyName, nameof(PlmnId))) tag = "plmnid";
                else if (Equals(propertyName, nameof(Port))) tag = "port";
            }
            else if (value is Enum enumValue)
            {
                paramValue = GetEnumString(enumValue);
                var enumType = enumValue.GetType();
                if (enumType == typeof(AntennaType)) tag = "antenna";
                else if (enumType == typeof(LTEAuthType)) tag = "auth";
            }
            else if (value is string stringValue)
            {
                paramValue = stringValue;
                if (Equals(propertyName, nameof(Apn))) tag = "apn";
                else if (Equals(propertyName, nameof(Username))) tag = "username";
                else if (Equals(propertyName, nameof(Password))) tag = "password";
                else if (Equals(propertyName, nameof(Address))) tag = "addr";
            }

            if (tag == "antenna")
            {
                if (paramValue.Contains("internal"))
                    paramValue = "int";
                if (paramValue.Contains("external"))
                    paramValue = "ext";
            }

            return $"lte config {tag} {paramValue}";
        }

        #endregion
    }
}
