using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public class DataPoint
    {
        public long ts { get; set; }
        public object value { get; set; }

        public string date { get => $"{hio_dotnet.Common.Helpers.TimeHelpers.UnixTimestampToDateTime(ts)}"; }
        public string complete { get => $"{hio_dotnet.Common.Helpers.TimeHelpers.UnixTimestampToDateTime(ts)} - {value}"; }

        public double asDouble { get => double.Parse(value.ToString() ?? string.Empty, CultureInfo.InvariantCulture); }

        public bool isValueNumeric()
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value.ToString() ?? "", @"^[0-9]+(\.[0-9]+)?$");
        }
    }
}
