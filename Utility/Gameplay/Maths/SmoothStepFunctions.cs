using System;
using UnityEngine;
namespace AstekUtility
{
	//https://www.shadertoy.com/view/st2BRd
	public static class Smoothstep
	{
		public static float CubicPolynomial_Smoothstep(float x) => x * x * (3f - 2f * x);
		public static float QuarticPolynomial_Smoothstep(float x) => x.Pow(2) * (2f - x * x);
		public static float QuinticPolynomial_Smoothstep(float x) => x.Pow(3) * (x * (x * 6f - 15f) + 10f);
		public static float QuadraticRational_Smoothstep(float x) => x.Pow(2) / (2 * x.Pow(2) - 2 * x + 1f);
		public static float CubicRational_Smoothstep(float x) => x.Pow(3) / (3 * x.Pow(2) - 3 * x + 1);
		public static float Rational_Smoothstep(float x, float k) => x.Pow(k) / (x.Pow(k) + (1 - x).Pow(k));
		public static float PiecewisePolynomial_Smoothstep(float x, float k)
		{
			if (x < 0.5f)
				return 0.5f * (2 * x).Pow(k);

			return 1 - 0.5f * (2 * (1 - x)).Pow(k);
		}
		public static float PiecewisePolynomial_Smoothstep(float x)
		{
			if (x < 0.5f)
				return 2 * x.Pow(2);

			return 2 * x * (2 - x) - 1;
		}
		public static float Trigonometric_Smoothstep(float x) => 0.5f - 0.5f * Mathf.Cos(Mathf.PI * x);
	}
	public static class SmoothstepFuncDerivative
	{
		public static float CubicPolynomial_Derivative(float x) => 6f * x * (1f - x);
		public static float QuarticPolynomial_Derivative(float x) => 4f * x * (1f - x.Pow(2));
		public static float QuinticPolynomial_Derivative(float x) => 30f * x.Pow(2) * (x * (x - 2f) + 1f);
		public static float QuadraticRational_Derivative(float x) => 2 * x * (1 - x) / (2f * x * (x - 1f) + 1f).Pow(2);
		public static float CubicRational_Derivative(float x) => 3f * x.Pow(2) * (x * (x - 2f) + 1f) / (3f * x * (x - 1f) + 1f).Pow(2);
		public static float Rational_Derivative(float x, float k)
			=> -k * (1 - x).Pow(k) * x.Pow(k - 1f) / (x - 1f) / (x.Pow(k) + (1f - x).Pow(k)) / (x.Pow(k) + (1f - x).Pow(k));
		public static float PiecewisePolynomial_Derivative(float x, float k)
		{
			if (x < 0.5f)
				return k * (2 * x).Pow(k - 1f);

			return k * (2f * (1f - x)).Pow(k - 1f);
		}
		public static float PiecewisePolynomial_Derivative(float x)
		{
			if (x < 0.5f)
				return 4 * x;

			return 4 - 4 * x;
		}
		public static float Trigonometric_Derivative(float x) => 0.5f * Mathf.PI * Mathf.Sin(x * Mathf.PI);

	}
	public static class SmoothstepFuncInverse
	{
		public static float CubicPolynomial_Inverse(float x) => 0.5f - Mathf.Sin(Mathf.Asin(1f - 2f * x) / 3f);
		public static float QuarticPolynomial_Inverse(float x) => Mathf.Sqrt(1f - Mathf.Sqrt(1f - x));
		/// <summary>
		/// Does not have an inverse -1 suggest failure
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static float QuinticPolynomial_Inverse(float x) => -1;
		public static float QuadraticRational_Inverse(float x) => (x - Mathf.Sqrt(x * (1 - x))) / (2 * x - 1f);
		public static float CubicRational_Inverse(float x) => x.Pow(1f / 3f) / (x.Pow(1f / 3f) + (1 - x).Pow(1f / 3f));
		public static float Rational_Inverse(float x, float k) => x.Pow(1f / k) / (x.Pow(1f / k) + (1f - x).Pow(1f / k));
		public static float PiecewisePolynomial_Inverse(float x, float k)
		{
			if (x < 0.5f)
				return 0.5f * (2 * x).Pow(1f / k);

			return 1 - 0.5f * (2 * (1 - x).Pow(1 / k));
		}
		public static float PiecewisePolynomial_Inverse(float x)
		{
			if (x < 0.5f)
				return Mathf.Sqrt(0.5f * x);

			return 1 - Mathf.Sqrt(0.5f - 0.5f * x);
		}
		public static float Trigonometric_Inverse(float x) => Mathf.Acos(1 - 2 * x) / Mathf.PI;
	}
}