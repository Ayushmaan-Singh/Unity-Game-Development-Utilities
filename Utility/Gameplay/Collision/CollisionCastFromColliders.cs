using System;
using UnityEngine;

namespace Astek.Gameplay.Collision
{
	public static class CollisionCast2D
	{
		private enum CastType
		{
			Invalid,
			Box,
			Circle,
			Capsule
		}

		public static RaycastHit2D Cast(Collider2D collider2D, Vector2 direction, LayerMask collisionMask)
		{
			CastType determineCast = DetermineCast(collider2D);
			Vector2 origin = collider2D.bounds.center;

			switch (determineCast)
			{
				case CastType.Box:
					BoxCollider2D boxCollider = collider2D as BoxCollider2D;
					return Physics2D.BoxCast(
						origin,
						boxCollider.bounds.size,
						collider2D.transform.eulerAngles.z,
						direction.normalized,
						direction.magnitude,
						collisionMask);

				case CastType.Circle:
					CircleCollider2D circleCollider = collider2D as CircleCollider2D;
					return Physics2D.CircleCast(
						origin,
						circleCollider.radius * collider2D.transform.lossyScale.x,
						direction.normalized,
						direction.magnitude,
						collisionMask);

				case CastType.Capsule:
					CapsuleCollider2D capsuleCollider = collider2D as CapsuleCollider2D;
					return Physics2D.CapsuleCast(
						origin,
						capsuleCollider.bounds.size,
						capsuleCollider.direction,
						collider2D.transform.eulerAngles.z,
						direction,
						direction.magnitude,
						collisionMask);

				default:
					throw new ArgumentNullException($"{collider2D} is not within defined cast type");
			}
		}
		public static RaycastHit2D[] CastAll(Collider2D collider2D, Vector2 direction, LayerMask collisionMask)
		{
			CastType determineCast = DetermineCast(collider2D);
			Vector2 origin = collider2D.bounds.center;

			switch (determineCast)
			{
				case CastType.Box:
					BoxCollider2D boxCollider = collider2D as BoxCollider2D;
					return Physics2D.BoxCastAll(
						origin,
						boxCollider.bounds.size,
						collider2D.transform.eulerAngles.z,
						direction.normalized,
						direction.magnitude,
						collisionMask);

				case CastType.Circle:
					CircleCollider2D circleCollider = collider2D as CircleCollider2D;
					return Physics2D.CircleCastAll(
						origin,
						circleCollider.radius * collider2D.transform.lossyScale.x,
						direction.normalized,
						direction.magnitude,
						collisionMask);

				case CastType.Capsule:
					CapsuleCollider2D capsuleCollider = collider2D as CapsuleCollider2D;
					return Physics2D.CapsuleCastAll(
						origin,
						capsuleCollider.bounds.size,
						capsuleCollider.direction,
						collider2D.transform.eulerAngles.z,
						direction,
						direction.magnitude,
						collisionMask);

					break;

				default:
					throw new ArgumentNullException($"{collider2D} is not within defined cast type");
			}
		}
		public static RaycastHit2D[] CastAll(Collider2D collider2D, Vector2 direction, float distance, LayerMask collisionMask)
		{
			CastType determineCast = DetermineCast(collider2D);
			Vector2 origin = collider2D.bounds.center;

			switch (determineCast)
			{
				case CastType.Box:
					BoxCollider2D boxCollider = collider2D as BoxCollider2D;
					return Physics2D.BoxCastAll(
						origin,
						boxCollider.bounds.size,
						collider2D.transform.eulerAngles.z,
						direction.normalized,
						distance,
						collisionMask);

				case CastType.Circle:
					CircleCollider2D circleCollider = collider2D as CircleCollider2D;
					return Physics2D.CircleCastAll(
						origin,
						circleCollider.radius * collider2D.transform.lossyScale.x,
						direction.normalized,
						distance,
						collisionMask);

				case CastType.Capsule:
					CapsuleCollider2D capsuleCollider = collider2D as CapsuleCollider2D;
					return Physics2D.CapsuleCastAll(
						origin,
						capsuleCollider.bounds.size,
						capsuleCollider.direction,
						collider2D.transform.eulerAngles.z,
						direction.normalized,
						distance,
						collisionMask);

					break;

				default:
					throw new ArgumentNullException($"{collider2D} is not within defined cast type");
			}
		}
		public static RaycastHit2D[] CastAll(Collider2D collider2D, Vector2 origin,Vector2 direction, float distance, LayerMask collisionMask)
		{
			CastType determineCast = DetermineCast(collider2D);

			switch (determineCast)
			{
				case CastType.Box:
					BoxCollider2D boxCollider = collider2D as BoxCollider2D;
					return Physics2D.BoxCastAll(
						origin,
						boxCollider.bounds.size,
						collider2D.transform.eulerAngles.z,
						direction.normalized,
						distance,
						collisionMask);

				case CastType.Circle:
					CircleCollider2D circleCollider = collider2D as CircleCollider2D;
					return Physics2D.CircleCastAll(
						origin,
						circleCollider.radius * collider2D.transform.lossyScale.x,
						direction.normalized,
						distance,
						collisionMask);

				case CastType.Capsule:
					CapsuleCollider2D capsuleCollider = collider2D as CapsuleCollider2D;
					return Physics2D.CapsuleCastAll(
						origin,
						capsuleCollider.bounds.size,
						capsuleCollider.direction,
						collider2D.transform.eulerAngles.z,
						direction,
						distance,
						collisionMask);

					break;

				default:
					throw new ArgumentNullException($"{collider2D} is not within defined cast type");
			}
		}

