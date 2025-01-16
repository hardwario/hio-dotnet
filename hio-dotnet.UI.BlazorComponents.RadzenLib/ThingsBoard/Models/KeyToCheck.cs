using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public class KeyToCheck
    {
        public string Key { get; set; }
        public bool Checked { get; set; }
        public List<DataPoint>? DataPoints { get; set; }
    }
}
