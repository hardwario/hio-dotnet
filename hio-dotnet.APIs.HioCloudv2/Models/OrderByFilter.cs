using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud.Models
{
    public static class OrderByFilter
    {
        public static string Ascending = "asc";
        public static string Descending = "desc";

        // check if string is one of the values in class
        public static bool IsOrderByFilter(string value)
        {
            return value == Ascending || value == Descending;
        }
    }
}
