using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public static class WMBusHelpers
    {
        public static DateTime ParseToUtc(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return DateTime.MinValue;

            string[] formats = {
            "yyyy-MM-dd'T'HH:mm",
            "yyyy-MM-dd HH:mm"
        };

            if (DateTime.TryParseExact(
                    input,
                    formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out DateTime localDateTime))
            {
                return localDateTime.ToUniversalTime();
            }

            return DateTime.MinValue;
        }
    }
}
