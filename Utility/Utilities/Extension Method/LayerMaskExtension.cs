using UnityEngine;

namespace AstekUtility
{
	public static class LayerMaskExtension
	{
		public static bool IsInLayer(this GameObject gameObject, LayerMask layerMask) => (layerMask.value & 1 << gameObject.layer) > 0;
	}
}