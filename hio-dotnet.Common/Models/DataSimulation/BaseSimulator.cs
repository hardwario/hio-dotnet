using hio_dotnet.Common.Models.Common;
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
        public static void Simulate(object obj, object? previousObj = null)
        {
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var simulationAttr = property.GetCustomAttribute<SimulationAttribute>();

                if (simulationAttr != null && !simulationAttr.IsStatic)
                {
                    FillRandomValue(simulationAttr, obj, property, previousObj);
                }

                else if (property.PropertyType.IsClass &&
                         property.PropertyType != typeof(string) &&
                         !property.PropertyType.IsPrimitive &&
                         property.GetValue(obj) != null)
                {
                    var nestedObj = property.GetValue(obj);
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
                }
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
                else if (property.PropertyType == typeof(double))
                {
                    var value = Math.Round(initialValue, 4);
                    property.SetValue(obj, value);
                }
            }
        }
    }
}