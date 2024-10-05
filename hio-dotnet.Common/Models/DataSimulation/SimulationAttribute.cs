using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SimulationAttribute : Attribute
    {
        /// <summary>
        /// Is Set => Value will not be simulated with random values
        /// Typically used for Ids such as IMEI, IMSI, etc.
        /// </summary>
        public bool IsStatic { get; set; } = false;
        /// <summary>
        /// If value should be random this will be the minimum value
        /// </summary>
        public double MinValue { get; set; } = 0.0;
        /// <summary>
        /// If value should be random this will be the maximum value
        /// </summary>
        public double MaxValue { get; set; } = 0.0;
        /// <summary>
        /// Set if random value should follow previous value
        /// Useful for simulating real world data such as battery voltage which drops over time so it should not randomly jump up and down
        /// </summary>
        public bool NeedsFollowPrevious { get; set; } = false;
        /// <summary>
        /// Set if random following value should raise over time. If false it will decrease over the time
        /// </summary>
        public bool ShouldRaise { get; set; } = false;

        /// <summary>
        /// Maximum change per one raise/decrease step in random value in percentage
        /// If you will set this to 0.1 and random value is 10.0 then next value will be between 9.9 and 10.1
        /// </summary>
        public double MaximumChange { get; set; } = 0.0;

        public SimulationAttribute(bool isStatic, 
                                   double minValue = 0.0, 
                                   double maxValue = 0.0, 
                                   bool needsFollowPrevious = false, 
                                   bool shouldRaise = false,
                                   double maximumChange = 0.0)
        {
            IsStatic = isStatic;
            MinValue = minValue;
            MaxValue = maxValue;
            NeedsFollowPrevious = needsFollowPrevious;
            ShouldRaise = shouldRaise;
            MaximumChange = maximumChange;
        }
    }
}
