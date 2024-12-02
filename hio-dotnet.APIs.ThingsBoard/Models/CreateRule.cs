using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.ThingsBoard.Models
{
    public class CreateRule
    {
        [JsonPropertyName("condition")]
        public Condition Condition { get; set; } = new Condition();
        [JsonPropertyName("schedule")]
        public Schedule Schedule { get; set; } = new Schedule();

        [JsonPropertyName("alarmDetails")]
        public string AlarmDetails { get; set; } = string.Empty;
        [JsonPropertyName("dashboardId")]
        public EntityId DashboardId { get; set; } = new EntityId();
    }
}
