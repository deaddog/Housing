using System;

namespace Housing.Models
{
    public class Area
    {
        public Area(double value, AreaUnits unit)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Area cannot be negative.");

            Value = value;
            Unit = unit;
        }

        public double Value { get; }
        public AreaUnits Unit { get; }
    }
}
