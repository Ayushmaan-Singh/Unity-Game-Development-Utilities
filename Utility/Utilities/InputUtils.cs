using AstekUtility.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.InputSystem;
namespace AstekUtility.Input
{
	public class InputUtils : IGameService
	{
		public Vector3 GetMousePosition { get; private set; }

		public InputUtils()
		{
			if (!ServiceLocator.Instance.CheckIfRegistered<InputUtils>())
				ServiceLocator.Instance.Register<InputUtils>(this);
		}

		~InputUtils()
		{
			if (ServiceLocator.Instance.CheckIfRegistered<InputUtils>())
				ServiceLocator.Instance.Unregister<InputUtils>();
		}

		public Vector3 MousePositionToXZPlane(Camera camera)
		{
			Plane plane = new Plane(Vector3.up, 0);
			Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (plane.Raycast(ray, out float distance))
			{
				GetMousePosition = ray.GetPoint(distance);
			}
			return GetMousePosition;
		}

		public Vector3 MousePositionToXYPlane(Camera camera)
		{
			GetMousePosition = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			return GetMousePosition;
		}
	}
}