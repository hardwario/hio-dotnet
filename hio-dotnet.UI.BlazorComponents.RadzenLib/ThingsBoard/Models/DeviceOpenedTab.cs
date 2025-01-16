using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClosedXML.Excel;
using hio_dotnet.APIs.ThingsBoard.Models;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public class DeviceOpenedTab : CommonOpenedTab
    {
        public Device? Device { get => Data as Device; set => Data = value; }

        public List<KeyToCheck> Keys = new List<KeyToCheck>();

        public IOrderedEnumerable<KeyToCheck> KeysWithDataPoints { get => Keys.Where(k => k.DataPoints != null).OrderBy(k => k.Key); }

        public bool IsAnyKeyWithDataPoints { get => Keys.Any(k => k.DataPoints != null && k.DataPoints.Count > 0); }

        /// <summary>
        /// Set all DataPoints to null in each Key
        /// </summary>
        public void ClearDataPoints()
        {
            foreach (var key in Keys)
            {
                key.DataPoints = null;
            }
        }

        /// <summary>
        /// Export selected Keys to the Excel sheet.
        /// </summary>
        /// <param name="data"></param>
        public void ParseHistoryDataPoints(string data)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, List<DataPoint>>>(data);
                foreach (var key in Keys)
                {
                    if (dict.ContainsKey(key.Key))
                    {
                        key.DataPoints = dict[key.Key];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public byte[]? ExportDataPointsAsExcel()
        {
            try
            {
                var keysWithData = Keys.Where(k => k.DataPoints != null && k.DataPoints.Count > 0)
                                       .OrderBy(k => k.Key)
                                       .ToList();

                if (!keysWithData.Any())
                {
                    return null;
                }

                using var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("ExportedData");

                int colIndex = 1;
                foreach (var k in keysWithData)
                {
                    worksheet.Cell(1, colIndex).Value = $"{k.Key}_date";
                    worksheet.Cell(1, colIndex + 1).Value = $"{k.Key}_value";
                    colIndex += 2;
                }

                int maxRowCount = keysWithData.Max(k => k.DataPoints.Count);

                for (int row = 0; row < maxRowCount; row++)
                {
                    colIndex = 1;
                    foreach (var k in keysWithData)
                    {
                        if (row < k.DataPoints.Count)
                        {
                            var dp = k.DataPoints[row];
                            worksheet.Cell(row + 2, colIndex).Value = dp.date;
                            worksheet.Cell(row + 2, colIndex + 1).Value = dp.value?.ToString() ?? string.Empty;
                        }
                        colIndex += 2;
                    }
                }

                worksheet.Columns().AdjustToContents();

                using var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);
                var fileContent = memoryStream.ToArray();

                return fileContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
