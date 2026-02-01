using UnityEngine;
namespace Astek
{
	public static class MathFunc
	{
		public static int ConvertLayerMaskToLayer(this LayerMask layerMask)
		{
			return (int)Mathf.Log(layerMask, 2);
		}
	}
}