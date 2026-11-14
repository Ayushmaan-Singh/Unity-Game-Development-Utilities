using System;

namespace Astek.Gameplay.ArcSystem
{
    public static class Arc2D
    {
        private const float _deg2Rad = MathF.PI / 180f;

        /// <summary>
        /// Fills <paramref name="results"/> with up to PointsOnArc points.
        /// Returns the number of points written. Zero allocations.
        /// </summary>
        public static int Calculate(in ArcProperties p, Span<ArcPoint> results)
        {
            int count = Math.Min(p.PointsOnArc, results.Length);
            if (count <= 0) return 0;

            // Single point sits at the arc's centre angle.
            float stepDeg = count > 1 ? p.ArcAngle / (count - 1) : 0f;
            float startDeg = p.AngleOffset - (count > 1 ? p.ArcAngle * 0.5f : 0f);

            float startRad = startDeg * _deg2Rad;
            float c = MathF.Cos(startRad);
            float s = MathF.Sin(startRad);

            float stepRad = stepDeg * _deg2Rad;
            float cStep = MathF.Cos(stepRad);
            float sStep = MathF.Sin(stepRad);

            float angleDeg = startDeg;

            for (int i = 0; i < count; i++)
            {
                results[i] = new ArcPoint
                {
                    X           = p.CentreX + p.Radius * c,
                    Y           = p.CentreY + p.Radius * s,
                    RotationDeg = angleDeg - 90f
                };

                // Rotate (c, s) by one step.
                // theta is angleDeg and delta is stepdeg
                // we have precalculated cos(delta) and sin(delta) as cStep and sStep and cos and sin theta is simpy the previous c and s value
                // cos(theta+delta)= cos(theta)cos(delta)-sin(theta)sin(delta)
                // sin(theta+delta)= sin(theta)cos(delta)+cos(theta)sin(delta)
                float nc = c * cStep - s * sStep;
                s        =  s * cStep + c * sStep;
                c        =  nc;
                angleDeg += stepDeg;
            }

            return count;
        }
    }

    /// <summary>
    /// Reusable, cached arc layout. Buffer only grows; recalculates only when properties change.
    /// </summary>
    public sealed class ArcLayout
    {
        private ArcPoint[] _buffer;
        private ArcProperties _last;
        private bool _hasLast;

        public int Count { get; private set; }

        public ReadOnlySpan<ArcPoint> Points => new ReadOnlySpan<ArcPoint>(_buffer, 0, Count);

        public ref readonly ArcPoint this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Count) throw new IndexOutOfRangeException();
                return ref _buffer[index];
            }
        }

        public ArcLayout(int initialCapacity = 10)
        {
            _buffer = new ArcPoint[Math.Max(1, initialCapacity)];
        }

        /// <returns>true if the layout was recalculated.</returns>
        public bool Update(in ArcProperties p)
        {
            if (_hasLast && p.Equals(_last)) return false;

            int needed = Math.Max(p.PointsOnArc, 0);
            if (_buffer.Length < needed)
                Array.Resize(ref _buffer, Math.Max(needed, _buffer.Length * 2));

            Count    = Arc2D.Calculate(p, _buffer.AsSpan(0, needed));
            _last    = p;
            _hasLast = true;
            return true;
        }
    }

    [Serializable]
    public struct ArcProperties
    {
        public int PointsOnArc;
        public float CentreX;
        public float CentreY;
        public float ArcAngle; // degrees, total sweep
        public float Radius;
        public float AngleOffset; // degrees, 90 = fan opens upward

        private static float _tolerance = 0.0001f;

        public bool Equals(ArcProperties o) =>
            PointsOnArc == o.PointsOnArc
            && Math.Abs(CentreX - o.CentreX) < _tolerance
            && Math.Abs(CentreY - o.CentreY) < _tolerance
            && Math.Abs(ArcAngle - o.ArcAngle) < _tolerance
            && Math.Abs(Radius - o.Radius) < _tolerance
            && Math.Abs(AngleOffset - o.AngleOffset) < _tolerance;

        public override bool Equals(object obj) => obj is ArcProperties o && Equals(o);

        public override int GetHashCode() =>
            HashCode.Combine(PointsOnArc, CentreX, CentreY, ArcAngle, Radius, AngleOffset);

        public static ArcProperties FromSpacing(
            int count, float radius, float spacingDeg, float maxArcAngle,
            float angleOffset = 90f, float centreX = 0f, float centreY = 0f)
        {
            float arc = count > 1
                ? MathF.Min(maxArcAngle, (count - 1) * spacingDeg)
                : 0f;

            return new ArcProperties
            {
                PointsOnArc = count,
                CentreX     = centreX,
                CentreY     = centreY,
                ArcAngle    = arc,
                Radius      = radius,
                AngleOffset = angleOffset
            };
        }

        /// <summary>
        /// Same idea, but spacing is a distance along the arc (e.g. card width * overlap),
        /// so the look stays consistent if you change the radius.
        /// </summary>
        public static ArcProperties FromArcLength(
            int count, float radius, float cardSpacing, float maxArcAngle,
            float angleOffset = 90f, float centreX = 0f, float centreY = 0f)
        {
            // arc length s = r * theta  ->  theta = s / r
            float spacingDeg = radius > 0f ? cardSpacing / radius * (180f / MathF.PI) : 0f;
            return FromSpacing(count, radius, spacingDeg, maxArcAngle, angleOffset, centreX, centreY);
        }
    }

    [Serializable]
    public struct ArcPoint
    {
        public float X;
        public float Y;
        public float RotationDeg;
    }
}