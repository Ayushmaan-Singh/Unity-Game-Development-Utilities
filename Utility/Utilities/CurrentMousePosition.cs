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
		private Vector2 _currentMousePosScreen;
		private Ray _rayToMousePosWorld;

		public Vector3 GetMousePositionWorld => _currentMousePosWorld;
		public Vector2 GetMousePositionScreen => _currentMousePosScreen;
		public Ray GetRayAtMousePosWorld => _rayToMousePosWorld;

		public void Update()
		{
			MousePositionUpdate(GameMasterManager.Instance.ActiveCamera);
		}

		/// <summary>
		/// Not used in current game
		/// </summary>
		/// <param name="cam"></param>
		private void MousePositionUpdate(Camera cam)
		{
			_currentMousePosScreen = Mouse.current.position.value;
			_currentMousePosWorld = cam.ScreenToWorldPoint(_currentMousePosScreen);
			_rayToMousePosWorld = cam.ScreenPointToRay(_currentMousePosScreen);
		}
	}
}