using hio_dotnet.Common.Helpers;
using hio_dotnet.Common.Models.CatalogApps.Dust;
using hio_dotnet.Common.Models.CatalogApps.Meteo;
using hio_dotnet.Common.Models.CatalogApps.Push;
using hio_dotnet.Common.Models.CatalogApps.Scale;
using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    public class BaseSimulator
    {
        public static void GetSimulatedData(object obj, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (!forceTimestamp)
            {
                timestamp = TimeHelpers.DateTimeToUnixTimestamp(DateTime.UtcNow);
                forceTimestamp = true;
            }

            Simulate(obj, previousObj, forceTimestamp, timestamp);

            // add name of the FW and other static basic common info
            FillCommonStatics(obj, previousObj);
        }

        public static void Simulate(object obj, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
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
                        FillRandomValue(simulationAttr, obj, property, previousObj, forceTimestamp, timestamp);
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
                            SimulateMeasurementGroup(nestedObj, simulationMeasurementAttr, previousNestedObj, forceTimestamp, timestamp);
                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(SimpleDoubleMeasurementGroup))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            SimulateSimpleDoubleMeasurementGroup(nestedObj, simulationMeasurementAttr, previousNestedObj, forceTimestamp, timestamp);
                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(SimpleIntMeasurementGroup))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            SimulateSimpleIntMeasurementGroup(nestedObj, simulationMeasurementAttr, previousNestedObj, forceTimestamp, timestamp);
                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(Temperature))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            Simulate_Temperature(nestedObj, simulationMeasurementAttr, previousNestedObj, forceTimestamp, timestamp);
                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<WeightMeasurement>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                // create a new Weight object

                                var w1t = new WeightMeasurement();
                                WeightMeasurement w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    if (((List<WeightMeasurement>)previousNestedObj).Count > i)
                                        w1tp = ((List<WeightMeasurement>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<WeightMeasurement>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate(w1t, w1tp, forceTimestamp, timestamp);

                                ((List<WeightMeasurement>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<PushButtonsStates>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                var w1t = new PushButtonsStates();
                                PushButtonsStates w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    if (((List<PushButtonsStates>)previousNestedObj).Count > i)
                                        w1tp = ((List<PushButtonsStates>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<PushButtonsStates>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate(w1t, w1tp, forceTimestamp, timestamp);

                                w1t.Button = $"Button_{i}";

                                ((List<PushButtonsStates>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
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
                                    if (((List<W1_Thermometer>)previousNestedObj).Count > i)
                                        w1tp = ((List<W1_Thermometer>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<W1_Thermometer>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate_W1_Thermoemter(w1t, simulationMeasurementAttr, w1tp, forceTimestamp, timestamp);

                                ((List<W1_Thermometer>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<SoilMeasurements>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                var w1t = new SoilMeasurements();
                                SoilMeasurements w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    if (((List<SoilMeasurements>)previousNestedObj).Count > i)
                                        w1tp = ((List<SoilMeasurements>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<SoilMeasurements>)previousNestedObj)?.FirstOrDefault();
                                }

                                Simulate(w1t, w1tp, forceTimestamp, timestamp);

                                ((List<SoilMeasurements>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<DustSensorData>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                var w1t = new DustSensorData();
                                DustSensorData w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    if (((List<DustSensorData>)previousNestedObj).Count > i)
                                        w1tp = ((List<DustSensorData>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<DustSensorData>)previousNestedObj)?.FirstOrDefault();
                                }

                                Simulate(w1t, w1tp, forceTimestamp, timestamp);

                                ((List<DustSensorData>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<RTD_Thermometer>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                var w1t = new RTD_Thermometer();
                                RTD_Thermometer w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    if (((List<RTD_Thermometer>)previousNestedObj).Count > i)
                                        w1tp = ((List<RTD_Thermometer>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<RTD_Thermometer>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate_RTD_Thermoemter(w1t, simulationMeasurementAttr, w1tp, i, forceTimestamp, timestamp);

                                ((List<RTD_Thermometer>)nestedObj).Add(w1t);
                            }

                            property.SetValue(obj, nestedObj);
                        }
                    }
                    else if (nestedObj.GetType() == typeof(List<AnalogChannel>))
                    {
                        var simulationMeasurementAttr = property.GetCustomAttribute<SimulationMeasurementAttribute>();
                        if (simulationMeasurementAttr != null)
                        {
                            var count = simulationMeasurementAttr.NumberOfInsideItems;
                            for (int i = 0; i < count; i++)
                            {
                                var w1t = new AnalogChannel();
                                AnalogChannel w1tp = null;
                                if (previousNestedObj != null)
                                {
                                    if (((List<AnalogChannel>)previousNestedObj).Count > i)
                                        w1tp = ((List<AnalogChannel>)previousNestedObj)[i];
                                    else
                                        w1tp = ((List<AnalogChannel>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate_AnalogChannel(w1t, simulationMeasurementAttr, w1tp, i, forceTimestamp, timestamp);

                                ((List<AnalogChannel>)nestedObj).Add(w1t);
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
                                    if (((List<BLE_Tag>)previousNestedObj).Count > i)
                                        bltp = ((List<BLE_Tag>)previousNestedObj)[i];
                                    else
                                        bltp = ((List<BLE_Tag>)previousNestedObj)?.FirstOrDefault();
                                }
                                Simulate(blt, bltp, forceTimestamp, timestamp);

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
                        Simulate(nestedObj, previousNestedObj, forceTimestamp, timestamp);
                        property.SetValue(obj, nestedObj);
                    }
                }
            }
        }

        public static void SimulateMeasurementGroup(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
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
                        FillMeasurementList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void SimulateSimpleDoubleMeasurementGroup(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (obj.GetType() != typeof(SimpleDoubleMeasurementGroup))
            {
                return;
            }
            if (previousObj != null)
            {
                if (previousObj.GetType() != typeof(SimpleDoubleMeasurementGroup))
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
                    if (property != null && property.PropertyType == typeof(List<SimpleTimeDoubleMeasurement>))
                    {
                        FillSimpleTimeDoubleList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void SimulateSimpleIntMeasurementGroup(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (obj.GetType() != typeof(SimpleIntMeasurementGroup))
            {
                return;
            }
            if (previousObj != null)
            {
                if (previousObj.GetType() != typeof(SimpleIntMeasurementGroup))
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
                    if (property != null && property.PropertyType == typeof(List<SimpleTimeIntMeasurement>))
                    {
                        FillSimpleTimeIntList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void Simulate_W1_Thermoemter(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
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
                if (property.Name == "SerialNumber")
                {
                    // if previousObject exists, take serial number from there. If not, use some random
                    if (previousObj != null)
                    {
                        property.SetValue(obj, ((W1_Thermometer)previousObj).SerialNumber);
                    }
                    else
                    {
                        // create serial number if not exists in format of random number with 10 numbers
                        property.SetValue(obj, new Random().Next(1000000000, 2000000000));
                    }
                }
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (property != null && property.PropertyType == typeof(List<Measurement>))
                    {
                        FillMeasurementList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void Simulate_RTD_Thermoemter(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, int? channel = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (obj.GetType() != typeof(RTD_Thermometer))
            {
                return;
            }
            if (previousObj != null)
            {
                if (previousObj.GetType() != typeof(RTD_Thermometer))
                {
                    return;
                }
            }

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Channel")
                {
                    // if previousObject exists, take channel from there. If not, use some random
                    if (previousObj != null)
                    {
                        property.SetValue(obj, ((RTD_Thermometer)previousObj).Channel);
                    }
                    else
                    {
                        if (channel != null)
                        {
                            property.SetValue(obj, channel);
                        }
                        else
                        {
                            property.SetValue(obj, new Random().Next(0, 99));
                        }
                    }
                }

                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (property != null && property.PropertyType == typeof(List<Measurement>))
                    {
                        FillMeasurementList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void Simulate_Temperature(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (obj.GetType() != typeof(Temperature))
            {
                return;
            }
            if (previousObj != null)
            {
                if (previousObj.GetType() != typeof(Temperature))
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
                        FillMeasurementList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                    
                    if (property != null && property.PropertyType == typeof(List<SimpleTimeDoubleMeasurement>))
                    {
                        FillSimpleTimeDoubleList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void Simulate_AnalogChannel(object obj, SimulationMeasurementAttribute simulationMeasurementAttr, object? previousObj = null, int? channel = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (obj.GetType() != typeof(AnalogChannel))
            {
                return;
            }
            if (previousObj != null)
            {
                if (previousObj.GetType() != typeof(AnalogChannel))
                {
                    return;
                }
            }

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Channel")
                {
                    // if previousObject exists, take channel from there. If not, use some random
                    if (previousObj != null)
                    {
                        property.SetValue(obj, ((AnalogChannel)previousObj).Channel);
                    }
                    else
                    {
                        if (channel != null)
                        {
                            property.SetValue(obj, channel);
                        }
                        else
                        {
                            property.SetValue(obj, new Random().Next(0, 99));
                        }
                    }
                }

                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    if (property != null && property.PropertyType == typeof(List<MeanRMSMeasurement>))
                    {
                        FillRMSMeasurementList(obj, simulationMeasurementAttr, property, previousObj, forceTimestamp, timestamp);
                    }
                }
            }
        }

        public static void FillMeasurementList(object obj, 
                                               SimulationMeasurementAttribute simulationMeasurementAttr, 
                                               PropertyInfo? property = null, 
                                               object? previousObj = null, 
                                               bool forceTimestamp = false, 
                                               long timestamp = 0)
        {
            // add new item to the list
            var list = (List<Measurement>)property.GetValue(obj);
            if (list != null)
            {
                for (int i = 0; i < simulationMeasurementAttr.NumberOfInsideItems; i++)
                {
                    var listitem = new Measurement();
                    Measurement? previousMeasurementObj = null;

                    // if previousObj is not null check the existence of the List<Measurement> and if it has any item take it as previousMeasurementObj
                    if (previousObj != null)
                    {
                        var previousList = (List<Measurement>)property.GetValue(previousObj);
                        if (previousList != null && previousList.Count >= simulationMeasurementAttr.NumberOfInsideItems)
                        {
                            previousMeasurementObj = previousList[i];
                        }
                    }

                    var propertiesMeasurement = typeof(Measurement).GetProperties();

                    if (forceTimestamp)
                    {
                        var ts = propertiesMeasurement.FirstOrDefault(p => p.Name == "Timestamp");
                        if (ts != null)
                        {
                            ts.SetValue(listitem, timestamp);
                        }
                    }

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
                        FillRandomValue(avgPropAttr, listitem, avgProp, previousMeasurementObj, forceTimestamp, timestamp);
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
        }

        public static void FillRMSMeasurementList(object obj,
                                       SimulationMeasurementAttribute simulationMeasurementAttr,
                                       PropertyInfo? property = null,
                                       object? previousObj = null, 
                                       bool forceTimestamp = false, 
                                       long timestamp = 0)
        {
            // add new item to the list
            var list = (List<MeanRMSMeasurement>)property.GetValue(obj);
            if (list != null)
            {
                for (int i = 0; i < simulationMeasurementAttr.NumberOfInsideItems; i++)
                {
                    var listitem = new MeanRMSMeasurement();
                    MeanRMSMeasurement? previousMeasurementObj = null;

                    // if previousObj is not null check the existence of the List<Measurement> and if it has any item take it as previousMeasurementObj
                    if (previousObj != null)
                    {
                        var previousList = (List<MeanRMSMeasurement>)property.GetValue(previousObj);
                        if (previousList != null && previousList.Count >= simulationMeasurementAttr.NumberOfInsideItems)
                        {
                            previousMeasurementObj = previousList[i];
                        }
                    }

                    var propertiesMeasurement = typeof(MeanRMSMeasurement).GetProperties();

                    if (forceTimestamp)
                    {
                        var ts = propertiesMeasurement.FirstOrDefault(p => p.Name == "Timestamp");
                        if (ts != null)
                        {
                            ts.SetValue(listitem, timestamp);
                        }
                    }

                    var avgProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "MeanAvg");
                    var mdnProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "MeanMdn");
                    var minProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "MeanMin");
                    var maxProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "MeanMax");

                    var avgPropAttr = avgProp?.GetCustomAttribute<SimulationAttribute>();
                    if (avgPropAttr != null)
                    {
                        avgPropAttr.MinValue = simulationMeasurementAttr.MinValue;
                        avgPropAttr.MaxValue = simulationMeasurementAttr.MaxValue;
                        avgPropAttr.NeedsFollowPrevious = simulationMeasurementAttr.NeedsFollowPrevious;
                        avgPropAttr.ShouldRaise = simulationMeasurementAttr.ShouldRaise;
                        avgPropAttr.MaximumChange = simulationMeasurementAttr.MaximumChange;
                        FillRandomValue(avgPropAttr, listitem, avgProp, previousMeasurementObj, forceTimestamp, timestamp);
                    }

                    var random = new Random();
                    //now calculate the min and max values with some random spread out of the avg value min to minus value and max to plus value
                    var randomSpread = listitem.MeanAvg * random.NextDouble() * 0.1;
                    listitem.MeanMdn = listitem.MeanAvg - randomSpread;
                    listitem.MeanMin = listitem.MeanAvg - (randomSpread * 2);
                    listitem.MeanMax = listitem.MeanAvg + (randomSpread * 2);

                    if (listitem.MeanMax > simulationMeasurementAttr.MaxValue)
                    {
                        listitem.MeanMax = simulationMeasurementAttr.MaxValue;
                    }
                    if (listitem.MeanMin < simulationMeasurementAttr.MinValue)
                    {
                        listitem.MeanMin = simulationMeasurementAttr.MinValue;
                    }

                    if (listitem.MeanMdn > simulationMeasurementAttr.MaxValue)
                    {
                        listitem.MeanMdn = simulationMeasurementAttr.MaxValue;
                    }
                    if (listitem.MeanMdn < simulationMeasurementAttr.MinValue)
                    {
                        listitem.MeanMdn = simulationMeasurementAttr.MinValue;
                    }

                    listitem.RmsAvg = listitem.MeanAvg * 2;
                    var randomSpreadRMS = listitem.RmsAvg * random.NextDouble() * 0.1;
                    listitem.RmsMdn = listitem.RmsAvg - randomSpreadRMS;
                    listitem.RmsMin = listitem.RmsAvg - (randomSpreadRMS * 2);
                    listitem.RmsMax = listitem.RmsAvg + (randomSpreadRMS * 2);

                    listitem.RmsMax = listitem.RmsMax > (simulationMeasurementAttr.MaxValue * 2) ? simulationMeasurementAttr.MaxValue * 2 : listitem.RmsMax;
                    listitem.RmsMin = listitem.RmsMin < (simulationMeasurementAttr.MinValue * 2) ? simulationMeasurementAttr.MinValue * 2 : listitem.RmsMin;
                    listitem.RmsMdn = listitem.RmsMdn > (simulationMeasurementAttr.MaxValue * 2) ? simulationMeasurementAttr.MaxValue * 2 : listitem.RmsMdn;
                    listitem.RmsMdn = listitem.RmsMdn < (simulationMeasurementAttr.MinValue * 2) ? simulationMeasurementAttr.MinValue * 2 : listitem.RmsMdn;

                    list.Add(listitem);
                }
            }
        }

        public static void FillSimpleTimeDoubleList(object obj,
                                       SimulationMeasurementAttribute simulationMeasurementAttr,
                                       PropertyInfo? property = null,
                                       object? previousObj = null, 
                                       bool forceTimestamp = false, 
                                       long timestamp = 0)
        {
            // add new item to the list
            var list = (List<SimpleTimeDoubleMeasurement>)property.GetValue(obj);
            if (list != null)
            {
                for (int i = 0; i < simulationMeasurementAttr.NumberOfInsideItems; i++)
                {
                    var listitem = new SimpleTimeDoubleMeasurement();
                    SimpleTimeDoubleMeasurement? previousMeasurementObj = null;

                    // if previousObj is not null check the existence of the List<Measurement> and if it has any item take it as previousMeasurementObj
                    if (previousObj != null)
                    {
                        var previousList = (List<SimpleTimeDoubleMeasurement>)property.GetValue(previousObj);
                        if (previousList != null && previousList.Count >= simulationMeasurementAttr.NumberOfInsideItems)
                        {
                            previousMeasurementObj = previousList[i];
                        }
                    }

                    var propertiesMeasurement = typeof(SimpleTimeDoubleMeasurement).GetProperties();

                    if (forceTimestamp)
                    {
                        var ts = propertiesMeasurement.FirstOrDefault(p => p.Name == "Timestamp");
                        if (ts != null)
                        {
                            ts.SetValue(listitem, timestamp);
                        }
                    }

                    var avgProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "Value");

                    var avgPropAttr = avgProp?.GetCustomAttribute<SimulationAttribute>();
                    if (avgPropAttr != null)
                    {
                        avgPropAttr.MinValue = simulationMeasurementAttr.MinValue;
                        avgPropAttr.MaxValue = simulationMeasurementAttr.MaxValue;
                        avgPropAttr.NeedsFollowPrevious = simulationMeasurementAttr.NeedsFollowPrevious;
                        avgPropAttr.ShouldRaise = simulationMeasurementAttr.ShouldRaise;
                        avgPropAttr.MaximumChange = simulationMeasurementAttr.MaximumChange;
                        FillRandomValue(avgPropAttr, listitem, avgProp, previousMeasurementObj, forceTimestamp, timestamp);
                    }

                    list.Add(listitem);
                }
            }
        }

        public static void FillSimpleTimeIntList(object obj,
                               SimulationMeasurementAttribute simulationMeasurementAttr,
                               PropertyInfo? property = null,
                               object? previousObj = null, 
                               bool forceTimestamp = false, 
                               long timestamp = 0)
        {
            // add new item to the list
            var list = (List<SimpleTimeIntMeasurement>)property.GetValue(obj);
            if (list != null)
            {
                for (int i = 0; i < simulationMeasurementAttr.NumberOfInsideItems; i++)
                {
                    var listitem = new SimpleTimeIntMeasurement();
                    SimpleTimeIntMeasurement? previousMeasurementObj = null;

                    // if previousObj is not null check the existence of the List<Measurement> and if it has any item take it as previousMeasurementObj
                    if (previousObj != null)
                    {
                        var previousList = (List<SimpleTimeIntMeasurement>)property.GetValue(previousObj);
                        if (previousList != null && previousList.Count >= simulationMeasurementAttr.NumberOfInsideItems)
                        {
                            previousMeasurementObj = previousList[i];
                        }
                    }

                    var propertiesMeasurement = typeof(SimpleTimeIntMeasurement).GetProperties();

                    if (forceTimestamp)
                    {
                        var ts = propertiesMeasurement.FirstOrDefault(p => p.Name == "Timestamp");
                        if (ts != null)
                        {
                            ts.SetValue(listitem, timestamp);
                        }
                    }

                    var avgProp = propertiesMeasurement.FirstOrDefault(p => p.Name == "Value");

                    var avgPropAttr = avgProp?.GetCustomAttribute<SimulationAttribute>();
                    if (avgPropAttr != null)
                    {
                        avgPropAttr.MinValue = simulationMeasurementAttr.MinValue;
                        avgPropAttr.MaxValue = simulationMeasurementAttr.MaxValue;
                        avgPropAttr.NeedsFollowPrevious = simulationMeasurementAttr.NeedsFollowPrevious;
                        avgPropAttr.ShouldRaise = simulationMeasurementAttr.ShouldRaise;
                        avgPropAttr.MaximumChange = simulationMeasurementAttr.MaximumChange;
                        FillRandomValue(avgPropAttr, listitem, avgProp, previousMeasurementObj, forceTimestamp, timestamp);
                    }

                    list.Add(listitem);
                }
            }
        }


        public static void FillRandomValue(SimulationAttribute simulationAttr, 
                                           object obj, 
                                           PropertyInfo? property = null, 
                                           object? previousObj = null, bool forceTimestamp = false, long timestamp = 0)
        {
            if (property == null)
            {
                return;
            }

            var random = new Random();

            var simAttrMin = simulationAttr.MinValue;
            var simAttrMax = simulationAttr.MaxValue;

            if (simAttrMax < simAttrMin)
            {
                simAttrMin = simulationAttr.MaxValue;
                simAttrMax = simulationAttr.MinValue;
            }

            if (simulationAttr.NeedsFollowPrevious && previousObj != null)
            {
                var previousValue = property.GetValue(previousObj);
                if (previousValue != null)
                {
                    var previousDoubleValue = Convert.ToDouble(previousValue);
                    double maxChange = Math.Abs((Math.Abs(simulationAttr.MaxValue - simulationAttr.MinValue)) * simulationAttr.MaximumChange);
                    double minValue, maxValue;

                    if (simulationAttr.ShouldRaise)
                    {
                        minValue = previousDoubleValue;
                        maxValue = previousDoubleValue + maxChange;

                        // in the case that value has already reached the maximum value, set the min value to the maximum value - maxChange
                        // this will keep the cycle still going in the ramp up style
                        if (maxValue > simAttrMax)
                        {
                            maxValue = simAttrMin + maxChange;
                            minValue = simAttrMin;
                        }
                    }
                    else
                    {
                        minValue = previousDoubleValue - maxChange;
                        maxValue = previousDoubleValue;

                        // in the case that value has already reached the minimum value, set the max value to the minimum value + maxChange
                        // this will keep the cycle still going in the ramp down style
                        if (minValue < simAttrMin)
                        {
                            minValue = simAttrMax - maxChange;
                            maxValue = simAttrMax;
                        }
                    }

                    minValue = Math.Max(minValue, simAttrMin);
                    maxValue = Math.Min(maxValue, simAttrMax);

                    if (property.PropertyType == typeof(double))
                    {
                        property.SetValue(obj, minValue + random.NextDouble() * (maxValue - minValue));
                    }
                    else if (property.PropertyType == typeof(double?))
                    {
                        if (property.GetValue(obj) == null)
                        {
                            property.SetValue(obj, 0.0);
                        }

                        property.SetValue(obj, minValue + random.NextDouble() * (maxValue - minValue));
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(obj, random.Next((int)minValue, (int)maxValue));
                    }
                    else if (property.PropertyType == typeof(int?))
                    {
                        if (property.GetValue(obj) == null)
                        {
                            property.SetValue(obj, 0);
                        }

                        if (minValue < maxValue)
                        {
                            property.SetValue(obj, random.Next((int)minValue, (int)maxValue));
                        }
                        else
                        {
                            property.SetValue(obj, random.Next((int)maxValue, (int)minValue));
                        }
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
                        initialValue = simAttrMin;
                    }
                    else
                    {
                        initialValue = simAttrMax;
                    }
                }
                else
                {
                    initialValue = simAttrMin + random.NextDouble() * (simAttrMax - simAttrMin);
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
                    if (forceTimestamp && property.Name == "Timestamp")
                    {
                        property.SetValue(obj, timestamp);
                    }
                    else
                    {
                        var value = Math.Round(initialValue, 4);
                        property.SetValue(obj, (long)initialValue);
                    }
                }
                else if (property.PropertyType == typeof(long?))
                {
                    //if is null init the prop
                    if (property.GetValue(obj) == null)
                    {
                        property.SetValue(obj, 0);
                    }
                    if (forceTimestamp && property.Name == "Timestamp")
                    {
                        property.SetValue(obj, timestamp);
                    }
                    else
                    {
                        var value = Math.Round(initialValue, 4);
                        property.SetValue(obj, (long)initialValue);
                    }
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
                            chesterCommonCloudMessage.Attribute.SerialNumber = GenerateSerialNumberString();
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


        public static uint GenerateSerialNumber(uint productFamily = 0)
        {
            if (productFamily == 0)
                productFamily = 0x00b; // default for CHESTER-M

            if (productFamily > 0x3FF)
            {
                throw new ArgumentOutOfRangeException(nameof(productFamily), "Product Family must be in range of 10 bits (0 up to 0x3FF).");
            }

            // Create serial number (20 bits)
            uint serialPart = (uint)RandomNumberGenerator.GetInt32(0x100000);

            // Assembly: Bit 31 = 1, Bit 30 = 0, bits 29-20 = Product Family, bits 19-0 = Serial Number
            uint serialNumber = (1u << 31) | (productFamily << 20) | serialPart;

            return serialNumber;
        }

        public static string GenerateSerialNumberString(uint productFamily = 0)
        {
            return GenerateSerialNumber(productFamily).ToString();
        }
    }
}