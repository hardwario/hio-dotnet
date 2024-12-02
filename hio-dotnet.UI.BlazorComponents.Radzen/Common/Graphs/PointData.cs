using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.Radzen.Common.Graphs
{
    public class PointData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Value { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

    }
}
