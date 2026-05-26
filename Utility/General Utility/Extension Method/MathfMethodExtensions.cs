using UnityEngine;
namespace Astek
{
	public static class MathfMethodExtensions
	{
		public static float Pow(this float x, float power) => Mathf.Pow(x, power);

		#region Explicit conversion

		public static int ToInt(this float x) => (int)x;

		#endregion
	}
}