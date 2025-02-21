using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Common.Console.Dto
{
    public enum AutomatedCommandStates
    {
        None,
        WaitingBefore,
        Running,
        WaitingAfter,
        Done
    }
    public class AutomatedCommand
    {
        public string Id { get; set; } = $"{Guid.NewGuid().ToString().Split('-')[0]}_AC";
        public string Command { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DelayAfter { get; set; } = 0;
        public int DelayBefore { get; set; } = 0;
        public bool IsActive { get; set; } = false;
        public AutomatedCommandStates State { get; set; } = AutomatedCommandStates.None;
    }
}
