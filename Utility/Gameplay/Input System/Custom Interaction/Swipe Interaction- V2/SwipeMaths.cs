using UnityEngine;

namespace Astek.InputSystem
{
    internal static class SwipeMaths
    {
        private static Vector2Int[] _directions = new[]
        {
            new Vector2Int(1, 0), // 0: East
            new Vector2Int(1, 1), // 1: North-East
            new Vector2Int(0, 1), // 2: North
            new Vector2Int(-1, 1), // 3: North-West
            new Vector2Int(-1, 0), // 4: West
            new Vector2Int(-1, -1), // 5: South-West
            new Vector2Int(0, -1), // 6: South
            new Vector2Int(1, -1) // 7: South-East
        };

        public static Vector2Int EvaluateDir8(this Vector2 direction)
        {
            if (direction == Vector2.zero)
                return Vector2Int.zero;

            // Calculate angle in degrees [-180, 180]
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Shift negative angles into [0, 360) range
            if (angle < 0)
                angle += 360f;

            // Quantize into 8 sectors of 45 degrees each
            int step = Mathf.RoundToInt(angle / 45f) % 8;

            return _directions[step];
        }
    }
}