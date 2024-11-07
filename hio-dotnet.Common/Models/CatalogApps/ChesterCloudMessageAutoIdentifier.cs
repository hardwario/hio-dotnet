using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.CatalogApps.ClimeIAQ;
using hio_dotnet.Common.Models.CatalogApps.Current;
using hio_dotnet.Common.Models.CatalogApps.Dust;
using hio_dotnet.Common.Models.CatalogApps.Input;
using hio_dotnet.Common.Models.CatalogApps.Meteo;
using hio_dotnet.Common.Models.CatalogApps.Push;
using hio_dotnet.Common.Models.CatalogApps.Radon;
using hio_dotnet.Common.Models.CatalogApps.Range;
using hio_dotnet.Common.Models.CatalogApps.wMBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps
{
    public static class ChesterCloudMessageAutoIdentifier
    {
        public static Type FindTypeByMessageStructure(string message)
        {
            if (message.Contains("CHESTER Push") ||
               (message.Contains("button_1") && message.Contains("button_2")))
            {
                return typeof(ChesterPushCloudMessage);
            }
            else if (message.Contains("CHESTER Clime"))
            {
                return typeof(ChesterClimeCloudMessage);
            }
            else if (message.Contains("CHESTER Clime IAQ") ||
                    message.Contains("iaq_sensor"))
            {
                return typeof(ChesterClimeIAQCloudMessage);
            }
            else if (message.Contains("CHESTER Current") ||
                    message.Contains("analog_channels"))
            {
                return typeof(ChesterCurrentCloudMessage);
            }
            else if (message.Contains("CHESTER Input"))
            {
                return typeof(ChesterInputCloudMessage);
            }
            else if (message.Contains("CHESTER Meteo") ||
                     message.Contains("weather_station"))
            {
                return typeof(ChesterMeteoCloudMessage);
            }
            else if (message.Contains("CHESTER Range") ||
                     message.Contains("ultrasonic_ranger"))
            {
                return typeof(ChesterRangeCloudMessage);
            }
            else if (message.Contains("CHESTER Radon") ||
                     message.Contains("radon_probe"))
            {
                return typeof(ChesterRadonCloudMessage);
            }
            else if (message.Contains("CHESTER wmBus") ||
                     message.Contains("wmbus"))
            {
                return typeof(ChesterWMBusCloudMessage);
            }
            else if (message.Contains("CHESTER Dust") ||
                     message.Contains("sps30"))
            {
                return typeof(ChesterDustCloudMessage);
            }

            return typeof(ChesterCommonCloudMessage);
        }
    }
}
