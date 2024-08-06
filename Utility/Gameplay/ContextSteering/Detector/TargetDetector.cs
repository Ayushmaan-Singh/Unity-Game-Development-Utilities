using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class TargetDetector : Detector
	{
		[SerializeField] private LayerMask _targetLayerMask;

		//gizmo parameters
		[SerializeField] private bool _showIfTargetIsInVisualRange;
		private List<Transform> colliders;

		#if UNITY_EDITOR
		public override void OnDrawGizmos()
		{
			if (_showGizmos == false)
				return;

			if (colliders == null)
				return;
			Gizmos.color = _gizmoColor;
			foreach (Transform item in colliders)
			{
				Gizmos.DrawSphere(item.position, 0.4f);
			}
		}
		#endif

		public override void Detect()
		{
			//Find out if player is near
			List<Collider> playerCollider =
				Physics.OverlapSphere(_mainModel.position, _detectionRadius, _targetLayerMask).ToList();

			playerCollider.Sort((x, y) => Mathf.Sqrt((_mainModel.position - x.transform.position).sqrMagnitude).CompareTo(Mathf.Sqrt((_mainModel.position - x.transform.position).sqrMagnitude)));

			if (playerCollider != null && playerCollider.Count > 0)
			{
				//Check if you see the player
				Vector3 direction = playerCollider[0].transform.position - _mainModel.position;
				Physics.Raycast(_mainModel.position, direction.normalized, out RaycastHit hit, _detectionRadius, _obstacleLayerMask | _targetLayerMask);

				//Make sure that the collider we see is on the "Player" layer
				if (hit.collider != null && (_targetLayerMask & 1 << hit.collider.gameObject.layer) != 0)
				{
					if (_showIfTargetIsInVisualRange)
						Debug.DrawRay(_mainModel.position, direction.normalized * direction.magnitude, Color.magenta);
					colliders = new List<Transform>
					{
						playerCollider[0].transform
					};
				}
				else
				{
					colliders = null;
				}
			}
			else
			{
				//Enemy doesn't see the player
				colliders = null;
			}
			_aiData.Targets = colliders;
		}
	}
}