using hio_dotnet.Common.Enums;
using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.CatalogApps.ClimeIAQ;
using hio_dotnet.Common.Models.CatalogApps.Control;
using hio_dotnet.Common.Models.CatalogApps.Counter;
using hio_dotnet.Common.Models.CatalogApps.Current;
using hio_dotnet.Common.Models.CatalogApps.Input;
using hio_dotnet.Common.Models.CatalogApps.Meteo;
using hio_dotnet.Common.Models.CatalogApps.Radon;
using hio_dotnet.Common.Models.CatalogApps.Range;
using hio_dotnet.Common.Models.CatalogApps.Scale;
using hio_dotnet.Common.Models.CatalogApps.wMBus;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.CatalogApps
{
    public static class ChesterCloudMessageFactory
    {
        /// <summary>
        /// Get the empty message for the device type
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static IChesterCommonCloudMessage? GetChesterEmptyMessage(DeviceType deviceType)
        {
            switch (deviceType)
            {
                case DeviceType.None:
                    return new ChesterCommonCloudMessage();
                case DeviceType.CHESTER_CLIME:
                    return new ChesterClimeCloudMessage();
                case DeviceType.CHESTER_CLIME_IAQ:
                    return new ChesterClimeIAQCloudMessage();
                case DeviceType.CHESTER_CONTROL:
                    return new ChesterControlCloudMessage();
                case DeviceType.CHESTER_COUNTER:
                    return new ChesterCounterCloudMessage();
                case DeviceType.CHESTER_CURRENT:
                    return new ChesterCurrentCloudMessage();
                case DeviceType.CHESTER_INPUT:
                    return new ChesterInputCloudMessage();
                case DeviceType.CHESTER_METEO:
                    return new ChesterMeteoCloudMessage();
                case DeviceType.CHESTER_RADON:
                    return new ChesterRadonCloudMessage();
                case DeviceType.CHESTER_RANGE:
                    return new ChesterRangeCloudMessage();
                case DeviceType.CHESTER_SCALE:
                    return new ChesterScaleCloudMessage();
                case DeviceType.CHESTER_WMBUS:
                    return new ChesterWMBusCloudMessage();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get Chester message with simulated data based on the device type
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static IChesterCommonCloudMessage? GetChesterFakeMessage(DeviceType deviceType)
        {
            var message = GetChesterEmptyMessage(deviceType);
            if (message == null)
                return null;
            BaseSimulator.GetSimulatedData(message);

            return message;
        }
    }
}
