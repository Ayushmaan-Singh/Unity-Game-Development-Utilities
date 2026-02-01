using System;

namespace Astek
{
    public static class FloatExtension
    {
        /// <summary>
        /// Return a float rounded to said Precision
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static float SetPrecision(this float value, int precision) => (float)Math.Round(value, precision);

        /// <summary>
        /// Recommended for comparing floating point numbers
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool Approximately(this float f1, float f2, uint tolerance = 4) => Math.Abs(f1 - f2) < Math.Pow(10, -tolerance);
    }
}