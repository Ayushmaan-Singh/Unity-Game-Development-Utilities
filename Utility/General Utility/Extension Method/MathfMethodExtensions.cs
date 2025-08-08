using UnityEngine;
namespace AstekUtility
{
	public static class MathfMethodExtensions
	{
		public static float Pow(this float x, float power) => Mathf.Pow(x, power);
	}
}