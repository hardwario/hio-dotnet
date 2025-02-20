using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.Interfaces
{
    public interface IRTTDriver
    {
        public bool IsConnected { get; set; }
        void ReconnectJLink();
        string ReadRtt(int bufferIndex);
        void WriteRtt(int bufferIndex, string message);
        void Close();
        void Dispose();
    }
}
