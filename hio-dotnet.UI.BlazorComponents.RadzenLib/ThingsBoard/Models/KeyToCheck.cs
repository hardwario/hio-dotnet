using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.UI.BlazorComponents.RadzenLib.ThingsBoard.Models
{
    public class KeyToCheck
    {
        public string Key { get; set; }
        public bool Checked { get; set; }
        public List<DataPoint>? DataPoints { get; set; }

        public double Average { get => GetAverage(); }

        public double Min { get => GetMin(); }
        public double Max { get => GetMax(); }

        public double Sum { get => GetSum(); }

        public double Median { get => GetMedian(); }


        public int Rounding(double value)
        {
            if (value <= 0)
                return 0;

            double log10Val = Math.Log10(value);
            int decimals = (int)Math.Ceiling(-log10Val);

            if (decimals < 0)
                decimals = 0;

            return decimals;
        }

        public double GetMin()
        {
            double value = DataPoints?.Min(x => x.asDouble) ?? 0;
            int decimals = Rounding(value);            
            return Math.Round(value, decimals);
        }

        public double GetMax()
        {
            double value = DataPoints?.Max(x => x.asDouble) ?? 0;
            int decimals = Rounding(value);
            return Math.Round(value, decimals);
        }

        public double GetAverage()
        {
            double value = DataPoints?.Average(x => x.asDouble) ?? 0;
            int decimals = Rounding(value);
            return Math.Round(value, decimals);
        }

        public double GetSum()
        {
            double value = DataPoints?.Sum(x => x.asDouble) ?? 0;
            int decimals = Rounding(value);
            return Math.Round(value, decimals);
        }

        public double GetMedian()
        {
            double value = CalcMedian(DataPoints);
            int decimals = Rounding(value);
            return Math.Round(value, decimals);
        }

        public double CalcMedian(List<DataPoint>? points)
        {
            if (points == null)
            {
                return 0;
            }
            if (points.Count == 0)
            {
                return 0;
            }
            points.Sort((a, b) => a.asDouble.CompareTo(b.asDouble));
            if (points.Count % 2 == 0)
            {
                return (points[points.Count / 2 - 1].asDouble + points[points.Count / 2].asDouble) / 2;
            }
            return points[points.Count / 2].asDouble;
        }
    }
}
