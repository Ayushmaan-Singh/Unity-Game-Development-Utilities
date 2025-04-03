using System;
using UnityEngine;

namespace AstekUtility
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

		public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
	}
}