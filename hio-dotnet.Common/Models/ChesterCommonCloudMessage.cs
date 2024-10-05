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
    public class ChesterCommonCloudMessage
    {
        [SimulationAttribute(true)]
        [JsonPropertyName("message")]
        public Message Message { get; set; } = new Message();

        [SimulationAttribute(true)]
        [JsonPropertyName("attribute")]
        public ChesterAttribute Attribute { get; set; } = new ChesterAttribute();

        [SimulationAttribute(false)]
        [JsonPropertyName("system")]
        public ChesterSystem System { get; set; } = new ChesterSystem();

        [SimulationAttribute(false)]
        [JsonPropertyName("backup")]
        public Backup Backup { get; set; } = new Backup();

        [SimulationAttribute(false)]
        [JsonPropertyName("network")]
        public Network Network { get; set; } = new Network();

        [SimulationAttribute(false)]
        [JsonPropertyName("thermometer")]
        public Thermometer Thermometer { get; set; } = new Thermometer();

        [SimulationAttribute(false)]
        [JsonPropertyName("accelerometer")]
        public Accelerometer Accelerometer { get; set; } = new Accelerometer();

        [SimulationAttribute(true)]
        [JsonPropertyName("tamper")]
        public Tamper? Tamper { get; set; }
    }
}
