using hio_dotnet.Common.Enums;
using hio_dotnet.Common.Models.CatalogApps.Clime;
using hio_dotnet.Common.Models.CatalogApps.ClimeIAQ;
using hio_dotnet.Common.Models.CatalogApps.Control;
using hio_dotnet.Common.Models.CatalogApps.Counter;
using hio_dotnet.Common.Models.CatalogApps.Current;
using hio_dotnet.Common.Models.CatalogApps.Dust;
using hio_dotnet.Common.Models.CatalogApps.Input;
using hio_dotnet.Common.Models.CatalogApps.Meteo;
using hio_dotnet.Common.Models.CatalogApps.Motion;
using hio_dotnet.Common.Models.CatalogApps.Push;
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
                case DeviceType.FAKE_CHESTER_CLIME:
                    return new ChesterClimeCloudMessage();
                case DeviceType.CHESTER_CLIME_IAQ:
                    return new ChesterClimeIAQCloudMessage();
                case DeviceType.FAKE_CHESTER_CLIME_IAQ:
                    return new ChesterClimeIAQCloudMessage();
                case DeviceType.CHESTER_CONTROL:
                    return new ChesterControlCloudMessage();
                case DeviceType.FAKE_CHESTER_CONTROL:
                    return new ChesterControlCloudMessage();
                case DeviceType.CHESTER_COUNTER:
                    return new ChesterCounterCloudMessage();
                case DeviceType.FAKE_CHESTER_COUNTER:
                    return new ChesterCounterCloudMessage();
                case DeviceType.CHESTER_CURRENT:
                    return new ChesterCurrentCloudMessage();
                case DeviceType.FAKE_CHESTER_CURRENT:
                    return new ChesterCurrentCloudMessage();
                case DeviceType.CHESTER_INPUT:
                    return new ChesterInputCloudMessage();
                case DeviceType.FAKE_CHESTER_INPUT:
                    return new ChesterInputCloudMessage();
                case DeviceType.CHESTER_METEO:
                    return new ChesterMeteoCloudMessage();
                case DeviceType.FAKE_CHESTER_METEO:
                    return new ChesterMeteoCloudMessage();
                case DeviceType.CHESTER_RADON:
                    return new ChesterRadonCloudMessage();
                case DeviceType.FAKE_CHESTER_RADON:
                    return new ChesterRadonCloudMessage();
                case DeviceType.CHESTER_RANGE:
                    return new ChesterRangeCloudMessage();
                case DeviceType.FAKE_CHESTER_RANGE:
                    return new ChesterRangeCloudMessage();
                case DeviceType.CHESTER_SCALE:
                    return new ChesterScaleCloudMessage();
                case DeviceType.FAKE_CHESTER_SCALE:
                    return new ChesterScaleCloudMessage();
                case DeviceType.CHESTER_WMBUS:
                    return new ChesterWMBusCloudMessage();
                case DeviceType.FAKE_CHESTER_WMBUS:
                    return new ChesterWMBusCloudMessage();
                case DeviceType.CHESTER_MOTION:
                    return new ChesterMotionCloudMessage();
                case DeviceType.FAKE_CHESTER_MOTION:
                    return new ChesterMotionCloudMessage();
                case DeviceType.CHESTER_DUST:
                    return new ChesterDustSps30CloudMessage();
                case DeviceType.FAKE_CHESTER_DUST:
                    return new ChesterDustSps30CloudMessage();
                case DeviceType.CHESTER_PUSH:
                    return new ChesterPushCloudMessage();
                case DeviceType.FAKE_CHESTER_PUSH:
                    return new ChesterPushCloudMessage();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get List of all types in the factory.
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllPossibleTypes()
        {
            List<Type> types = new List<Type>();
            DeviceType type = DeviceType.None;
            foreach (var t in Enum.GetValues(typeof(DeviceType)))
            {
                type = (DeviceType)t;
                if (type == DeviceType.None)
                    continue;
                var c = GetChesterEmptyMessage(type);
                if (c != null)
                {
                    if (!types.Contains(c.GetType()))
                        types.Add(c.GetType());
                }

            }

            return types;
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
