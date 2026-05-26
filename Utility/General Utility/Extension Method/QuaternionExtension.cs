using UnityEngine;

namespace Astek
{
	public static class QuaternionExtensions
	{
		/// <summary>
		/// Measure if 2 quaternions are equal with provided tolerance.
		/// </summary>
		/// <param name="q1"></param>
		/// <param name="q2"></param>
		/// <param name="tolerance">Between 0 and 1</param>
		/// <returns></returns>
		public static bool IsRotationApproximatelySame(this Quaternion q1, Quaternion q2, float tolerance = 0f)
		{
			float dot = Quaternion.Dot(q1, q2);
			return Mathf.Abs(dot) > (1.0f - Mathf.Clamp(tolerance, 0, 1));
		}
	}
}