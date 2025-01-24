using hio_dotnet.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Collections;
using System.Text.RegularExpressions;

namespace hio_dotnet.Common.Models
{
    public static class TimeStampFormatDataConverter
    {
        private static string dataPrefix = "job.body";
        public static IEnumerable<TimeStampData> GetTimeStampData(object data, string parentName = "")
        {
            if (data == null) yield break;

            var groupedData = new Dictionary<long, TimeStampData>();

            void AddToGroup(long timestamp, string key, object value)
            {
                if (!groupedData.TryGetValue(timestamp, out var timestampData))
                {
                    timestampData = new TimeStampData
                    {
                        Timestamp = timestamp,
                        Values = new Dictionary<string, object>()
                    };
                    groupedData[timestamp] = timestampData;
                }

                if (!timestampData.Values.ContainsKey(key))
                {
                    timestampData.Values[key] = value;
                }
            }

            var props = data.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == "Timestamp")
                {
                    var timestamp = Convert.ToInt64(prop.GetValue(data) ?? 0);
                    if (!groupedData.ContainsKey(timestamp))
                    {
                        groupedData[timestamp] = new TimeStampData
                        {
                            Timestamp = timestamp,
                            Values = new Dictionary<string, object>()
                        };
                    }
                    continue;
                }

                var jsonPropertyName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                if (string.IsNullOrEmpty(jsonPropertyName)) continue;

                var propName = string.IsNullOrEmpty(parentName) ? jsonPropertyName : $"{parentName}.{jsonPropertyName}";

