using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace AstekUtility.Gameplay.Collision
{
	[System.Serializable]
	public class BatchRaycast
	{
		[SerializeField] private bool showGizmo;

		private NativeArray<RaycastCommand> _raycastCommands;
		private NativeArray<RaycastHit> _hitResults;

		public void Raycast(Vector3[] origins, Vector3[] directions, int layerMask, float maxDistance,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<RaycastHit[]> callback,
			int maxHitPerRaycast = 1)
		{
			int rayCount = origins.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_raycastCommands = new NativeArray<RaycastCommand>(rayCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < rayCount; i++)
				{
					_raycastCommands[i] = new RaycastCommand(origins[i], directions[i], parameters, maxDistance);
				}

				ExecuteRaycast(_raycastCommands, callback, maxHitPerRaycast);
				_raycastCommands.Dispose();
				_hitResults.Dispose();
			}
		}
		private void ExecuteRaycast(NativeArray<RaycastCommand> raycastCommands, Action<RaycastHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = raycastCommands.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<RaycastHit>(totalHitsNeeded, Allocator.TempJob))
			{
				#if UNITY_EDITOR
				if (showGizmo)
					raycastCommands.ForEach(raycastCommand =>
					{
						Debug.DrawLine(raycastCommand.from, raycastCommand.from + raycastCommand.direction * raycastCommand.distance, Color.red, 0.5f);
					});
				 #endif


				JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(raycastCommands, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					RaycastHit[] results = _hitResults.ToArray();

					#if UNITY_EDITOR
					if (showGizmo)
					{
						for (int i = 0; i < results.Length; i++)
						{
							if (results[i].collider)
							{
								Debug.DrawLine(raycastCommands[i].from, results[i].point, Color.green, 1f);
							}
						}
					}
					#endif

					callback?.Invoke(results);
				}
			}
		}
	}

	[System.Serializable]
	public class BatchSpherecast
	{
		private NativeArray<SpherecastCommand> _spherecastCommands;
		private NativeArray<RaycastHit> _hitResults;

		public void Spherecast(Vector3[] origins, float[] radius, Vector3[] directions, int layerMask, float maxDistance,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<RaycastHit[]> callback,
			int maxHitPerRaycast = 10)
		{
			int sphereCount = origins.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_spherecastCommands = new NativeArray<SpherecastCommand>(sphereCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < sphereCount; i++)
				{
					_spherecastCommands[i] = new SpherecastCommand(origins[i], radius[i], directions[i], parameters, maxDistance);
				}

				ExecuteSpherecast(_spherecastCommands, callback, maxHitPerRaycast);
				_spherecastCommands.Dispose();
				_hitResults.Dispose();
			}
		}

		private void ExecuteSpherecast(NativeArray<SpherecastCommand> raycastCommands, Action<RaycastHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = raycastCommands.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<RaycastHit>(totalHitsNeeded, Allocator.TempJob))
			{
				JobHandle raycastJobHandle = SpherecastCommand.ScheduleBatch(raycastCommands, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					RaycastHit[] results = _hitResults.RemoveWhere(result => !result.collider).ToArray();
					callback?.Invoke(results);
				}
			}
		}
	}

	[System.Serializable]
	public class BatchBoxcast
	{
		private NativeArray<BoxcastCommand> _boxcastCommand;
		private NativeArray<RaycastHit> _hitResults;

		public void BoxCast(Vector3[] origins, Vector3[] halfExtents, Quaternion[] orientations, Vector3[] directions, int layerMask, float maxDistance,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<RaycastHit[]> callback,
			int maxHitPerRaycast = 10)
		{
			int boxCount = origins.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_boxcastCommand = new NativeArray<BoxcastCommand>(boxCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < boxCount; i++)
				{
					_boxcastCommand[i] = new BoxcastCommand(origins[i], halfExtents[i], orientations[i], directions[i], parameters, maxDistance);
				}

				ExecuteBoxcast(_boxcastCommand, callback, maxHitPerRaycast);
				_boxcastCommand.Dispose();
				_hitResults.Dispose();
			}
		}

		private void ExecuteBoxcast(NativeArray<BoxcastCommand> raycastCommands, Action<RaycastHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = raycastCommands.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<RaycastHit>(totalHitsNeeded, Allocator.TempJob))
			{
				JobHandle raycastJobHandle = BoxcastCommand.ScheduleBatch(raycastCommands, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					RaycastHit[] results = _hitResults.RemoveWhere(result => !result.collider).ToArray();
					callback?.Invoke(results);
				}
			}
		}
	}

	[System.Serializable]
	public class BatchCapsulecast
	{
		private NativeArray<CapsulecastCommand> _capsulecastCommand;
		private NativeArray<RaycastHit> _hitResults;

		public void CapsuleCast((Vector3 point1, Vector3 point2)[] points, float[] radius, Vector3[] directions, int layerMask, float maxDistance,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<RaycastHit[]> callback,
			int maxHitPerRaycast = 10)
		{
			int boxCount = points.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_capsulecastCommand = new NativeArray<CapsulecastCommand>(boxCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < boxCount; i++)
				{
					_capsulecastCommand[i] = new CapsulecastCommand(points[i].point1, points[i].point2, radius[i], directions[i], parameters, maxDistance);
				}

				ExecuteCapsulecast(_capsulecastCommand, callback, maxHitPerRaycast);
				_capsulecastCommand.Dispose();
				_hitResults.Dispose();
			}
		}

		private void ExecuteCapsulecast(NativeArray<CapsulecastCommand> raycastCommands, Action<RaycastHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = raycastCommands.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<RaycastHit>(totalHitsNeeded, Allocator.TempJob))
			{
				JobHandle raycastJobHandle = CapsulecastCommand.ScheduleBatch(raycastCommands, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					RaycastHit[] results = _hitResults.RemoveWhere(result => !result.collider).ToArray();
					callback?.Invoke(results);
				}
			}
		}
	}

	[System.Serializable]
	public class BatchOverlapSphere
	{
		private NativeArray<OverlapSphereCommand> _overlapSphereCommand;
		private NativeArray<ColliderHit> _hitResults;

		public void OverlapSphere(Vector3[] origins, float[] radius, int layerMask,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<ColliderHit[]> callback,
			int maxHitPerRaycast = 10)
		{
			int sphereCount = origins.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_overlapSphereCommand = new NativeArray<OverlapSphereCommand>(sphereCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < sphereCount; i++)
				{
					_overlapSphereCommand[i] = new OverlapSphereCommand(origins[i], radius[i], parameters);
				}

				ExecuteOverlapSphere(_overlapSphereCommand, callback, maxHitPerRaycast);
				_overlapSphereCommand.Dispose();
				_hitResults.Dispose();
			}
		}

		private void ExecuteOverlapSphere(NativeArray<OverlapSphereCommand> overlapCommands, Action<ColliderHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = overlapCommands.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<ColliderHit>(totalHitsNeeded, Allocator.TempJob))
			{
				JobHandle raycastJobHandle = OverlapSphereCommand.ScheduleBatch(overlapCommands, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					ColliderHit[] results = _hitResults.RemoveWhere(result => !result.collider).ToArray();
					callback?.Invoke(results);
				}
			}
		}
	}

	[System.Serializable]
	public class BatchOverlapBox
	{
		private NativeArray<OverlapBoxCommand> _overlapboxCommands;
		private NativeArray<ColliderHit> _hitResults;

		public void OverlapBox(Vector3[] origins, Vector3[] halfExtents, Quaternion[] orientations, int layerMask,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<ColliderHit[]> callback,
			int maxHitPerRaycast = 10)
		{
			int boxCount = origins.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_overlapboxCommands = new NativeArray<OverlapBoxCommand>(boxCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < boxCount; i++)
				{
					_overlapboxCommands[i] = new OverlapBoxCommand(origins[i], halfExtents[i], orientations[i], parameters);
				}

				ExecuteOverlapBox(_overlapboxCommands, callback, maxHitPerRaycast);
				_overlapboxCommands.Dispose();
				_hitResults.Dispose();
			}
		}

		private void ExecuteOverlapBox(NativeArray<OverlapBoxCommand> overlapCommands, Action<ColliderHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = overlapCommands.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<ColliderHit>(totalHitsNeeded, Allocator.TempJob))
			{
				JobHandle raycastJobHandle = OverlapBoxCommand.ScheduleBatch(overlapCommands, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					ColliderHit[] results = _hitResults.RemoveWhere(result => !result.collider).ToArray();
					callback?.Invoke(results);
				}
			}
		}
	}

	[System.Serializable]
	public class BatchOverlapCapsule
	{
		private NativeArray<OverlapCapsuleCommand> _overlapcapsuleCommand;
		private NativeArray<ColliderHit> _hitResults;

		public void OverlapCapsule((Vector3 point1, Vector3 point2)[] points, float[] radius, int layerMask,
			bool hitBackfaces,
			bool hitTriggerColliders,
			bool hitMultiFace,
			Action<ColliderHit[]> callback,
			int maxHitPerRaycast = 10)
		{
			int boxCount = points.Length;
			QueryTriggerInteraction queryTriggerInteraction = hitTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

			using (_overlapcapsuleCommand = new NativeArray<OverlapCapsuleCommand>(boxCount, Allocator.TempJob))
			{
				QueryParameters parameters = new QueryParameters()
				{
					layerMask = layerMask,
					hitBackfaces = hitBackfaces,
					hitTriggers = queryTriggerInteraction,
					hitMultipleFaces = hitMultiFace
				};

				for (int i = 0; i < boxCount; i++)
				{
					_overlapcapsuleCommand[i] = new OverlapCapsuleCommand(points[i].point1, points[i].point2, radius[i], parameters);
				}

				ExecuteOverlapCapsule(_overlapcapsuleCommand, callback, maxHitPerRaycast);
				_overlapcapsuleCommand.Dispose();
				_hitResults.Dispose();
			}
		}

		private void ExecuteOverlapCapsule(NativeArray<OverlapCapsuleCommand> overlapCommand, Action<ColliderHit[]> callback, int maxHitsPerRaycast)
		{
			int totalHitsNeeded = overlapCommand.Length * maxHitsPerRaycast;

			using (_hitResults = new NativeArray<ColliderHit>(totalHitsNeeded, Allocator.TempJob))
			{
				JobHandle raycastJobHandle = OverlapCapsuleCommand.ScheduleBatch(overlapCommand, _hitResults, 2, maxHitsPerRaycast);
				raycastJobHandle.Complete();

				if (_hitResults.Length > 0)
				{
					ColliderHit[] results = _hitResults.RemoveWhere(result => !result.collider).ToArray();
					callback?.Invoke(results);
				}
			}
		}
	}
}