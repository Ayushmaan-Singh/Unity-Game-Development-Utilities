using UnityEngine;

namespace Astek
{
    public static class ColorMethodExtension
    {
        public static Color With(this Color color, int? r = null, int? g = null, int? b = null, int? a = null) =>
            new(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
    }
}