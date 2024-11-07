using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Models.DataSimulation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SimulationMeasurementAttribute : SimulationAttribute
    {
        /// <summary>
        /// For case when property is a list of items and we want to simulate each item separately
        /// </summary>
        public int NumberOfInsideItems { get; set; } = 1;

        /// <summary>
        /// Setup of parameters for automatic basic simulation of measurement
        /// </summary>
        /// <param name="isStatic">If the value should not be simulated set this true</param>
        /// <param name="minValue">Mimimum value for simulated value</param>
        /// <param name="maxValue">Maximum value for simulated value</param>
        /// <param name="needsFollowPrevious">Set this if value should follow previous simulated value</param>
        /// <param name="shouldRaise">When value follows some previous it can be higher or lower than previous each cycle. If this is true value starts at Min and then each cycle it raise upper and upper up to the Max value</param>
        /// <param name="maximumChange">Maximum value in percentage for example 5% is 0.05 value</param>
        /// <param name="numberOfInsideItems">If the property object contains list inside this will tell how many items will be simulated</param>
        public SimulationMeasurementAttribute(bool isStatic,
                                              double minValue = 0.0,
                                              double maxValue = 0.0,
                                              bool needsFollowPrevious = false,
                                              bool shouldRaise = false,
                                              double maximumChange = 0.0,
                                              int numberOfInsideItems = 1)
            : base(isStatic, minValue, maxValue, needsFollowPrevious, shouldRaise, maximumChange)
        {
            NumberOfInsideItems = numberOfInsideItems;
        }
    }
}
