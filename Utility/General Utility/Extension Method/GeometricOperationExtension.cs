namespace Astek
{
    public static class GeometricOperationExtension
    {
        public static float WrapAngle360(this float angle)
        {
            float result = angle % 360f;
            if (result < 0f) result += 360f;

            return result;
        }
        public static int WrapAngle360(this int angle) => ((angle % 360) + 360) % 360;
    }
}