		private static CastType DetermineCast(Collider2D collider2D)
		{
			if (collider2D is BoxCollider2D)
				return CastType.Box;
			if (collider2D is CircleCollider2D)
				return CastType.Circle;
			if (collider2D is CapsuleCollider2D)
				return CastType.Capsule;

			return CastType.Invalid;
		}
	}
	public static class CollisionCast
	{
		private enum CastType
		{
			Invalid,
			Box,
			Sphere,
			Capsule
		}

		public static RaycastHit Cast(Collider collider, Vector3 direction, LayerMask collisionMask)
		{
			if (collider == null)
				throw new ArgumentNullException(nameof(collider));

			//Physics.SyncTransform() might need this if update problems occur
			CastType determineCast = DetermineCast(collider);
			Vector3 origin = collider.bounds.center;
			RaycastHit hit = default(RaycastHit);

			switch (determineCast)
			{
				case CastType.Box:
					if (collider is BoxCollider boxCollider)
						Physics.BoxCast(
							origin,
							boxCollider.bounds.size * 0.5f,
							direction.normalized,
							out hit,
							boxCollider.transform.rotation,
							direction.magnitude,
							collisionMask);
					break;

				case CastType.Sphere:
					if (collider is SphereCollider sphereCollider)
						Physics.SphereCast(
							origin,
							sphereCollider.radius * collider.transform.lossyScale.x,
							direction.normalized,
							out hit,
							direction.magnitude,
							collisionMask);
					break;

				case CastType.Capsule:
					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					GetCapsulePoints(capsuleCollider, origin, collider.transform.rotation, out Vector3 point1, out Vector3 point2);
					float radius = GetScaledCapsuleRadius(capsuleCollider);
					Physics.CapsuleCast(
						point1,
						point2,
						radius,
						direction,
						out hit,
						direction.magnitude,
						collisionMask);
					break;

				default:
					throw new ArgumentNullException($"{collider} is not within defined cast type");
			}
			return hit;
		}
		public static RaycastHit[] CastAll(Collider collider, Vector3 direction, LayerMask collisionMask)
		{
			if (collider == null)
				throw new ArgumentNullException(nameof(collider));

			CastType determineCast = DetermineCast(collider);
			Vector3 origin = collider.bounds.center;

			switch (determineCast)
			{
				case CastType.Box:
					if (collider is BoxCollider boxCollider)
						return Physics.BoxCastAll(
							origin,
							boxCollider.bounds.size * 0.5f,
							direction.normalized,
							boxCollider.transform.rotation,
							direction.magnitude,
							collisionMask);
					break;

				case CastType.Sphere:
					if (collider is SphereCollider sphereCollider)
						return Physics.SphereCastAll(
							origin,
							sphereCollider.radius * collider.transform.lossyScale.x,
							direction.normalized,
							direction.magnitude,
							collisionMask);
					break;

				case CastType.Capsule:
					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					GetCapsulePoints(capsuleCollider, origin, collider.transform.rotation, out Vector3 point1, out Vector3 point2);
					float radius = GetScaledCapsuleRadius(capsuleCollider);
					return Physics.CapsuleCastAll(
						point1,
						point2,
						radius,
						direction,
						direction.magnitude,
						collisionMask);
					break;

				default:
					throw new ArgumentNullException($"{collider} is not within defined cast type");
			}
			return Array.Empty<RaycastHit>();
		}

		#region Helper

		//Capsule Collider
		private static void GetCapsulePoints(CapsuleCollider capsule, Vector3 origin, Quaternion rotation, out Vector3 point1, out Vector3 point2)
		{
			Vector3 center = capsule.bounds.center;
			Vector3 scale = capsule.transform.lossyScale;
			float height = capsule.height * GetAxisScale(scale, capsule.direction);
			float radius = GetScaledCapsuleRadius(capsule);

			// Calculate cylinder height without end caps
			float cylinderHeight = Mathf.Max(height - 2 * radius, 0);
			float halfCylinderHeight = cylinderHeight * 0.5f;

			// Calculate local axis based on capsule direction
			Vector3 localAxis = capsule.direction switch
			{
				0 => Vector3.right,   // X-axis
				1 => Vector3.up,      // Y-axis
				2 => Vector3.forward, // Z-axis
				_ => Vector3.up
			};

			// Convert to world space
			Vector3 worldAxis = rotation * localAxis;

			// Calculate endpoints
			Vector3 worldCenter = origin + rotation * center;
			point1 = worldCenter + worldAxis * halfCylinderHeight;
			point2 = worldCenter - worldAxis * halfCylinderHeight;
		}
		private static float GetScaledCapsuleRadius(CapsuleCollider capsule)
		{
			Vector3 scale = capsule.transform.lossyScale;
			float radiusScale = capsule.direction switch
			{
				0 => Mathf.Max(scale.y, scale.z), // X-axis: use Y and Z
				1 => Mathf.Max(scale.x, scale.z), // Y-axis: use X and Z
				2 => Mathf.Max(scale.x, scale.y), // Z-axis: use X and Y
				_ => Mathf.Max(scale.x, scale.y, scale.z)
			};
			return capsule.radius * radiusScale;
		}

		private static float GetAxisScale(Vector3 scale, int axis)
		{
			return axis switch
			{
				0 => scale.x, // X-axis
				1 => scale.y, // Y-axis
				2 => scale.z, // Z-axis
				_ => Mathf.Max(scale.x, scale.y, scale.z)
			};
		}

		#endregion

		private static CastType DetermineCast(Collider collider)
		{
			if (collider is BoxCollider)
				return CastType.Box;
			if (collider is SphereCollider)
				return CastType.Sphere;
			if (collider is CapsuleCollider)
				return CastType.Capsule;

			return CastType.Invalid;
		}
	}
}