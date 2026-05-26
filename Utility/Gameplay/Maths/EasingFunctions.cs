using System;
using UnityEngine;

namespace Astek
{
	//https://easings.net/#easeInOutExpo
	public static class EasingFunctionImplementations
	{
		public static float ApplyEasingFunction(this float x, EasingFunction func)
		{
			float value = Mathf.Clamp01(x);

			switch (func)
			{
				case EasingFunction.Linear:
					return LinearFunc(value);

				case EasingFunction.EaseInSine:
					return EaseInSineFunc(value);

				case EasingFunction.EaseOutSine:
					return EaseOutSineFunc(value);

				case EasingFunction.EaseInOutSine:
					return EaseInOutSineFunc(value);

				case EasingFunction.EaseInQuad:
					return EaseInQuad(value);

				case EasingFunction.EaseOutQuad:
					return EaseOutQuad(value);

				case EasingFunction.EaseInOutQuad:
					return EaseInOutQuad(value);

				case EasingFunction.EaseInCubic:
					return EaseInCubic(value);

				case EasingFunction.EaseOutCubic:
					return EaseOutCubic(value);

				case EasingFunction.EaseInOutCubic:
					return EaseInOutCubic(value);

				case EasingFunction.EaseInQuart:
					return EaseInQuart(value);

				case EasingFunction.EaseOutQuart:
					return EaseOutQuart(value);

				case EasingFunction.EaseInOutQuart:
					return EaseInOutQuart(value);

				case EasingFunction.EaseInQuint:
					return EaseInQuint(value);

				case EasingFunction.EaseOutQuint:
					return EaseOutQuint(value);

				case EasingFunction.EaseInOutQuint:
					return EaseInOutQuint(value);

				case EasingFunction.EaseInExpo:
					return EaseInExponential(value);

				case EasingFunction.EaseOutExpo:
					return EaseOutExponential(value);

				case EasingFunction.EaseInOutExpo:
					return EaseInOutExponential(value);

				case EasingFunction.EaseInCirc:
					return EaseInCirc(value);

				case EasingFunction.EaseOutCirc:
					return EaseOutCirc(value);

				case EasingFunction.EaseInOutCirc:
					return EaseInOutCirc(value);

				case EasingFunction.EaseInBack:
					return EaseInBack(value);

				case EasingFunction.EaseOutBack:
					return EaseOutBack(value);

				case EasingFunction.EaseInOutBack:
					return EaseInOutBack(value);

				case EasingFunction.EaseInElastic:
					return EaseInElastic(value);

				case EasingFunction.EaseOutElastic:
					return EaseOutElastic(value);

				case EasingFunction.EaseInOutElastic:
					return EaseInOutElastic(value);

				case EasingFunction.EaseInBounce:
					return EaseInBounce(value);

				case EasingFunction.EaseOutBounce:
					return EaseOutBounce(value);

				case EasingFunction.EaseInOutBounce:
					return EaseInOutBounce(value);

				default:
					throw new ArgumentOutOfRangeException(nameof(func), func, null);
			}
		}

		private static float LinearFunc(float x) => Mathf.Clamp01(x);

		//Sine
		private static float EaseInSineFunc(float x) => 1f - Mathf.Cos(x * Mathf.PI / 2f);
		private static float EaseOutSineFunc(float x) => Mathf.Sin(x * Mathf.PI / 2f);
		private static float EaseInOutSineFunc(float x) => -(Mathf.Cos(x * Mathf.PI) - 1) / 2f;

		//Quadratic
		private static float EaseInQuad(float x) => x.Pow(2);
		private static float EaseOutQuad(float x) => 1f - (1 - x).Pow(2);
		private static float EaseInOutQuad(float x)
		{
			if (x < 0.5f)
				return 2 * x.Pow(2);

			return 1 - (-2 * x + 2).Pow(2) / 2f;
		}

		//Cubic
		private static float EaseInCubic(float x) => x.Pow(3);
		private static float EaseOutCubic(float x) => 1f - (1 - x).Pow(3);
		private static float EaseInOutCubic(float x)
		{
			if (x < 0.5f)
				return 4 * x.Pow(3);

			return 1 - (-2 * x + 2).Pow(3) / 2f;
		}

		//Quart
		private static float EaseInQuart(float x) => x.Pow(4);
		private static float EaseOutQuart(float x) => 1f - (1 - x).Pow(4);
		private static float EaseInOutQuart(float x)
		{
			if (x < 0.5f)
				return 8 * x.Pow(4);

			return 1 - (-2 * x + 2).Pow(4) / 2f;
		}

		//Quint
		private static float EaseInQuint(float x) => x.Pow(5);
		private static float EaseOutQuint(float x) => 1f - (1 - x).Pow(5);
		private static float EaseInOutQuint(float x)
		{
			if (x < 0.5f)
				return 16 * x.Pow(5);

			return 1 - (-2 * x + 2).Pow(5) / 2f;
		}

