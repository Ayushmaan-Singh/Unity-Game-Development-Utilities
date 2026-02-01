using UnityEngine;

namespace Astek
{
	public static class VectorExtension
	{
		#region Vector2

		/// <summary>
		/// Sets any x y _values of a Vector2
		/// </summary>
		public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
			=> new Vector2(x ?? vector.x, y ?? vector.y);

		/// <summary>
		/// Adds to any x y _values of a Vector2
		/// </summary>
		public static Vector2 Add(this Vector2 vector, float x = 0, float y = 0)
			=> new Vector2(vector.x + x, vector.y + y);

		/// <summary>
		/// Returns distance between 2 points using the more optimized method of root of squared magnitude method
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static float SqrMagnitudeDistance(this Vector2 source, Vector2 destination) => Mathf.Sqrt((source - destination).sqrMagnitude);

		/// <summary>
		/// Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
		/// </summary>
		/// <param name="current">The current Vector2 position</param>
		/// <param name="target">The Vector2 position to compare against</param>
		/// <param name="range">The range value to compare against</param>
		/// <returns>True if the current Vector2 is in the given range from the target Vector3, false otherwise</returns>
		public static bool InRangeOf(this Vector2 current, Vector2 target, float range) => (current - target).sqrMagnitude <= range * range;

		public static Vector2 SetPrecision(this Vector2 current, int precision) =>
			new Vector2(current.x.SetPrecision(precision), current.y.SetPrecision(precision));

		public static Vector3 ToVector3(this Vector2 current) => new Vector3(current.x, current.y, 0);

		#endregion

		#region Vector3
		
		/// <summary>
		/// Sets any x y z _values of a Vector3
		/// </summary>
		public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
			=> new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);

		/// <summary>
		///     Adds to any x y z _values of a Vector3
		/// </summary>
		public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
			=> new Vector3(vector.x + x, vector.y + y, vector.z + z);

		/// <summary>
		/// Returns distance between 2 points using the more optimized method of root of squared magnitude method
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public static float SqrMagnitudeDistance(this Vector3 source, Vector3 destination) => Mathf.Sqrt((source - destination).sqrMagnitude);

		/// <summary>
		///     Returns a Boolean indicating whether the current Vector3 is in a given range from another Vector3
		/// </summary>
		/// <param name="current">The current Vector3 position</param>
		/// <param name="target">The Vector3 position to compare against</param>
		/// <param name="range">The range value to compare against</param>
		/// <returns>True if the current Vector3 is in the given range from the target Vector3, false otherwise</returns>
		public static bool InRangeOf(this Vector3 current, Vector3 target, float range) => (current - target).sqrMagnitude <= range * range;

		public static Vector3 SetPrecision(this Vector3 current, int precision) =>
			new Vector3(current.x.SetPrecision(precision), current.y.SetPrecision(precision), current.z.SetPrecision(precision));
		
		public static Vector3 ToVector2(this Vector3 current) => new Vector2(current.x, current.y);

		#endregion
	}
}