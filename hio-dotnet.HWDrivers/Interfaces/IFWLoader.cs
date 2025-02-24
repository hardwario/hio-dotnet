using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Interfaces
{
    public interface IFWLoader
    {
        bool LoadFirmware(string filename);
    }
}
