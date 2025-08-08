using System;
using System.Collections.Generic;
using AstekUtility.Gameplay.Collision;
using UnityEngine;

namespace AstekUtility.Gameplay.CustomPhysics2D
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class KinematicPhysics2D : MonoBehaviour
	{
		[Header("Physics Properties")]
		[SerializeField] private float mass = 1f;
		[SerializeField] private float drag = 0.1f;
		[SerializeField] private float angularDrag = 0.05f;
		[SerializeField] private float gravityScale = 1f;
		[SerializeField] private bool useGravity = true;

		[Header("Movement Settings")]
		[SerializeField] [Range(1, 10)] private int subSteps = 3;

		[Header("Collision")]
		[SerializeField] private Collider2D ActiveCollider2D;
		[SerializeField] [Range(0.01f, 1f)] private float skinWidth = 0.05f;
		[SerializeField] private LayerMask collisionMask = ~0;

		[Header("Advanced")]
		[SerializeField] private bool enableRotation = true;
		[SerializeField] private bool slideOnCollision = true;
		#if UNITY_EDITOR
		[SerializeField] private bool showDebugGizmos = true;
		#endif

		// Physics state
		private Vector2 _velocity;
		private float _angularVelocity;
		private Vector2 _forceAccumulator;
		private float _torqueAccumulator;
		private float _momentOfInertia;

		// Components
		private Rigidbody2D _rb;
		private Transform _transform;

		// Collision data
		private RaycastHit2D[] _collisionBuffer;
		private ContactPoint2D[] _contactBuffer;

		// Debug visualization
		#if UNITY_EDITOR

		private const float GIZMO_DURATION = 2f;
		private readonly List<Vector2> _substepPositions = new List<Vector2>();
		private readonly List<Vector2> _substepVelocities = new List<Vector2>();

		#endif

		#region Getter And Setters

		public float Mass { get => mass; set => mass = value; }
		public float Drag { get => drag; set => drag = value; }
		public float AngularDrag { get => angularDrag; set => angularDrag = value; }
		public float GravityScale { get => gravityScale; set => gravityScale = value; }
		public bool UseGravity { get => useGravity; set => useGravity = value; }

		public int SubSteps { get => subSteps; set => subSteps = Math.Clamp(value, 1, int.MaxValue); }
		public float SkinWidth { get => skinWidth; set => skinWidth = value; }
		public LayerMask CollisionMask { get => collisionMask; set => collisionMask = value; }

		public bool EnableRotation { get => enableRotation; set => enableRotation = value; }
		public bool SlideOnCollision { get => slideOnCollision; set => slideOnCollision = value; }
		public Collider2D Collider { get => ActiveCollider2D; set => ActiveCollider2D = value; }

		public Vector2 Velocity { get => _velocity; set => _velocity = value; }
		public float AngularVelocity { get => _angularVelocity; set => _angularVelocity = value; }

		#endregion

		private void Awake()
		{
			//Cache if possible
			if (TryGetComponent(out Collider2D colliderComponent2D))
				ActiveCollider2D = colliderComponent2D;
			_transform = transform;
			_rb = GetComponent<Rigidbody2D>();

			#if UNITY_EDITOR

			// Initialize with current position
			_substepPositions.Add(_transform.position);

			#endif
		}

		private void FixedUpdate()
		{
			// Apply physics forces
			ApplyForces();

			#if UNITY_EDITOR

			// Clear debug data
			_substepPositions.Clear();
			_substepVelocities.Clear();
			_substepPositions.Add(_transform.position);

			#endif

			// Calculate frame movement
			Vector2 frameMovement = _velocity * Time.fixedDeltaTime;
			float frameRotation = _angularVelocity * Time.fixedDeltaTime;

			// Apply substepping
			ExecuteSubsteps(frameMovement, frameRotation);

			// Apply drag
			ApplyDrag();
		}

		/// <summary>
		/// Add force to the controller (in world space)
		/// </summary>
		public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
		{
			switch (mode)
			{
				case ForceMode2D.Force:
					_forceAccumulator += force;
					break;
				case ForceMode2D.Impulse:
					_velocity += force / mass;
					break;
			}
		}

		/// <summary>
		/// Add force at specific position (creating torque)
		/// </summary>
		public void AddForceAtPosition(Vector2 force, Vector2 position)
		{
			_forceAccumulator += force;

			if (enableRotation)
			{
				Vector2 leverArm = position - (Vector2)_transform.position;
				float torque = leverArm.x * force.y - leverArm.y * force.x;
				_torqueAccumulator += torque;
			}
		}

		/// <summary>
		/// Add torque to the controller (in Newton-meters)
		/// </summary>
		public void AddTorque(float torque)
		{
			_torqueAccumulator += torque;
		}

		private void ApplyForces()
		{
			// Apply gravity
			if (useGravity)
			{
				_forceAccumulator += Physics2D.gravity * (mass * gravityScale);
			}

			// Apply accumulated forces to velocity
			if (mass > Mathf.Epsilon)
			{
				_velocity += _forceAccumulator / mass * Time.fixedDeltaTime;
			}

			// Apply torque to angular velocity
			if (enableRotation)
			{
				// Simplified moment of inertia (adjust based on your collider shape)
				_momentOfInertia = GetMomentOfInertia();

				if (_momentOfInertia > Mathf.Epsilon)
				{
					_angularVelocity += _torqueAccumulator / _momentOfInertia * Time.fixedDeltaTime;
				}
			}

			// Reset accumulators
			_forceAccumulator = Vector2.zero;
			_torqueAccumulator = 0f;
		}

		#region Moment of inertia Calculations

		private float GetMomentOfInertia()
		{
			if (ActiveCollider2D is BoxCollider2D boxCollider)
				return CalculateBoxInertia(boxCollider);
			if (ActiveCollider2D is CircleCollider2D circleCollider)
				return CalculateCircleInertia(circleCollider);
			if (ActiveCollider2D is CapsuleCollider2D capsuleCollider)
				return CalculateCapsuleInertia(capsuleCollider);

			return mass * (ActiveCollider2D.bounds.size.x * ActiveCollider2D.bounds.size.x + ActiveCollider2D.bounds.size.y * ActiveCollider2D.bounds.size.y) / 12f;
		}
		
		private float CalculateBoxInertia(BoxCollider2D box)
		{
			Vector2 size = box.size * GetScale();
			// I = (1/12) * m * (w² + h²)
			return mass * (size.x * size.x + size.y * size.y) / 12f;
		}
		private float CalculateCircleInertia(CircleCollider2D circle)
		{
			float radius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
			// I = (1/2) * m * r²
			return 0.5f * mass * radius * radius;
		}
		private float CalculateCapsuleInertia(CapsuleCollider2D capsule)
		{
			Vector2 size = capsule.size * GetScale();
			float radius, length;

			// Extract radius and length based on capsule direction
			if (capsule.direction == CapsuleDirection2D.Vertical)
			{
				radius = size.x / 2f;
				length = Mathf.Max(0, size.y - size.x); // Straight portion length
			}
			else
			{
				radius = size.y / 2f;
				length = Mathf.Max(0, size.x - size.y); // Straight portion length
			}

			// Mass distribution (approximate)
			float totalLength = length + 2 * radius;
			float rectMass = mass * (length / totalLength);
			float capMass = mass * (2 * radius / totalLength) / 2f; // Half for each cap

			// Calculate components
			float rectInertia = rectMass * (4 * radius * radius + length * length) / 12f;
			float capInertia = capMass * (0.5f * radius * radius + 0.25f * length * length);

			// Total inertia = rectangle + 2 end caps
			return rectInertia + 2 * capInertia;
		}

		private Vector2 GetScale()
		{
			return new Vector2(
				Mathf.Abs(transform.lossyScale.x),
				Mathf.Abs(transform.lossyScale.y)
			);
		}

		#endregion

		private void ApplyDrag()
		{
			// Apply linear drag
			_velocity *= Mathf.Clamp01(1f - drag * Time.fixedDeltaTime);

			// Apply angular drag
			if (enableRotation)
			{
				_angularVelocity *= Mathf.Clamp01(1f - angularDrag * Time.fixedDeltaTime);
			}
		}

		private void ExecuteSubsteps(Vector2 frameMovement, float frameRotation)
		{
			Vector2 substepMovement = frameMovement / subSteps;
			float substepRotation = frameRotation / subSteps;

			for (int i = 0; i < subSteps; i++)
			{
				// Apply rotation
				if (enableRotation && Mathf.Abs(substepRotation) > Mathf.Epsilon)
				{
					_transform.Rotate(0, 0, substepRotation);
				}

				// Apply movement with collision detection
				if (substepMovement.sqrMagnitude > Mathf.Epsilon)
				{
					MoveWithCollision(substepMovement);
				}

				// Store step for visualization
				_substepPositions.Add(_transform.position);
				_substepVelocities.Add(_velocity);
			}
		}

		private void MoveWithCollision(Vector2 movement)
		{
			Vector2 direction = movement.normalized;
			float distance = movement.magnitude;

			// Early out for negligible movement
			if (distance < 0.001f) return;

			// Perform collision detection
			//_collisionBuffer = CollisionCast2D.CastAll(ActiveCollider2D, movement, collisionMask);

			// Process collisions
			float remainingDistance = distance;
			Vector2 currentPosition = _transform.position;
			Vector2 moveDirection = direction;

			for (int i = 0; i < _collisionBuffer.Length && remainingDistance > 0.001f; i++)
			{
				RaycastHit2D hit = _collisionBuffer[i];

				// Skip triggers and self-collisions
				if (hit.collider.isTrigger || hit.collider == ActiveCollider2D) continue;

				// Calculate hit distance with skin width
				float hitDistance = Mathf.Max(0, hit.distance - skinWidth);

				// Move to hit point
				currentPosition += moveDirection * hitDistance;
				_transform.position = currentPosition;

				// Update remaining distance
				remainingDistance -= hitDistance;

				// Handle collision response
				if (slideOnCollision)
				{
					// Get collision normal
					int contactCount = hit.collider.GetContacts(_contactBuffer);
					Vector2 normal = Vector2.zero;

					if (contactCount > 0)
					{
						foreach (ContactPoint2D contact in _contactBuffer)
						{
							if (contact.collider == hit.collider)
							{
								normal = contact.normal;
								break;
							}
						}
					}
					else
					{
						normal = hit.normal;
					}

					// Calculate slide direction
					moveDirection = Vector2.Reflect(moveDirection, normal).normalized;

					// Trigger collision events
					OnCollisionDetected(hit.collider, hit.point, normal);
				}
				else
				{
					// Stop movement on collision
					remainingDistance = 0;
					OnCollisionDetected(hit.collider, hit.point, hit.normal);
					break;
				}
			}

			// Apply remaining movement
			if (remainingDistance > 0)
			{
				_rb.position = currentPosition + moveDirection * remainingDistance;
			}
		}

		private void OnCollisionDetected(Collider2D other, Vector2 point, Vector2 normal)
		{
			// You can override this method for custom collision handling
			// Example: Damage system, sound effects, particle effects

			// Push dynamic rigidbodies
			Rigidbody2D rb = other.attachedRigidbody;
			if (rb && rb.bodyType != RigidbodyType2D.Kinematic)
			{
				Vector2 force = normal * (_velocity.magnitude * mass);
				rb.AddForceAtPosition(force, point, ForceMode2D.Impulse);
			}
		}

		#region Debug Visualization

		#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			if (!showDebugGizmos || !Application.isPlaying) return;

			// Draw substep positions
			Gizmos.color = Color.green;
			for (int i = 0; i < _substepPositions.Count; i++)
			{
				Gizmos.DrawSphere(_substepPositions[i], 0.05f);

				// Draw velocity vectors
				if (i < _substepVelocities.Count && i > 0)
				{
					Vector2 start = _substepPositions[i];
					Vector2 velocity = _substepVelocities[i];
					Vector2 direction = velocity.normalized;
					float magnitude = Mathf.Clamp01(velocity.magnitude / 10f);

					// Color based on speed (blue = slow, red = fast)
					Color velocityColor = Color.Lerp(Color.blue, Color.red, magnitude);
					Debug.DrawRay(start, direction * 0.5f, velocityColor, GIZMO_DURATION);
				}
			}

			// Draw collider bounds
			if (ActiveCollider2D != null)
			{
				Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
				Gizmos.DrawWireCube(ActiveCollider2D.bounds.center, ActiveCollider2D.bounds.size);
			}

			// Draw velocity vector
			Debug.DrawRay(transform.position, _velocity.normalized, Color.yellow, GIZMO_DURATION);

			// Draw angular velocity indicator
			if (enableRotation)
			{
				Vector3 rotationIndicator = Quaternion.Euler(0, 0, _angularVelocity * 0.1f) * Vector3.up * 0.7f;
				Debug.DrawRay(transform.position, rotationIndicator, Color.cyan, GIZMO_DURATION);
			}
		}

		#endif

		#endregion
	}
}