		//Exponential
		private static float EaseInExponential(float x)
		{
			if (x == 0)
				return 0;

			return 2f.Pow(10f * x - 10f);
		}
		private static float EaseOutExponential(float x)
		{
			if (Mathf.Approximately(x, 1f))
				return 1;

			return 1 - 2f.Pow(-10f * x);
		}
		private static float EaseInOutExponential(float x)
		{
			if (x == 0)
				return 0;
			if (Mathf.Approximately(x, 1))
				return 1;
			if (x < 0.5f)
				return 2f.Pow(20f * x - 10f) / 2f;

			return 2 - 2f.Pow(-20f * x + 10f) / 2f;
		}

		//Circ
		private static float EaseInCirc(float x) => 1 - Mathf.Sqrt(1 - x.Pow(2));
		private static float EaseOutCirc(float x) => Mathf.Sqrt(1 - (x - 1).Pow(2));
		private static float EaseInOutCirc(float x)
		{
			if (x < 0.5f)
				return 1 - Mathf.Sqrt(1 - (2 * x).Pow(2)) / 2f;

			return Mathf.Sqrt(1 - (-2 * x + 2).Pow(2)) / 2f;
		}

		//Back
		private static float c1 = 1.70158f;
		private static float EaseInBack(float x)
		{
			float c3 = c1 + 1;

			return c3 * x.Pow(3) - c1 * x.Pow(2);
		}
		private static float EaseOutBack(float x)
		{
			float c3 = c1 + 1;
			return 1 + c3 * (x - 1).Pow(3) + c1 * (x - 1).Pow(2);
		}
		private static float EaseInOutBack(float x)
		{
			float c2 = c1 * 1.525f;

			if (x < 0.5f)
				return (2 * x).Pow(2) * ((c2 + 1) * 2 * x - c2) / 2f;

			return ((2 * x - 2).Pow(2) * ((c2 + 1) * (2 * x - 2) + c2) + 2) / 2f;
		}

		//Elastic
		private static float EaseInElastic(float x)
		{
			float c4 = 2 * Mathf.PI / 3f;

			if (x == 0)
				return 0;
			if (Mathf.Approximately(x, 1))
				return 1;

			return -2f.Pow(10 * x - 10) * Mathf.Sin((10 * x - 10.75f) * c4);
		}
		private static float EaseOutElastic(float x)
		{
			float c4 = 2 * Mathf.PI / 3f;

			if (x == 0)
				return 0;
			if (Mathf.Approximately(x, 1))
				return 1;

			return 2f.Pow(-10 * x) * Mathf.Sin((10 * x - 0.75f) * c4) + 1;
		}
		private static float EaseInOutElastic(float x)
		{
			float c5 = 2 * Mathf.PI / 4.5f;

			if (x == 0)
				return 0;
			if (Mathf.Approximately(x, 1))
				return 1;
			if (x < 0.5f)
				return -2f.Pow(20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5) / 2f;

			return 2f.Pow(-20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5) / 2f + 1;
		}

		//Bounce
		private static float EaseInBounce(float x) => 1 - EaseOutBounce(1 - x);
		private static float EaseOutBounce(float x)
		{
			float n1 = 7.5625f;
			float d1 = 2.75f;

			if (x < 1 / d1)
				return n1 * Mathf.Pow(x, 2);
			if (x < 2 / d1)
				return n1 * (x -= 1.5f / d1) * x + 0.75f;
			if (x < 2.5f / d1)
				return n1 * (x -= 2.2f / d1) * x + 0.9375f;

			return n1 * (x -= 2.625f / d1) * x + 0.984375f;
		}
		private static float EaseInOutBounce(float x)
		{
			if (x < 0.5f)
				return 1 - EaseOutBounce(1 - 2 * x) / 2f;

			return 1 + EaseOutBounce(2 * x - 1) / 2f;
		}
	}

	public enum EasingFunction
	{
		Linear = 0,
		EaseInSine = 1,
		EaseOutSine = 2,
		EaseInOutSine = 3,
		EaseInQuad = 4,
		EaseOutQuad = 5,
		EaseInOutQuad = 6,
		EaseInCubic = 7,
		EaseOutCubic = 8,
		EaseInOutCubic = 9,
		EaseInQuart = 10,
		EaseOutQuart = 11,
		EaseInOutQuart = 12,
		EaseInQuint = 13,
		EaseOutQuint = 14,
		EaseInOutQuint = 15,
		EaseInExpo = 16,
		EaseOutExpo = 17,
		EaseInOutExpo = 18,
		EaseInCirc = 19,
		EaseOutCirc = 20,
		EaseInOutCirc = 21,
		EaseInBack = 22,
		EaseOutBack = 23,
		EaseInOutBack = 24,
		EaseInElastic = 25,
		EaseOutElastic = 26,
		EaseInOutElastic = 27,
		EaseInBounce = 28,
		EaseOutBounce = 29,
		EaseInOutBounce = 30
	}
}