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
