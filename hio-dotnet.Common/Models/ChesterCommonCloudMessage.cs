using hio_dotnet.Common.Models.Common;
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
        [JsonPropertyName("message")]
        public Message Message { get; set; } = new Message();

        [JsonPropertyName("attribute")]
        public ChesterAttribute Attribute { get; set; } = new ChesterAttribute();

        [JsonPropertyName("system")]
        public ChesterSystem System { get; set; } = new ChesterSystem();

        [JsonPropertyName("backup")]
        public Backup Backup { get; set; } = new Backup();

        [JsonPropertyName("network")]
        public Network Network { get; set; } = new Network();

        [JsonPropertyName("thermometer")]
        public Thermometer Thermometer { get; set; } = new Thermometer();

        [JsonPropertyName("accelerometer")]
        public Accelerometer Accelerometer { get; set; } = new Accelerometer();
    }
}
