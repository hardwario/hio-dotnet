using hio_dotnet.Common.Enums.LoRaWAN;
using hio_dotnet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace hio_dotnet.Common.Config
{
    public class LoRaWANConfig : ConfigCommon
    {
        #region ConnectionParams
        public string DevAddr { get; set; } = "00000000";
        public string DevEui { get; set; } = "0000000000000000";
        public string JoinEui { get; set; } = "0000000000000000";
        public string AppKey { get; set; } = "00000000000000000000000000000000";
        public string AppSKey { get; set; } = "00000000000000000000000000000000";
        public string NwkSKey { get; set; } = "00000000000000000000000000000000";
        #endregion

        #region LoRaWANParams
        public AntennaType Antenna { get; set; } = AntennaType.Internal;
        public LoRaWANBand Band { get; set; } = LoRaWANBand.EU868;
        public LoRaWANMode Mode { get; set; } = LoRaWANMode.OTAA;
        public LoRaWANNetwork Network { get; set; } = LoRaWANNetwork.Private;
        public LoRaWANClass Class { get; set; } = LoRaWANClass.A;
        #endregion

        public bool Adr { get; set; } = true;
        public bool Test { get; set; } = false;
        public bool DutyCycle { get; set; } = false;
        public int DataRate { get; set; } = 0;

        #region FluentInterface
        // Metody pro fluent interface
        public LoRaWANConfig WithDevAddr(string devAddr)
        {
            DevAddr = devAddr;
            return this;
        }

        public LoRaWANConfig WithDevEui(string devEui)
        {
            DevEui = devEui;
            return this;
        }

        public LoRaWANConfig WithJoinEui(string joinEui)
        {
            JoinEui = joinEui;
            return this;
        }

        public LoRaWANConfig WithAppKey(string appKey)
        {
            AppKey = appKey;
            return this;
        }

        public LoRaWANConfig WithAppSKey(string appSKey)
        {
            AppSKey = appSKey;
            return this;
        }

        public LoRaWANConfig WithNwkSKey(string nwkSKey)
        {
            NwkSKey = nwkSKey;
            return this;
        }

        public LoRaWANConfig WithAntenna(AntennaType antenna)
        {
            Antenna = antenna;
            return this;
        }

        public LoRaWANConfig WithBand(LoRaWANBand band)
        {
            Band = band;
            return this;
        }

        public LoRaWANConfig WithMode(LoRaWANMode mode)
        {
            Mode = mode;
            return this;
        }

        public LoRaWANConfig WithNetwork(LoRaWANNetwork network)
        {
            Network = network;
            return this;
        }

        public LoRaWANConfig WithClass(LoRaWANClass @class)
        {
            Class = @class;
            return this;
        }

        public LoRaWANConfig WithAdr(bool adr)
        {
            Adr = adr;
            return this;
        }

        public LoRaWANConfig WithTest(bool test)
        {
            Test = test;
            return this;
        }

        public LoRaWANConfig WithDutyCycle(bool dutyCycle)
        {
            DutyCycle = dutyCycle;
            return this;
        }

        public LoRaWANConfig WithDataRate(int dataRate)
        {
            DataRate = dataRate;
            return this;
        }
        #endregion

        public void ParseLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                throw new ArgumentNullException("LoRaWAN Parsing>> Cannot parse null or empty line");

            if (line.Contains("lte "))
                throw new ArgumentException("LoRaWAN Parsing>> The config line is not for LoRa. It belongs to LTE parsing.");
            if (line.Contains("ble "))
                throw new ArgumentException("LoRaWAN Parsing>> The config line is not for LoRa. It belongs to BLE parsing.");
            if (line.Contains("app "))
                throw new ArgumentException("LoRaWAN Parsing>> The config line is not for LoRa. It belongs to APP parsing.");


            if (line.Contains("lrw config "))
                line = line.Replace("lrw config ", string.Empty).ReplaceLineEndings();

            if (line.Contains("test "))
            {
                Test = GetBoolParameter(line);
            }
            else if (line.Contains("adr "))
            {
                Adr = GetBoolParameter(line);
            }
            else if (line.Contains("dutycycle "))
            {
                DutyCycle = GetBoolParameter(line);
            }
            else if (line.Contains("antenna "))
            {
                if (line.Contains("int") && !line.Contains("internal"))
                    line = line.Replace("int", "internal");
                if (line.Contains("ext") && !line.Contains("external"))
                    line = line.Replace("ext", "external");

                Antenna = GetEnumParameter<AntennaType>(line) ?? AntennaType.Internal;
            }
            else if (line.Contains("band "))
            {
                Band = GetEnumParameter<LoRaWANBand>(line) ?? LoRaWANBand.EU868;
            }
            else if (line.Contains("class "))
            {
                Class = GetEnumParameter<LoRaWANClass>(line) ?? LoRaWANClass.A;
            }
            else if (line.Contains("mode "))
            {
                Mode = GetEnumParameter<LoRaWANMode>(line) ?? LoRaWANMode.OTAA;
            }
            else if (line.Contains("nwk "))
            {
                Network = GetEnumParameter<LoRaWANNetwork>(line) ?? LoRaWANNetwork.Private;
            }
            else if (line.Contains("devaddr "))
            {
                DevAddr = GetStringParameter(line) ?? "00000000";
            }
            else if (line.Contains("deveui "))
            {
                DevEui = GetStringParameter(line) ?? "0000000000000000";
            }
            else if (line.Contains("joineui "))
            {
                JoinEui = GetStringParameter(line) ?? "0000000000000000";
            }
            else if (line.Contains("appkey "))
            {
                AppKey = GetStringParameter(line) ?? "00000000000000000000000000000000";
            }
            else if (line.Contains("nwkskey "))
            {
                NwkSKey = GetStringParameter(line) ?? "00000000000000000000000000000000";
            }
            else if (line.Contains("appskey "))
            {
                AppSKey = GetStringParameter(line) ?? "00000000000000000000000000000000";
            }
            else if (line.Contains("datarate "))
            {
                DataRate = GetIntParameter(line) ?? 0;
            }

        }

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
                throw new ArgumentException($"Property '{propertyName}' not found in LoRaConfig.");

            var value = propertyInfo.GetValue(this);

            if (value is bool boolValue)
            {
                paramValue = GetBoolString(boolValue);
                if (Equals(propertyName, nameof(Test))) tag = "test";
                else if (Equals(propertyName, nameof(Adr))) tag = "adr";
                else if (Equals(propertyName, nameof(DutyCycle))) tag = "dutycycle";
            }
            else if (value is int intValue)
            {
                paramValue = GetIntString(intValue);
                if (Equals(propertyName, nameof(DataRate))) tag = "datarate";
            }
            else if (value is Enum enumValue)
            {
                paramValue = GetEnumString(enumValue);

                // Přidáme logiku pro přiřazení tagu
                var enumType = enumValue.GetType();
                if (enumType == typeof(AntennaType)) tag = "antenna";
                else if (enumType == typeof(LoRaWANBand)) tag = "band";
                else if (enumType == typeof(LoRaWANClass)) tag = "class";
                else if (enumType == typeof(LoRaWANMode)) tag = "mode";
                else if (enumType == typeof(LoRaWANNetwork)) tag = "nwk";
            }
            else if (value is string stringValue)
            {
                paramValue = stringValue;
                if (Equals(propertyName, nameof(DevAddr))) tag = "devaddr";
                else if (Equals(propertyName, nameof(DevEui))) tag = "deveui";
                else if (Equals(propertyName, nameof(JoinEui))) tag = "joineui";
                else if (Equals(propertyName, nameof(AppKey))) tag = "appkey";
                else if (Equals(propertyName, nameof(NwkSKey))) tag = "nwkskey";
                else if (Equals(propertyName, nameof(AppSKey))) tag = "appskey";
            }

            if (tag == "antenna")
            {
                if (paramValue.Contains("internal"))
                    paramValue = "int";
                if (paramValue.Contains("external"))
                    paramValue = "ext";
            }

            // Vrací finální konfigurační řádek ve formátu "lrw {tag} {paramValue}"
            return $"lrw config {tag} {paramValue}";
        }

        #endregion
    }
}