                // Processing simple types
                if (prop.PropertyType == typeof(string) ||
                    prop.PropertyType == typeof(bool) ||
                    prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(float) ||
                    prop.PropertyType == typeof(double) ||
                    prop.PropertyType == typeof(long))
                {
                    var timestampProp = props.FirstOrDefault(p => p.Name == "Timestamp");
                    var timestamp = timestampProp != null ? Convert.ToInt64(timestampProp.GetValue(data) ?? 0) : 0;
                    AddToGroup(timestamp, propName, $"{dataPrefix}.{propName}");
                }
                // Processing lists
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                         prop.PropertyType != typeof(string))
                {
                    var list = prop.GetValue(data) as System.Collections.IEnumerable;
                    if (list == null) continue;

                    int index = 0;
                    foreach (var item in list)
                    {
                        var listItemName = $"{propName}[{index}]";
                        foreach (var nestedItem in GetTimeStampData(item, listItemName))
                        {
                            foreach (var value in nestedItem.Values)
                            {
                                AddToGroup(nestedItem.Timestamp, value.Key, value.Value);
                            }
                        }
                        index++;
                    }
                }
                // Processing objects
                else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string))
                {
                    var childObject = prop.GetValue(data);
                    if (childObject != null)
                    {
                        foreach (var nestedItem in GetTimeStampData(childObject, propName))
                        {
                            foreach (var value in nestedItem.Values)
                            {
                                AddToGroup(nestedItem.Timestamp, value.Key, value.Value);
                            }
                        }
                    }
                }
            }

            foreach (var result in groupedData.Values)
            {
                yield return result;
            }
        }

        public static TimeStampData GetCombinedTimeStampData(object data, string parentName = "")
        {
            if (data == null) return null;

            TimeStampData combinedData = new TimeStampData();
            var isTimestampPlaced = false;
            var props = data.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name == "Timestamp" && combinedData != null && !isTimestampPlaced)
                {
                    var timestamp = Convert.ToInt64(prop.GetValue(data) ?? 0);
                    combinedData.Timestamp = timestamp;
                    isTimestampPlaced = true;
                }

                var jsonPropertyName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                if (string.IsNullOrEmpty(jsonPropertyName)) continue;

                var propName = string.IsNullOrEmpty(parentName) ? jsonPropertyName : $"{parentName}.{jsonPropertyName}";

                // Processing simple types
                if (prop.PropertyType == typeof(string) ||
                    prop.PropertyType == typeof(bool) ||
                    prop.PropertyType == typeof(int) ||
                    prop.PropertyType == typeof(float) ||
                    prop.PropertyType == typeof(double) ||
                    prop.PropertyType == typeof(long))
                {
                    combinedData?.Values.Add(propName, $"job.body.{propName}");
                }
                // Processing lists
                else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                         prop.PropertyType != typeof(string))
                {
                    var list = prop.GetValue(data) as System.Collections.IEnumerable;
                    if (list == null) continue;

                    int index = 0;
                    foreach (var item in list)
                    {
                        var listItemName = $"{propName}[{index}]";
                        foreach (var nestedItem in GetTimeStampData(item, listItemName))
                        {
                            foreach (var value in nestedItem.Values)
                            {
                                combinedData?.Values.Add(value.Key, value.Value);
                            }
                        }
                        index++;
                    }
                }
                // Processing objects
                else if (!prop.PropertyType.IsPrimitive && prop.PropertyType != typeof(string))
                {
                    var childObject = prop.GetValue(data);
                    if (childObject != null)
                    {
                        foreach (var nestedItem in GetTimeStampData(childObject, propName))
                        {
                            foreach (var value in nestedItem.Values)
                            {
                                combinedData?.Values.Add(value.Key, value.Value);
                            }
                        }
                    }
                }
            }

            return combinedData;
        }

        public static string GetCombinedTimeStampDataJSCode(object data, List<string>? keysToInclude = null)
        {
            var combinedData = GetCombinedTimeStampData(data);
            if (combinedData == null) return string.Empty;

            combinedData.Values = combinedData.Values
                                    .OrderBy(x => x.Key, new CustomComparer())
                                    .ToDictionary(x => x.Key, x => x.Value);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("var data = {");
            stringBuilder.AppendLine($"\t\tts: {combinedData.Timestamp},");
            stringBuilder.AppendLine("\t\tvalues: {");
            foreach (var val in combinedData.Values)
            {
                if (keysToInclude != null && keysToInclude.Count == 0)
                {
                    continue;
                }
                else if (keysToInclude == null || keysToInclude.Contains(val.Key))
                {
                    stringBuilder.AppendLine($"\t\t\t\"{val.Key}\": {val.Value},");
                }
            }
            stringBuilder.AppendLine("\t\t}");
            stringBuilder.AppendLine("\t};");
            var result = stringBuilder.ToString();
            
            return result;
        }

        public static string GetTimeStampDataJSCode(object data, List<string>? keysToInclude = null)
        {
            var combinedDataList = GetTimeStampData(data).ToList();
            if (combinedDataList == null) return string.Empty;
            var stringBuilder = new StringBuilder();
            var index = 0;

            stringBuilder.AppendLine($"");
            stringBuilder.AppendLine($"var data = [");

            long timestamp = 0;

            foreach (var combinedData in combinedDataList)
            {
                if (combinedData.Timestamp == 0)
                {
                    combinedData.Timestamp = timestamp;
                }
                else
                {
                    timestamp = combinedData.Timestamp;
                }

                combinedData.Values = combinedData.Values
                                    .OrderBy(x => x.Key, new CustomComparer())
                                    .ToDictionary(x => x.Key, x => x.Value);

                stringBuilder.AppendLine($"\t{{");
                stringBuilder.AppendLine($"\t\tts: {combinedData.Timestamp},");
                stringBuilder.AppendLine("\t\tvalues: {");
                foreach (var val in combinedData.Values)
                {
                    if (keysToInclude != null && keysToInclude.Count == 0)
                    {
                        continue;
                    }
                    else if (keysToInclude == null || keysToInclude.Contains(val.Key))
                    {
                        stringBuilder.AppendLine($"\t\t\t\"{val.Key}\": {val.Value},");
                    }
                }
                stringBuilder.AppendLine("\t\t},");
                if (index == combinedDataList.Count - 1)
                {
                    stringBuilder.AppendLine("\t}");
                }
                else
                {
                    stringBuilder.AppendLine("\t},");
                }
                index++;
            }

            stringBuilder.AppendLine("];");
            stringBuilder.AppendLine("");
            var result = stringBuilder.ToString();

            return result;
        }

        public static List<string> GetCombinedKeys(object data)
        {
            var combinedData = GetCombinedTimeStampData(data);
            if (combinedData == null) return new List<string>();

            combinedData.Values = combinedData.Values
                                    .OrderBy(x => x.Key, new CustomComparer())
                                    .ToDictionary(x => x.Key, x => x.Value);

            return combinedData.Values.Keys.ToList();
        }

    }

    public class CustomComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null || y == null)
            {
                return string.Compare(x, y, StringComparison.Ordinal);
            }

            // split strings into parts (text and numeric)
            var xParts = SplitIntoParts(x);
            var yParts = SplitIntoParts(y);

            // compare parts one by one
            for (int i = 0; i < Math.Min(xParts.Count, yParts.Count); i++)
            {
                var xPart = xParts[i];
                var yPart = yParts[i];

                // if parts are numbers, compare as numbers
                if (int.TryParse(xPart, out int xNum) && int.TryParse(yPart, out int yNum))
                {
                    int result = xNum.CompareTo(yNum);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    int result = string.Compare(xPart, yPart, StringComparison.Ordinal);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            // if parts are the same, compare lengths
            return xParts.Count.CompareTo(yParts.Count);
        }

        private List<string> SplitIntoParts(string input)
        {
            // Regular expression for separating text and numeric parts
            var regex = new Regex(@"\d+|[^\d]+");
            return regex.Matches(input).Select(m => m.Value).ToList();
        }
    }
}
