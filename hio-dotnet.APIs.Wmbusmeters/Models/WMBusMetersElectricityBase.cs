using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace hio_dotnet.APIs.Wmbusmeters.Models
{
    public class WMBusMetersElectricityBase : WMBusMetersCommon
    {
        [JsonPropertyName("total_energy_consumption_kw")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TotalEnergyConsumptionKw { get; set; }
        
        [JsonPropertyName("current_power_consumption_kw")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentEnergyConsumptionKw { get; set; }
        
        [JsonPropertyName("total_energy_production_kwh")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? TotalEnergyProductionKw { get; set; }
        
        [JsonPropertyName("current_power_production_kw")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? CurrentEnergyProductionKw { get; set; }
        
        [JsonPropertyName("voltage_at_phase_1_v")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? VoltageAtPhase1V { get; set; }

        [JsonPropertyName("voltage_at_phase_2_v")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? VoltageAtPhase2V { get; set; }

        [JsonPropertyName("voltage_at_phase_3_v")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? VoltageAtPhase3V { get; set; }
    }
}
