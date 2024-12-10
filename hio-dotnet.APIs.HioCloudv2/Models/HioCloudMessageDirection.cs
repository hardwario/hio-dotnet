using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloudv2.Models
{
    public static class HioCloudMessageDirection
    {
        public static string Up = "up";
        public static string Down = "down";

        // is one of the string
        public static bool IsMessageDirection(string direction)
        {
            return direction == Up || direction == Down;
        }
    }
}
