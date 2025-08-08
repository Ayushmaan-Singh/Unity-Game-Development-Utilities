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
		[SerializeField, OnValueChanged("CalculatePosAndTransformUp")] private int pointsOnArc = 5;

		//Arc Properties
		[SerializeField, PropertyTooltip("Center Point Of Arc"), OnValueChanged("CalculatePosAndTransformUp")] private Vector2 centerPosition;
		[SerializeField, PropertyTooltip("Desired Center Point Of Arc"), OnValueChanged("CalculatePosAndTransformUp")] private Vector2 arcCenterPosition;
		[SerializeField, Range(0, 360), OnValueChanged("CalculatePosAndTransformUp")] private float arcAngle;
		[SerializeField, OnValueChanged("CalculatePosAndTransformUp")] private float radius;

		public int PointsOnArc { get => pointsOnArc; set => pointsOnArc = value; }
		public Vector2 CenterPosition { get => centerPosition; set => centerPosition = value; }
		public Vector2 ArcCenterPosition { get => arcCenterPosition; set => arcCenterPosition = value; }
		public float ArcAngle { get => arcAngle; set => arcAngle = value; }
		public float Radius { get => radius; set => radius = value; }


		[ShowInInspector, ReadOnly] public readonly List<Vector2> Position = new List<Vector2>();
		[ShowInInspector, ReadOnly] public readonly List<Vector2> Up = new List<Vector2>();

		public void CalculatePosAndTransformUp()
		{
			float startAngle = arcAngle > 0 ? -arcAngle / 2 : 0;
			float angleStep = pointsOnArc > 1 ? arcAngle / (pointsOnArc - 1) : 0;

			Vector2 centroid = Vector2.zero;

			//Calculate Centroid
			for (int i = 0; i < pointsOnArc; i++)
			{
				float angle = startAngle + i * angleStep;
				centroid += centerPosition + (Vector2)(Quaternion.Euler(0, 0, angle) * Vector2.up * radius);
			}
			centroid /= pointsOnArc;

			Position.Clear();
			Up.Clear();
			//Calculate position
			for (int i = 0; i < pointsOnArc; i++)
			{
				float angle = startAngle + i * angleStep;
				Vector2 cardPos = centerPosition + (Vector2)(Quaternion.AngleAxis(angle, -Vector3.forward) * Vector3.up * radius);

				Vector2 finalPos = cardPos + (arcCenterPosition - centroid);
				Position.Add(finalPos);
				Up.Add((finalPos - centerPosition).normalized);
			}
		}

		#if UNITY_EDITOR
		public void OnDrawGizmos()
		{
			if (Position.Count < pointsOnArc)
				return;

			Gizmos.color = Color.red;
			Gizmos.DrawSphere(centerPosition, 0.1f);

			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(arcCenterPosition, 0.1f);

			Gizmos.color = Color.green;
			for (int i = 0; i < pointsOnArc; i++)
			{
				Gizmos.DrawLine(Position[i], Position[i] - (Up[i] / 2f));
			}
		}
		#endif
	}
}