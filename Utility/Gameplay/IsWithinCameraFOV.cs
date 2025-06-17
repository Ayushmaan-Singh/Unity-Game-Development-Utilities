using Global;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class IsWithinCameraFOV : MonoBehaviour
	{
		[SerializeField] private Camera_SV cameraSV;

		public bool IsInFov(Transform target)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraSV.GetCamera(gameObject.scene,CameraTag.Main).GetComponent<Camera>());

			foreach (Plane plane in planes)
			{
				if (plane.GetDistanceToPoint(target.position) < 0)
				{
					return false;
				}
			}

			return true;
		}
	}
}