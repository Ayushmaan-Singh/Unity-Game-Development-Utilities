#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;

namespace AstekUtility.Gameplay.Collision
{
	public static class OverlapCollisionExtension
	{
		private enum CastType
		{
			Invalid,
			Box,
			Sphere,
			Capsule
		}

		private enum CastType2D
		{
			Invalid,
			Box,
			Circle,
			Capsule
		}


		public static Collider[] OverlapCollision(this Collider collider, LayerMask collisionMask)
		{
			if (!collider)
				throw new ArgumentNullException(nameof(collider));

			Vector3 origin = collider.bounds.center;

			switch (DetermineCast(collider))
			{
				case CastType.Box:

					BoxCollider boxCollider = collider as BoxCollider;
					return Physics.OverlapBox(
						origin,
						boxCollider.bounds.size * 0.5f,
						boxCollider.transform.rotation,
						collisionMask);

				case CastType.Sphere:

					SphereCollider sphereCollider = collider as SphereCollider;
					return Physics.OverlapSphere(
						origin,
						sphereCollider.radius * sphereCollider.transform.lossyScale.x,
						collisionMask);

				case CastType.Capsule:

					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					GetCapsulePoints(capsuleCollider, origin, collider.transform.rotation, out Vector3 point1, out Vector3 point2);
					float radius = GetScaledCapsuleRadius(capsuleCollider);
					return Physics.OverlapCapsule(
						point1,
						point2,
						radius,
						collisionMask);

				default:
					throw new ArgumentOutOfRangeException();
			}
			return null;
		}

		public static Collider2D OverlapCollision2D(this Collider2D collider, LayerMask collisionMask)
		{
			if (!collider)
				throw new ArgumentNullException(nameof(collider));

			Vector3 origin = collider.bounds.center;

			switch (DetermineCast(collider))
			{
				case CastType2D.Box:

					BoxCollider2D boxCollider = collider as BoxCollider2D;
					return Physics2D.OverlapBox(
						origin,
						boxCollider.bounds.size,
						boxCollider.transform.rotation.z,
						collisionMask);

				case CastType2D.Circle:

					CircleCollider2D sphereCollider = collider as CircleCollider2D;
					return Physics2D.OverlapCircle(
						origin,
						sphereCollider.radius * sphereCollider.transform.lossyScale.x,
						collisionMask);

				case CastType2D.Capsule:

					CapsuleCollider2D capsuleCollider = collider as CapsuleCollider2D;
					return Physics2D.OverlapCapsule(
						origin,
						capsuleCollider.bounds.size,
						capsuleCollider.direction,
						collisionMask);

				default:
					throw new ArgumentOutOfRangeException();
			}
			return null;
		}
		public static Collider2D[] OverlapCollisionAll2D(this Collider2D collider, LayerMask collisionMask)
		{
			if (!collider)
				throw new ArgumentNullException(nameof(collider));

			Vector3 origin = collider.bounds.center;

			switch (DetermineCast(collider))
			{
				case CastType2D.Box:

					BoxCollider2D boxCollider = collider as BoxCollider2D;
					return Physics2D.OverlapBoxAll(
						origin,
						boxCollider.bounds.size,
						boxCollider.transform.rotation.z,
						collisionMask);

				case CastType2D.Circle:

					CircleCollider2D sphereCollider = collider as CircleCollider2D;
					return Physics2D.OverlapCircleAll(
						origin,
						sphereCollider.radius * sphereCollider.transform.lossyScale.x,
						collisionMask);

				case CastType2D.Capsule:

					CapsuleCollider2D capsuleCollider = collider as CapsuleCollider2D;
					return Physics2D.OverlapCapsuleAll(
						origin,
						capsuleCollider.bounds.size,
						capsuleCollider.direction,
						collisionMask);

				default:
					throw new ArgumentOutOfRangeException();
			}
			return null;
		}

		#if UNITY_EDITOR

		public static void DrawWireShapes(this Collider collider, Color color)
		{
			Gizmos.color = color;
			Gizmos.matrix = collider.transform.localToWorldMatrix;

			switch (DetermineCast(collider))
			{
				case CastType.Box:

					BoxCollider boxCollider = collider as BoxCollider;
					Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);

					break;

				case CastType.Sphere:

					SphereCollider sphereCollider = collider as SphereCollider;
					Gizmos.DrawWireSphere(sphereCollider.bounds.center, sphereCollider.radius * collider.transform.lossyScale.x);

					break;

				case CastType.Capsule:

					CapsuleCollider capsuleCollider = collider as CapsuleCollider;
					GetCapsulePoints(capsuleCollider, capsuleCollider.bounds.center, capsuleCollider.transform.rotation, out Vector3 p1, out Vector3 p2);
					DrawWireCapsule(p1, p2, GetScaledCapsuleRadius(capsuleCollider) * 1.01f);

					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void DrawWireCapsule(Vector3 point1, Vector3 point2, float radius)
		{
			Vector3 upOffset = point2 - point1;
			Vector3 up = upOffset.Equals(default) ? Vector3.up : upOffset.normalized;
			Quaternion orientation = Quaternion.FromToRotation(Vector3.up, up);
			Vector3 forward = orientation * Vector3.forward;
			Vector3 right = orientation * Vector3.right;
			// z axis
			Handles.DrawWireArc(point2, forward, right, 180, radius);
			Handles.DrawWireArc(point1, forward, right, -180, radius);
			Handles.DrawLine(point1 + right * radius, point2 + right * radius);
			Handles.DrawLine(point1 - right * radius, point2 - right * radius);
			// x axis
			Handles.DrawWireArc(point2, right, forward, -180, radius);
			Handles.DrawWireArc(point1, right, forward, 180, radius);
			Handles.DrawLine(point1 + forward * radius, point2 + forward * radius);
			Handles.DrawLine(point1 - forward * radius, point2 - forward * radius);
			// y axis
			Handles.DrawWireDisc(point2, up, radius);
			Handles.DrawWireDisc(point1, up, radius);
		}

		#endif

		#region Helper

		public static void GetCapsulePoints(CapsuleCollider capsule, Vector3 origin, Quaternion rotation, out Vector3 point1, out Vector3 point2)
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
			point1 = rotation * center + worldAxis * halfCylinderHeight;
			point2 = rotation * center - worldAxis * halfCylinderHeight;
		}
		public static float GetScaledCapsuleRadius(CapsuleCollider capsule)
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
		private static CastType2D DetermineCast(Collider2D collider)
		{
			if (collider is BoxCollider2D)
				return CastType2D.Box;
			if (collider is CircleCollider2D)
				return CastType2D.Circle;
			if (collider is CapsuleCollider2D)
				return CastType2D.Capsule;

			return CastType2D.Invalid;
		}

	}
}