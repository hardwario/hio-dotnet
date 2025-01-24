using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.Common.Models
{
    public class ActionCommandButtonData
    {
        public string ButtonText { get; set; } = "Action";
        public string DialogTitle { get; set; } = "Action";
        public string DialogDescription { get; set; } = "Are you sure you want to perform this action?";
        public bool IsWithDialog { get; set; } = false;
        public List<string> Commands { get; set; } = new List<string>();

        public int PositionX { get; set; } = 0;
        public int PositionY { get; set; } = 0;
    }
}
