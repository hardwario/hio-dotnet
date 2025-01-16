using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public static class OpenedTabFactory
    {
        public static IOpenedTab? GetOpenedTab(OpenedTabType openedTabType)
        {
            switch (openedTabType)
            {
                case OpenedTabType.Device:
                    return new DeviceOpenedTab();
                case OpenedTabType.None:
                    return null;
                default:
                    return null;
            }
        }
    }
}
