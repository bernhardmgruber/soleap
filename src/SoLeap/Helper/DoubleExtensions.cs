using System;

namespace SoLeap
{
    public static class DoubleExtensions
    {
        public static bool IsAlmost(this double value, double other, double epsilon = double.Epsilon)
        {
            return Math.Abs(value - other) < epsilon;
        }
    }
}
