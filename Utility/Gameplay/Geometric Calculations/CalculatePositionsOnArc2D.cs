using System;
using System.Collections.Generic;
using Gameplay.Strategem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace AstekUtility.Gameplay.Geometric_Calculations
{
	[Serializable]
	public class CalculatePositionsOnArc2D
	{
		private int _pointsOnArc = 5;

		//Arc Properties
		private Vector2 _centerPosition;
		private Vector2 _arcCenterPosition;
		private float _angleOffset;
		private float _arcAngle;
		private float _radius;

		[ShowInInspector] public int PointsOnArc
		{
			get => _pointsOnArc;
			set
			{
				_pointsOnArc = value;
				PreCalculateCardPositionsAndTransformUp();
			}
		}
		[ShowInInspector, PropertyTooltip("Center Point Of Arc")] public Vector2 CenterPosition
		{
			get => _centerPosition;
			set
			{
				_centerPosition = value;
				PreCalculateCardPositionsAndTransformUp();
			}
		}
		[ShowInInspector, PropertyTooltip("Desired Center Point Of Arc")] public Vector2 ArcCenterPosition
		{
			get => _arcCenterPosition;
			set
			{
				_arcCenterPosition = value;
				PreCalculateCardPositionsAndTransformUp();
			}
		}
		[ShowInInspector] public float AngleOffset
		{
			get => _angleOffset;
			set
			{
				_angleOffset = value;
				PreCalculateCardPositionsAndTransformUp();
			}
		}
		[ShowInInspector, PropertyRange(0f, 360f)] public float ArcAngle
		{
			get => _arcAngle;
			set
			{
				_arcAngle = value;
				PreCalculateCardPositionsAndTransformUp();
			}
		}
		[ShowInInspector] public float Radius
		{
			get => _radius;
			set
			{
				_radius = value;
				PreCalculateCardPositionsAndTransformUp();
			}
		}


		[ShowInInspector, EnableIf("@false")] public readonly List<Vector2> _position = new List<Vector2>();
		[ShowInInspector, EnableIf("@false")] public readonly List<Vector2> _up = new List<Vector2>();

		private void PreCalculateCardPositionsAndTransformUp()
		{
			float startAngle = -_arcAngle / 2;
			float angleStep = _arcAngle / (_pointsOnArc - 1);

			Vector2 centroid = Vector2.zero;

			//Calculate Centroid
			for (int i = 0; i < _pointsOnArc; i++)
			{
				float angle = startAngle + i * angleStep;
				centroid += _centerPosition + (Vector2)(Quaternion.Euler(0, 0, angle) * Vector2.up * _radius);
			}
			centroid /= _pointsOnArc;

			_position.Clear();
			_up.Clear();
			//Calculate position
			for (int i = 0; i < _pointsOnArc; i++)
			{
				float angle = startAngle + i * angleStep;
				Vector2 cardPos = _centerPosition + (Vector2)(Quaternion.Euler(0, 0, angle+_angleOffset) * Vector2.up * _radius);

				Vector2 finalPos = cardPos + (_arcCenterPosition - centroid);
				_position.Add(finalPos);
				_up.Add((finalPos - _centerPosition).normalized);
			}
		}

		public void OnDrawGizmos()
		{
			if (_position.Count < _pointsOnArc)
				return;

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(_centerPosition, 0.1f);

			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(_arcCenterPosition, 0.1f);

			Gizmos.color = Color.green;
			for (int i = 0; i < _pointsOnArc; i++)
			{
				Gizmos.DrawLine(_position[i], _position[i] - (_up[i] / 2f));
			}
		}
	}
}