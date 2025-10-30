using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.HioCloud.Models
{
    public static class HioCloudMessageType
    {
        public static string Data = "data";
        public static string Config = "config";
        public static string Shell = "shell";
        public static string Session = "session";
        public static string Stats = "stats";
        public static string Codec = "codec";

        // is one of the string
        public static bool IsMessageType(string type)
        {
            return type == Data || type == Config || type == Shell || type == Session || type == Stats || type == Codec;
        }
    }
}
