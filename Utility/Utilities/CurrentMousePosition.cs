using System;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using Global;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace AstekUtility.Input
{
	public class CurrentMousePosition : Singleton<CurrentMousePosition>
	{
		private Vector3 _currentMousePosWorld;
		private Vector3 _currentMousePosUI;
		
		public Vector3 GetMousePositionWorld => _currentMousePosWorld;
		public Vector3 GetMousePositionUI => _currentMousePosUI;

		public void Update()
		{
			MousePositionUI(GameManager.Instance.ActiveCamera);
		}
		
		public void FixedUpdate()
		{
			MousePositionToXZPlane(GameManager.Instance.ActiveCamera);
		}

		private Vector3 GetCurrentMousePosition()
		{
			return _currentMousePosWorld;
		}

		private void MousePositionToXZPlane(Camera camera)
		{
			Plane plane = new Plane(Vector3.up, 0);
			Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (plane.Raycast(ray, out float distance))
			{
				_currentMousePosWorld = ray.GetPoint(distance);
			}
		}

		/// <summary>
		/// Not used in current game
		/// </summary>
		/// <param name="camera"></param>
		private void MousePositionUI(Camera camera)
		{
			_currentMousePosUI = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		}
	}
}