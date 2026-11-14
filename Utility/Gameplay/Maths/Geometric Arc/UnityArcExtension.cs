using UnityEngine;

namespace Astek.Gameplay.ArcSystem.Core
{
    public static class ArcUnityExtensions
    {
        public static Vector2 Position(this in ArcPoint p) => new Vector2(p.X, p.Y);
        public static Quaternion Rotation(this in ArcPoint p) => Quaternion.Euler(0f, 0f, p.RotationDeg);

        public static ArcProperties WithCentre(this ArcProperties p, Vector2 centre)
        {
            p.CentreX = centre.x;
            p.CentreY = centre.y;
            return p;
        }
    }
}