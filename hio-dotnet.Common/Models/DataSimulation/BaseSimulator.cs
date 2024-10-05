﻿using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public class BaseSimulator
    {
        public static void GetSimulatedData(object obj, object? previousObj = null)
        {
            Simulate(obj, previousObj);

            // add name of the FW and other static basic common info
            FillCommonStatics(obj, previousObj);
        }

        public static void Simulate(object obj, object? previousObj = null)
        {
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();
                
                if (simulationAttr != null && !simulationAttr.IsStatic &&
                    property.GetCustomAttribute<SimulationMeasurementAttribute>() == null &&
                    (property.PropertyType.IsPrimitive || Nullable.GetUnderlyingType(property.PropertyType)?.IsPrimitive == true))
                {
                    var isList = false;
                    if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        isList = true;
                    }

                    if (!isList)
                        FillRandomValue(simulationAttr, obj, property, previousObj);
                }

                else if (simulationAttr != null &&
                         property.PropertyType.IsClass &&
                         property.PropertyType != typeof(string) &&
                         !property.PropertyType.IsPrimitive &&
                         !simulationAttr.IsStatic)// &&
                         //property.GetValue(obj) != null)
                {
                    
                    var nestedObj = property.GetValue(obj);
                    if (nestedObj == null)
                    {
                        nestedObj = Activator.CreateInstance(property.PropertyType);
                        //property.SetValue(obj, nestedObj);
                    }
                    var previousNestedObj = previousObj?.GetType().GetProperty(property.Name)?.GetValue(previousObj);

                    //check if the nested object is a MeasurementGroup and if yes use SimulateMeasurementGroup function
                    if (nestedObj.GetType() == typeof(MeasurementGroup))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            SimulateMeasurementGroup(nestedObj, simulationMeasurementAttr, previousNestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<W1_Thermometer>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                // create a new W1_Thermometer object
                                
                                var w1t = new W1_Thermometer();
                                W1_Thermometer w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    w1tp = ((List<W1_Thermometer>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate_W1_Thermoemter(w1t, simulationMeasurementAttr, w1tp);

                                ((List<W1_Thermometer>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<BLE_Tag>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                // create a new BLE_Tag object
                                var blt = new BLE_Tag();
                                BLE_Tag bltp = null;
                                if (previousNestedObj != null)
                                {
                                    bltp = ((List<BLE_Tag>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate(blt, bltp);

                                if (bltp != null)
                                {
                                    blt.Addr = bltp.Addr;
                                }
                                else
                                {
                                    // create random BLE address in format like BA0987654321
                                    var random = new Random();
                                    var addr = new StringBuilder();
                                    addr.Append("BA");
                                    for (int j = 0; j < 10; j++)
                                    {
                                        addr.Append(random.Next(0, 9));
                                    }
                                    blt.Addr = addr.ToString();
                                }

                                ((List<BLE_Tag>)nestedObj).Add(blt);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else
                    {
                        Simulate(nestedObj, previousNestedObj);
                    }
                }
            }
        }

        public static void SimulateMeasurementGroup(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null)
        {
            if (obj.GetType() != typeof(MeasurementGroup))
            {
                return;
            }
            if (previousObj != null) {
                if (previousObj.GetType() != typeof(MeasurementGroup))
                {
                    return;
                }
            }

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (property != null && property.PropertyType == typeof(List<Measurement>))
                    {
                        FillMeasurementList(obj, simulationMeasurementAttr, property, previousObj);
                    }
                }
            }
        }

        public static void Simulate_W1_Thermoemter(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null)
        {
            if (obj.GetType() != typeof(W1_Thermometer))
            {
                return;
            }
            if (previousObj != null)
            {
                if (previousObj.GetType() != typeof(W1_Thermometer))
                {
                    return;
                }
            }

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (property != null && property.PropertyType == typeof(List<Measurement>))
                    {
                        FillMeasurementList(obj, simulationMeasurementAttr, property, previousObj);
                    }
                }
            }
        }

        public static void FillMeasurementList(object obj, 
                                               SimulationMeasurementAttribute simulationMeasurementAttr, 
                                               PropertyInfo? property = null, 
                                               object? previousObj = null)
        {
            // add new item to the list
            var list = (List<Measurement>)property.GetValue(obj);
            if (list != null)
            {
                var listitem = new Measurement();
                Measurement? previousMeasurementObj = null;

                // if previousObj is not null check the existence of the List<Measurement> and if it has any item take it as previousMeasurementObj
                if (previousObj != null)
                {
                    var previousList = (List<Measurement>)property.GetValue(previousObj);
                    if (previousList != null && previousList.Count > 0)
                    {
                        previousMeasurementObj = previousList[0];
                    }
                }

                var propertiesMeasurement = typeof(Measurement).GetProperties();

                var avgProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "Avg");
                var mdnProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "Mdn");
                var minProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "Min");
                var maxProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "Max");

                var avgPropAttr = avgProp?.GetCustomAttribute<SimulationAttribute>();
                if (avgPropAttr != null)
                {
                    avgPropAttr.MinValue = simulationMeasurementAttr.MinValue;
                    avgPropAttr.MaxValue = simulationMeasurementAttr.MaxValue;
                    avgPropAttr.NeedsFollowPrevious = simulationMeasurementAttr.NeedsFollowPrevious;
                    avgPropAttr.ShouldRaise = simulationMeasurementAttr.ShouldRaise;
                    avgPropAttr.MaximumChange = simulationMeasurementAttr.MaximumChange;
                    FillRandomValue(avgPropAttr, listitem, avgProp, previousMeasurementObj);
                }

                var random = new Random();
                //now calculate the min and max values with some random spread out of the avg value min to minus value and max to plus value
                var randomSpread = listitem.Avg * random.NextDouble() * 0.1;
                listitem.Mdn = listitem.Avg - randomSpread;
                listitem.Min = listitem.Avg - (randomSpread * 2);
                listitem.Max = listitem.Avg + (randomSpread * 2);

                if (listitem.Max > simulationMeasurementAttr.MaxValue)
                {
                    listitem.Max = simulationMeasurementAttr.MaxValue;
                }
                if (listitem.Min < simulationMeasurementAttr.MinValue)
                {
                    listitem.Min = simulationMeasurementAttr.MinValue;
                }

                if (listitem.Mdn > simulationMeasurementAttr.MaxValue)
                {
                    listitem.Mdn = simulationMeasurementAttr.MaxValue;
                }
                if (listitem.Mdn < simulationMeasurementAttr.MinValue)
                {
                    listitem.Mdn = simulationMeasurementAttr.MinValue;
                }

                list.Add(listitem);

            }
        }


        public static void FillRandomValue(SimulationAttribute simulationAttr, 
                                           object obj, 
                                           PropertyInfo? property = null, 
                                           object? previousObj = null)
        {
            if (property == null)
            {
                return;
            }

            var random = new Random();

            if (simulationAttr.NeedsFollowPrevious && previousObj != null)
            {
                var previousValue = property.GetValue(previousObj);
                if (previousValue != null)
                {
                    var previousDoubleValue = Convert.ToDouble(previousValue);
                    double maxChange = previousDoubleValue * simulationAttr.MaximumChange;
                    double minValue, maxValue;

                    if (simulationAttr.ShouldRaise)
                    {
                        minValue = previousDoubleValue;
                        maxValue = previousDoubleValue + maxChange;
                    }
                    else
                    {
                        minValue = previousDoubleValue - maxChange;
                        maxValue = previousDoubleValue;
                    }

                    minValue = Math.Max(minValue, simulationAttr.MinValue);
                    maxValue = Math.Min(maxValue, simulationAttr.MaxValue);

                    if (property.PropertyType == typeof(double))
                    {
                        property.SetValue(obj, minValue + random.NextDouble() * (maxValue - minValue));
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(obj, random.Next((int)minValue, (int)maxValue));
                    }
                }
            }
            else
            {
                double initialValue;
                if (simulationAttr.NeedsFollowPrevious)
                {
                    if (simulationAttr.ShouldRaise)
                    {
                        initialValue = simulationAttr.MinValue;
                    }
                    else
                    {
                        initialValue = simulationAttr.MaxValue;
                    }
                }
                else
                {
                    initialValue = simulationAttr.MinValue + random.NextDouble() * (simulationAttr.MaxValue - simulationAttr.MinValue);
                }

                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(obj, (int)initialValue);
                }
                else if (property.PropertyType == typeof(int?))
                {
                    // if is null init the prop
                    if (property.GetValue(obj) == null)
                    {
                        property.SetValue(obj, 0);
                    }
                    property.SetValue(obj, (int)initialValue);
                }
                else if (property.PropertyType == typeof(double))
                {
                    var value = Math.Round(initialValue, 4);
                    property.SetValue(obj, value);
                }
                else if (property.PropertyType == typeof(double?))
                {
                    // if is null init the prop
                    if (property.GetValue(obj) == null)
                    {
                        property.SetValue(obj, 0.0);
                    }

                    var value = Math.Round(initialValue, 4);
                    property.SetValue(obj, value);
                }
                else if (property.PropertyType == typeof(long))
                {
                    var value = Math.Round(initialValue, 4);
                    property.SetValue(obj, (long)initialValue);
                }
                else if (property.PropertyType == typeof(long?))
                {
                    //if is null init the prop
                    if (property.GetValue(obj) == null)
                    {
                        property.SetValue(obj, 0);
                    }
                    var value = Math.Round(initialValue, 4);
                    property.SetValue(obj, (long)initialValue);
                }

            }
        }

        public static void FillCommonStatics(object obj, object? previousObj = null)
        {
            if (obj != null)
            {
                var type = obj.GetType();

                // add name of the FW and other static basic common info
                if (obj is ChesterCommonCloudMessage chesterCommonCloudMessage)
                {
                    chesterCommonCloudMessage.Attribute.FwVersion = "3.4.0";
                    chesterCommonCloudMessage.Attribute.HwVariant = "CGLS";
                    chesterCommonCloudMessage.Attribute.HwRevision = "R3.2";
                    chesterCommonCloudMessage.Attribute.ProductName = "CHESTER-M";
                    chesterCommonCloudMessage.Attribute.VendorName = "HARDWARIO";

                    if (type == typeof(CatalogApps.Clime.ChesterClimeCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Clime";
                    }
                    else if (type == typeof(CatalogApps.Current.ChesterCurrentCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Current";
                    }
                    else if (type == typeof(CatalogApps.Boiler.ChesterBoilerCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Boiler";
                    }
                    else if (type == typeof(CatalogApps.ClimeIAQ.ChesterClimeIAQCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Clime IAQ";
                    }
                    else if (type == typeof(CatalogApps.Control.ChesterControlCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Control";
                    }
                    else if (type == typeof(CatalogApps.Counter.ChesterCounterCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Counter";
                    }
                    else if (type == typeof(CatalogApps.Input.ChesterInputCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Input";
                    }
                    else if (type == typeof(CatalogApps.Meteo.ChesterMeteoCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Meteo";
                    }
                    else if (type == typeof(CatalogApps.Push.ChesterPushCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Push";
                    }
                    else if (type == typeof(CatalogApps.Radon.ChesterRadonCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Radon";
                    }
                    else if (type == typeof(CatalogApps.Range.ChesterRangeCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER Range";
                    }
                    else if (type == typeof(CatalogApps.wMBus.ChesterWMBusCloudMessage))
                    {
                        chesterCommonCloudMessage.Attribute.FwName = "CHESTER wM-Bus";
                    }

                    if (previousObj != null)
                    {
                        chesterCommonCloudMessage.Attribute.SerialNumber = ((ChesterCommonCloudMessage)previousObj).Attribute.SerialNumber;
                        // imei
                        chesterCommonCloudMessage.Network.Imei = ((ChesterCommonCloudMessage)previousObj).Network.Imei;
                        // imsi
                        chesterCommonCloudMessage.Network.Imsi = ((ChesterCommonCloudMessage)previousObj).Network.Imsi;
                    }
                    else
                    {
                        // create serial number if not exists in format of random number with 10 numbers
                        if (string.IsNullOrEmpty(chesterCommonCloudMessage.Attribute.SerialNumber) ||
                            chesterCommonCloudMessage.Attribute.SerialNumber == Defaults.UnknownSerialNumber)
                        {
                            chesterCommonCloudMessage.Attribute.SerialNumber = new Random().Next(1000000000, 2000000000).ToString();
                        }

                        // create imei number if not exists in format of random number with 15 numbers
                        if (chesterCommonCloudMessage.Network.Imei == 0)
                        {
                            // get first 5 number of the imei
                            var firstpart = new Random().Next(10000, 20000);
                            // get second 5 numbers of the imei
                            var secondpart = new Random().Next(10000, 20000);
                            // get third 5 numbers of the imei
                            var thirdpart = new Random().Next(10000, 20000);
                            chesterCommonCloudMessage.Network.Imei = long.Parse($"{firstpart}{secondpart}{thirdpart}");
                        }

                        // create imsi number if not exists in format of random number with 15 numbers
                        if (chesterCommonCloudMessage.Network.Imsi == 0)
                        {
                            // get first 5 number of the imsi
                            var firstpart = new Random().Next(10000, 20000);
                            // get second 5 numbers of the imsi
                            var secondpart = new Random().Next(10000, 20000);
                            // get third 5 numbers of the imsi
                            var thirdpart = new Random().Next(10000, 20000);
                            chesterCommonCloudMessage.Network.Imsi = long.Parse($"{firstpart}{secondpart}{thirdpart}");
                        }
                    }
                    // fill other static network parameters
                    chesterCommonCloudMessage.Network.Parameter.Plmn = 23003;
                    chesterCommonCloudMessage.Network.Parameter.Cid = 939040;
                    chesterCommonCloudMessage.Network.Parameter.Band = 20;
                    chesterCommonCloudMessage.Network.Parameter.Earfcn = 6557;
                    chesterCommonCloudMessage.Network.Parameter.Eest = 7;
                }
            }
        }
    }
}