#if UNITY_EDITOR
using System;
using Astek.Gameplay.ArcSystem;
using Astek.Gameplay.ArcSystem.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace Astek.Gameplay
{
    public partial class UnityArcHolder
    {
        [Title("Gizmo Preview")]
        [SerializeField, PropertyOrder(99)] private Vector2 cardSize = new Vector2(1.4f, 2f);
        [SerializeField, PropertyOrder(99)] private bool showCards = true;
        [SerializeField, PropertyOrder(99)] private bool showIndices = true;
        [SerializeField, PropertyOrder(99)] private bool showRadiusLines = true;
        [SerializeField, PropertyOrder(99), Min(0.01f)] private float pointRadius = 0.08f;
        [SerializeField, PropertyOrder(99), Range(8, 128)] private int arcSegments = 48;
        [SerializeField, PropertyOrder(99), Range(0, 10)] private int cardCount = 5;

        private void OnDrawGizmos()
        {
            ArcProperties props;
            if (Application.isPlaying) props = BuildProperties();
            else
            {
                props = spacingMode switch
                {
                    ArcSpacingMode.Null => new ArcProperties()
                    {
                        PointsOnArc = cardCount,
                        ArcAngle    = Mathf.Clamp(arcAngle, 0, maxArcAngle),
                        Radius      = radius,
                        AngleOffset = angleOffset
                    }.WithCentre(new Vector2(0f, -radius)),
                    ArcSpacingMode.Degrees => ArcProperties.FromSpacing(cardCount, radius, spacing, maxArcAngle, angleOffset)
                        .WithCentre(new Vector2(0f, -radius)),
                    ArcSpacingMode.ArcLength => ArcProperties.FromArcLength(cardCount, radius, spacing, maxArcAngle, angleOffset)
                        .WithCentre(new Vector2(0f, -radius)),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            _layout.Update(props);

            Matrix4x4 local = transform.localToWorldMatrix;
            Gizmos.matrix = local;

            // Centre of the circle
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(new Vector2(props.CentreX, props.CentreY), pointRadius * 1.5f);

            // Faint guide: the maximum allowed sweep
            DrawArc(props.CentreX + angleOffset - maxArcAngle * 0.5f, props.CentreY + angleOffset + maxArcAngle * 0.5f,
                new Color(1f, 1f, 1f, 0.15f), new Vector2(props.CentreX, props.CentreY));

            // Solid arc: the sweep actually in use
            if (props.ArcAngle > 0f)
                DrawArc(angleOffset - props.ArcAngle * 0.5f, angleOffset + props.ArcAngle * 0.5f,
                    new Color(0.2f, 0.9f, 1f, 1f), new Vector2(props.CentreX, props.CentreY));

            for (int i = 0; i < _layout.Count; i++)
            {
                ref readonly ArcPoint pt = ref _layout[i];
                Vector3 pos = new Vector3(pt.X, pt.Y, 0f);

                if (showRadiusLines)
                {
                    Gizmos.color = new Color(1f, 1f, 1f, 0.12f);
                    Gizmos.DrawLine(new Vector2(props.CentreX, props.CentreY), pos);
                }

                // Point
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(pos, pointRadius);

                // Card footprint, rotated exactly as the card would be
                if (showCards)
                {
                    Gizmos.matrix = local * Matrix4x4.TRS(pos, Quaternion.Euler(0f, 0f, pt.RotationDeg), Vector3.one);
                    Gizmos.color  = new Color(0.3f, 1f, 0.4f, 0.9f);
                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(cardSize.x, cardSize.y, 0.01f));
                    // "Up" direction of the card
                    Gizmos.DrawLine(Vector3.zero, new Vector3(0f, cardSize.y * 0.5f, 0f));
                    Gizmos.matrix = local;
                }

                if (spacingMode!=ArcSpacingMode.Null && showIndices)
                    Handles.Label(transform.TransformPoint(pos + Vector3.up * (pointRadius * 2f)),
                        $"{i}  ({pt.RotationDeg:0.#}°)");
            }

            Gizmos.matrix = Matrix4x4.identity;
        }
        private void DrawArc(float startDeg, float endDeg, Color color, Vector2 center = default)
        {
            Gizmos.color = color;
            Vector3 prev = PointOnCircle(startDeg).Add(x: center.x, y: center.y);
            for (int i = 1; i <= arcSegments; i++)
            {
                float a = Mathf.Lerp(startDeg, endDeg, i / (float)arcSegments);
                Vector3 next = PointOnCircle(a).Add(x: center.x, y: center.y);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
        private Vector3 PointOnCircle(float deg)
        {
            float rad = deg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
        }
    }
}
#endif