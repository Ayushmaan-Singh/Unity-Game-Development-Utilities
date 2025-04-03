using Global;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class IsWithinCameraFOV : MonoBehaviour
	{
		[SerializeField] private CameraRuntimeSet cameraRTSet;
		[SerializeField] private CameraInGame cameraInGame;

		public bool IsInFov(Transform target)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraRTSet[cameraInGame.ToString()].GetComponent<Camera>());

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