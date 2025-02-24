using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Common.Console.Dto
{
    public class AutomatedCommandsTab
    {
        public string Id { get; set; } = $"{Guid.NewGuid().ToString().Split('-')[0]}_ACT";
        public string Name { get; set; } = string.Empty;
        public List<AutomatedCommand> AutomatedCommands { get; set; } = new List<AutomatedCommand>();
    }
}
