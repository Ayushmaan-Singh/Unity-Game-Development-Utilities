using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Astek.Gameplay.Geometric_Calculations
{
    [Serializable]
    public class Arc2D
    {
        [SerializeField, OnValueChanged("Calculate")] private int pointsOnArc = 5;

        [Header("Arc Properties")]
        [PropertyTooltip("Center Point Of Arc"), OnValueChanged("Calculate")]
        [SerializeField] private Vector2 centre;
        [Range(0, 360), OnValueChanged("Calculate")]
        [SerializeField] private float arcAngle;
        [OnValueChanged("Calculate")]
        [SerializeField] private float radius;
        [Range(0, 360), OnValueChanged("Calculate")]
        [SerializeField] private float angleOffset;

        public int PointsOnArc { get => pointsOnArc; set => pointsOnArc = value; }
        public Vector2 Centre { get => centre; set => centre = value; }
        public float Radius { get => radius; set => radius = value; }
        public float ArcAngle { get => arcAngle; set => arcAngle = value; }

        [ShowInInspector, ReadOnly] public ArcPointTransform[] ArcPointsTransform { get; private set; }
        
        public Arc2D Calculate()
        {
            ArcPointsTransform = GetPositionPointsOnArc(pointsOnArc, centre, radius, -arcAngle / 2f, arcAngle / 2f);
            return this;
        }

        private ArcPointTransform[] GetPositionPointsOnArc(int points, Vector2 center, float radius, float startAngleDeg, float endAngleDeg)
        {
            ArcPointTransform[] transforms = new ArcPointTransform[points];
            float startRad = startAngleDeg * Mathf.Deg2Rad;
            float endRad = endAngleDeg * Mathf.Deg2Rad;
            float rotationOffsetRad = angleOffset * Mathf.Deg2Rad;

            for (int i = 0; i < points; i++)
            {
                float t = (points == 1) ? 0.5f : (float)i / (points - 1);
                float angle = Mathf.Lerp(startRad, endRad, t);

                // Base arc position
                float x = center.x + radius * Mathf.Cos(angle);
                float y = center.y + radius * Mathf.Sin(angle);

                // Apply rotation offset to (x,y) around center
                float dx = x - center.x;
                float dy = y - center.y;

                float rotatedX = center.x + dx * Mathf.Cos(rotationOffsetRad) - dy * Mathf.Sin(rotationOffsetRad);
                float rotatedY = center.y + dx * Mathf.Sin(rotationOffsetRad) + dy * Mathf.Cos(rotationOffsetRad);

                // Rotation: tangent to arc, also rotated
                float rotationDeg = angle * Mathf.Rad2Deg - 90f + angleOffset;

                transforms[i] = new ArcPointTransform
                {
                    Position = new Vector2(rotatedX, rotatedY),
                    RotationDeg = rotationDeg
                };
            }

            return transforms;

        }

        [Serializable]
        public struct ArcPointTransform
        {
            public Vector2 Position;
            public float RotationDeg;
        }


		#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (ArcPointsTransform == null || ArcPointsTransform.Length < pointsOnArc)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(centre, 0.05f);

            Gizmos.color = Color.green;
            for (int i = 0; i < pointsOnArc; i++)
            {
                //Gizmos.DrawLine(Position[i], Position[i] - (Up[i] / 2f));
                Gizmos.DrawWireSphere(ArcPointsTransform[i].Position, 0.05f);
            }
        }
		#endif
    }
}