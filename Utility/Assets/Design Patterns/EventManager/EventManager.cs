using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astek.DesignPattern.EventSystem
{
	/// <summary>
	///     Manages all types of Game Events.
	///     All the communications should be from this class across the layers
	/// </summary>
	public static class EventManager
	{
		public delegate void EventDelegate<in T>(T e) where T : GameEvent;
		private delegate void EventDelegate(GameEvent e);

		private static Dictionary<Delegate, EventDelegate> _delegateLookup;
		private static Dictionary<Type, EventDelegate> _delegates;
		private static Dictionary<Delegate, bool> _onceLookups;

		/// <summary>
		/// Queue of events that has to run 
		/// </summary>
		private static Queue _eventsInQueue;

		/// <summary>
		/// Every update cycle the queue is processed, if the queue processing is limited,
		/// a maximum processing time per update can be set after which the events will have
		/// to be processed next update loop.
		/// </summary>
		public static void Update()
		{
			if (_eventsInQueue.Count > 0)
			{
				//Handling one event per frame
				GameEvent evt = _eventsInQueue.Dequeue() as GameEvent;
				TriggerEvent(evt);
			}
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void ResetStatic()
		{
			_delegateLookup = new Dictionary<Delegate, EventDelegate>();
			_delegates = new Dictionary<Type, EventDelegate>();
			_onceLookups = new Dictionary<Delegate, bool>();

			_eventsInQueue = new Queue();
		}
		
		/// <summary>
		/// Queues events to be run synchronously
		/// </summary>
		/// <param name="gEvent"></param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		public static bool QueueEvent(GameEvent gEvent)
		{
			if (!_delegates.ContainsKey(gEvent.GetType()))
			{
				Debug.LogWarning($"EventManager: QueueEvent failed due to no listeners for event: {gEvent.GetType()}");
				throw new NullReferenceException($"EventManager: QueueEvent failed due to no listeners for event: {gEvent.GetType()}");
			}

			_eventsInQueue.Enqueue(gEvent);
			return true;
		}
		
		/// <summary>
		/// Registering Listener
		/// </summary>
		/// <param name="eventDel"></param>
		/// <typeparam name="T"></typeparam>
		public static void AddListener<T>(EventDelegate<T> eventDel) where T : GameEvent => AddDelegate(eventDel);
		public static void AddListenerOnce<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			EventDelegate result = AddDelegate(eventDel);

			if (result != null)
			{
				//Only called once
				_onceLookups[result] = true;
			}
		}
		/// <summary>
		/// Unregister Listener
		/// </summary>
		/// <param name="eventDel"></param>
		/// <typeparam name="T"></typeparam>
		public static void RemoveListener<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			if (!_delegateLookup.TryGetValue(eventDel, out EventDelegate internalDelegate))
				return;
			if (_delegates.TryGetValue(typeof(T), out EventDelegate tempDelegate))
			{
				tempDelegate -= internalDelegate;
				if (tempDelegate == null)
				{
					_delegates.Remove(typeof(T));
				}
				else
				{
					_delegates[typeof(T)] = tempDelegate;
				}
			}
			_delegateLookup.Remove(eventDel);
		}

		/// <summary>
		/// Clears all events and queues
		/// </summary>
		public static void Release()
		{
			RemoveAll();
			_eventsInQueue.Clear();
		}
		public static bool HasListeners<T>(EventDelegate<T> eventDel) where T : GameEvent => _delegateLookup.ContainsKey(eventDel);

		public static void TriggerEvent(GameEvent gEvent)
		{
			EventDelegate eventDel;
			if (_delegates.TryGetValue(gEvent.GetType(), out eventDel))
			{
				eventDel.Invoke(gEvent);

				//Remove Listeners which should only be called once
				foreach (Delegate @delegate in _delegates[gEvent.GetType()].GetInvocationList())
				{
					EventDelegate eventD = (EventDelegate)@delegate;
					if (_onceLookups.ContainsKey(eventD))
					{
						_onceLookups.Remove(eventD);
					}
				}
			}
			else
			{
				Debug.LogError($"Event {gEvent.GetType()} has no Listener");
				throw new NullReferenceException($"Event {gEvent.GetType()} has no Listener");
			}
		}

		#region Add Remove Delegate Internal

		private static EventDelegate AddDelegate<T>(EventDelegate<T> eventDel) where T : GameEvent
		{
			if (_delegateLookup.ContainsKey(eventDel))
				return null;

			//This is the one that gets invoked
			EventDelegate internalDelegate = e => eventDel((T)e);
			_delegateLookup[eventDel] = internalDelegate;

			EventDelegate tempDel = null;
			if (_delegates.TryGetValue(typeof(T), out tempDel))
			{
				_delegates[typeof(T)] = tempDel += internalDelegate;
			}
			else
			{
				_delegates[typeof(T)] = internalDelegate;
			}

			return internalDelegate;
		}
		private static void RemoveAll()
		{
			_delegates.Clear();
			_delegateLookup.Clear();
			_onceLookups.Clear();
		}

		#endregion
	}
}