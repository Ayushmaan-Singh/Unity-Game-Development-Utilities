using System;
using UnityEngine;
using UnityEngine.Events;

namespace Astek.SOAP.ScriptableEvents
{
	public class GameEventListener<T> : MonoBehaviour,IGameEventListener<T>
	{
		[SerializeField] private GameEvent<T> gameEvent;
		[SerializeField] private UnityEvent<T> response;

		private void OnEnable() => gameEvent.RegisterListener(this);
		private void OnDisable() => gameEvent.DeregisterListener(this);

		public void OnEventRaised(T data) => response.Invoke(data);
	}

	public class GameEventListener : GameEventListener<Unit> { }
}