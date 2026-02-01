using UnityEngine;

namespace Astek
{
	public static class LayerMaskExtension
	{
		public static bool IsInLayer(this GameObject gameObject, LayerMask layerMask) => (layerMask.value & 1 << gameObject.layer) > 0;
	}
}