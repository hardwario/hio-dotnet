using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{
    public static class SortByFilters
    {
        public static string Id = "id";
        public static string Name = "name";
        public static string SerialNumber = "serial_number";
        public static string ExternalId = "external_id";
        public static string LastSeen = "last_seen";
        public static string SessionId = "session_id";

        //check if string is one of the values in class
        public static bool IsSortByFilter(string value)
        {
            return value == Id || value == Name || value == SerialNumber || value == ExternalId || value == LastSeen || value == SessionId;
        }

    }
}
