using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class Condition
    {
        [JsonPropertyName("conditionDetails")]
        public List<ConditionDetail> ConditionDetails { get; set; } = new List<ConditionDetail>();
        [JsonPropertyName("spec")]
        public Spec Spec { get; set; } = new Spec();
    }
}
