using System.Collections.Generic;
using UnityEngine;
namespace AstekUtility.SOAP.ScriptableEvents
{
	public class GameEvent<T> : ScriptableObject
	{
		private readonly List<IGameEventListener<T>> _listeners = new List<IGameEventListener<T>>();

		/// <summary>
		///we start raising in reverse order of registering so that if one of the reaction to raising the event is
		///the object destroying itself or unregistering, it doesn't break operation flow and cause errors
		/// </summary>
		/// <param name="data"></param>
		public void Raise(T data)
		{
			for (int i = _listeners.Count - 1; i >= 0; i--)
			{
				_listeners[i].OnEventRaised(data);
			}
		}

		public void RegisterListener(IGameEventListener<T> listener) => _listeners.Add(listener);
		public void DeregisterListener(IGameEventListener<T> listener) => _listeners.Remove(listener);
	}

	public class GameEvent : GameEvent<Unit>
	{
		public void Raise() => Raise(Unit.Default);
	}
	
	/// <summary>
	/// Null object for defining null object game events
	/// </summary>
	public struct Unit
	{
		public static Unit Default => default;
	}
}