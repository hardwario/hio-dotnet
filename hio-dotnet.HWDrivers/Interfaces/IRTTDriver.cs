using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Interfaces
{
    public interface IRTTDriver
    {
        void ReconnectJLink();
        string ReadRtt(int bufferIndex);
        void WriteRtt(int bufferIndex, string message);
        void Close();
        void Dispose();
    }
}
