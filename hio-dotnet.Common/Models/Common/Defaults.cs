using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.Common
{
    public static class Defaults
    {
        public static string Unknown = "Unknown";
        public static string UnknownSerialNumber = "0000000000";
        public static string UnknownEventType = "UnknownEvent";
        public static string UnknownTamperState = "UnknownTamperState";

        public static string InactiveState = "inactive";
        public static string ActivatedState = "activated";
        public static string DeactivatedState = "deactivated";

        public static string BackupDisconnectedState = "disconnected";
        public static string BackupConnectedState = "connected";

        public static string PushClickedState = "clicked";
        public static string PushHoldState = "held";
    }
}
