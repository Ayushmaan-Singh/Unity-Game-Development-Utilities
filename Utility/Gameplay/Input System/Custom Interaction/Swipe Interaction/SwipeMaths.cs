using UnityEngine;

namespace Astek.InputSystem.Swipe.Core
{
    public enum SwipeDirection
    {
        E = 0,
        NE = 1,
        N = 2,
        NW = 3,
        W = 4,
        SW = 5,
        S = 6,
        SE = 7,
        Null
    }
    
    internal static class SwipeMaths
    {
        /// <summary>
        /// Quantizes a vector to one of 8 directions with adjustable sector sizes.
        /// diagonalWidth = angle given to each diagonal sector:
        ///   0  -> 4-way (diagonals never returned)
        ///   30 -> cardinals 60 deg wide, diagonals 30 deg wide (recommended for swipes)
        ///   45 -> classic equal 45 deg sectors
        ///   90 -> 4-way (cardinals never returned)
        /// </summary>
        public static SwipeDirection EvaluateDirection(this Vector2 direction, float diagonalWidth = 0)
        {
            if (direction == Vector2.zero) return SwipeDirection.Null;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0f) angle    += 360f;
            if (angle >= 360f) angle -= 360f; // float rounding guard

            float cardinalHalf = (90f - Mathf.Clamp(diagonalWidth, 0f, 90f)) * 0.5f;

            int quadrant = (int)(angle / 90f) % 4; // 0: E->N, 1: N->W, 2: W->S, 3: S->E
            float within = angle - quadrant * 90f; // [0, 90)

            int index;
            if (within < cardinalHalf)
                index = quadrant * 2; // cardinal at the start of the quadrant
            else if (within >= 90f - cardinalHalf)
                index = (quadrant * 2 + 2) % 8; // cardinal at the end of the quadrant
            else
                index = quadrant * 2 + 1; // diagonal in between

            return (SwipeDirection)index;
        }
    }
}