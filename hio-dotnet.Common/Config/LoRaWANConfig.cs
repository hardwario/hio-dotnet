using hio_dotnet.Common.Enums.LoRaWAN;
using hio_dotnet.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace hio_dotnet.Common.Config
{
    public class LoRaWANConfig : ConfigCommon
    {
        #region ConnectionParams
        [ConfigSerializationAttribute(true, false, "lrw", "devaddr")]
        public string DevAddr { get; set; } = "00000000";
        [ConfigSerializationAttribute(true, false, "lrw", "deveui")]
        public string DevEui { get; set; } = "0000000000000000";
        [ConfigSerializationAttribute(true, false, "lrw", "joineui")]
        public string JoinEui { get; set; } = "0000000000000000";
        [ConfigSerializationAttribute(true, false, "lrw", "appkey")]
        public string AppKey { get; set; } = "00000000000000000000000000000000";
        [ConfigSerializationAttribute(true, false, "lrw", "appskey")]
        public string AppSKey { get; set; } = "00000000000000000000000000000000";
        [ConfigSerializationAttribute(true, false, "lrw", "nwkskey")]
        public string NwkSKey { get; set; } = "00000000000000000000000000000000";
        #endregion

        #region LoRaWANParams
        [ConfigSerializationAttribute(true, false, "lrw", "antenna")]
        public AntennaType Antenna { get; set; } = AntennaType.Internal;
        [ConfigSerializationAttribute(true, false, "lrw", "band")]
        public LoRaWANBand Band { get; set; } = LoRaWANBand.EU868;
        [ConfigSerializationAttribute(true, false, "lrw", "mode")]
        public LoRaWANMode Mode { get; set; } = LoRaWANMode.OTAA;
        [ConfigSerializationAttribute(true, false, "lrw", "nwk")]
        public LoRaWANNetwork Network { get; set; } = LoRaWANNetwork.Private;
        [ConfigSerializationAttribute(true, false, "lrw", "class")]
        public LoRaWANClass Class { get; set; } = LoRaWANClass.A;
        #endregion

        [ConfigSerializationAttribute(true, false, "lrw", "adr")]
        public bool Adr { get; set; } = true;
        [ConfigSerializationAttribute(true, false, "lrw", "test")]
        public bool Test { get; set; } = false;
        [ConfigSerializationAttribute(true, false, "lrw", "dutycycle")]
        public bool DutyCycle { get; set; } = false;
        [ConfigSerializationAttribute(true, false, "lrw", "datarate")]
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

            ParseLineToProp(line);
        }

    }
}
