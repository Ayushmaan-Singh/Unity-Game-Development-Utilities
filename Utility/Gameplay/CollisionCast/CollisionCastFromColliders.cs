using System;
using UnityEngine;

namespace AstekUtility.Gameplay.Collision
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
			Vector2 origin = (Vector2)collider2D.transform.position + collider2D.offset;

			switch (determineCast)
			{
				case CastType.Box:
					BoxCollider2D boxCollider = collider2D as BoxCollider2D;
					return Physics2D.BoxCast(
						origin,
						boxCollider.size * collider2D.transform.lossyScale,
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
						capsuleCollider.size,
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
			Vector2 origin = (Vector2)collider2D.transform.position + collider2D.offset;

			switch (determineCast)
			{
				case CastType.Box:
					BoxCollider2D boxCollider = collider2D as BoxCollider2D;
					return Physics2D.BoxCastAll(
						origin,
						boxCollider.size * collider2D.transform.lossyScale,
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
						capsuleCollider.size,
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

		public static void DrawGizmoForCast(Collider2D collider2D, Vector2 direction)
		{
			Vector2 origin = (Vector2)collider2D.transform.position + collider2D.offset;
			if (collider2D is CircleCollider2D circleCollider)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(origin + circleCollider.offset, circleCollider.radius * collider2D.transform.lossyScale.x / 3f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(origin + circleCollider.offset, circleCollider.radius * collider2D.transform.lossyScale.x);
			}
			else if (collider2D is BoxCollider2D boxCollider)
			{

				Gizmos.color = Color.green;
				Gizmos.DrawCube(origin + boxCollider.offset, boxCollider.size * collider2D.transform.lossyScale.x / 3f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(origin + boxCollider.offset, boxCollider.size * collider2D.transform.lossyScale.x);
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
}