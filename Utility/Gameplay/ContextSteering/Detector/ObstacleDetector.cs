using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class ObstacleDetector : Detector
	{
		public string _detectorName;
		private Collider[] _colliders;

		private void Start()
		{
			if (_aiData.AvoidedObjectCollection == null)
			{
				_aiData.AvoidedObjectCollection = new Dictionary<string, Collider[]>();
			}
			_aiData.AvoidedObjectCollection.Add(_detectorName, null);
		}

		#if UNITY_EDITOR
		public override void OnDrawGizmos()
		{
			if (_showGizmos == false)
				return;

			if (Application.isPlaying && _colliders != null)
			{
				Gizmos.color = _gizmoColor;
				foreach (Collider obstacleCollider in _colliders)
				{
					Gizmos.DrawSphere(obstacleCollider.transform.position, 0.4f);
				}
			}
		}
		#endif

		public override void Detect()
		{
			_colliders = Physics.OverlapSphere(_mainModel.position, _detectionRadius, _obstacleLayerMask);
			_aiData.AvoidedObjectCollection[_detectorName] = _colliders;
		}
	}
}