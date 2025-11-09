using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace AstekUtility.Gameplay
{
	[RequireComponent(typeof(GraphicRaycaster))]
	public class UIRaycaster : MonoBehaviour
	{
		private GraphicRaycaster _raycaster;
		private EventSystem _eventSystem;

		private void Awake()
		{
			_raycaster = GetComponent<GraphicRaycaster>();
			_eventSystem = EventSystem.current;
			ServiceLocator.For(this).Register(this);
		}

		public List<RaycastResult> Raycast(Vector2 pos)
		{
			PointerEventData pointerEventData = new PointerEventData(_eventSystem)
			{
				position = pos
			};

			// Create a list to store raycast results
			List<RaycastResult> results = new List<RaycastResult>();

			// Perform the raycast
			_raycaster.Raycast(pointerEventData, results);
			return results;
		}
	}
}