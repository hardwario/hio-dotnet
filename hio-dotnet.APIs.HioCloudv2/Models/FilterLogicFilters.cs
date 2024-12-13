using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud.Models
{
    public static class FilterLogicFilters
    {
        public static string Any = "any";
        public static string All = "all";

        // check if string is one of the values in class
        public static bool IsFilterLogicFilter(string value)
        {
            return value == Any || value == All;
        }
    }
}
