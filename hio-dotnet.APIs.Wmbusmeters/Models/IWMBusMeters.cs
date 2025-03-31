using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public interface IWMBusMeters
    {
        string Id { get; set; }
        string Name { get; set; }

        string Meter { get; set; } 
        string Media { get; set; }
    }
}
