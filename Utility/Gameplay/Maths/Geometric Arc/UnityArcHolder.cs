using System;
using System.Collections.Generic;
using Astek.Gameplay.ArcSystem;
using Astek.Gameplay.ArcSystem.Core;
using UnityEngine;

namespace Astek.Gameplay
{
    [DisallowMultipleComponent]
    public partial class UnityArcHolder : MonoBehaviour
    {
        protected enum ArcSpacingMode
        {
            Null,
            Degrees,
            ArcLength
        }

        [Header("Arc Properties")]
        [SerializeField] protected float radius = 8f;
        [SerializeField] protected float arcAngle = 40f;
        [SerializeField, Range(0f, 360f)] public float maxArcAngle = 50f;
        [SerializeField] protected float angleOffset = 90f;
        [SerializeField] protected ArcSpacingMode spacingMode = ArcSpacingMode.Degrees;
        [SerializeField, Min(0f)] public float spacing = 6f; // degrees or world units, per mode

        [Header("Arc Layout")]
        [SerializeField, Min(1)] protected int initialCapacity = 10;

        protected List<Transform> _cards;
        protected ArcLayout _layout = new(10);

        private void Awake()
        {
            _cards  = new(initialCapacity);
            _layout = new(initialCapacity);
        }

        public void AddCard(Transform card)
        {
            card.SetParent(transform, false);
            _cards.Add(card);
            Relayout();
        }
        public void RemoveCard(Transform card)
        {
            _cards.Remove(card);
            Relayout();
        }

        public ArcProperties BuildProperties() =>
            spacingMode switch
            {
                ArcSpacingMode.Null => new ArcProperties()
                {
                    PointsOnArc = _layout.Count,
                    ArcAngle    = Mathf.Clamp(arcAngle, 0, maxArcAngle),
                    Radius      = radius,
                    AngleOffset = angleOffset
                }.WithCentre(new Vector2(0f, -radius)),
                ArcSpacingMode.Degrees => ArcProperties.FromSpacing(_layout.Count, radius, spacing, maxArcAngle, angleOffset)
                    .WithCentre(new Vector2(0f, -radius)),
                ArcSpacingMode.ArcLength => ArcProperties.FromArcLength(_layout.Count, radius, spacing, maxArcAngle, angleOffset)
                    .WithCentre(new Vector2(0f, -radius)),
                _ => throw new ArgumentOutOfRangeException()
            };
        
        private void Relayout()
        {
            ArcProperties props = BuildProperties();
            _layout.Update(props); // always apply below, even if cached

            for (int i = 0; i < _layout.Count; i++)
            {
                ref readonly ArcPoint pt = ref _layout[i];
                _cards[i].localPosition = pt.Position();
                _cards[i].localRotation = pt.Rotation();
            }
        }
    }
}