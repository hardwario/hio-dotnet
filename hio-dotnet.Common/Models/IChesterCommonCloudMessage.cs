using hio_dotnet.Common.Models.Common;
using hio_dotnet.Common.Models.DataSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models
{
    public interface IChesterCommonCloudMessage
    {
        Message Message { get; set; }
        ChesterAttribute Attribute { get; set; }
        ChesterSystem System { get; set; }
        Backup Backup { get; set; }
        Network Network { get; set; }
        Thermometer Thermometer { get; set; }
        Accelerometer Accelerometer { get; set; }
        Tamper? Tamper { get; set; }
    }
}
