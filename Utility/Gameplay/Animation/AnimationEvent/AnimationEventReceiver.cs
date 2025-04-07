using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility.Gameplay
{
	public class AnimationEventReceiver : MonoBehaviour
	{
		[SerializeField] private List<AnimationEvent> animationEvents = new List<AnimationEvent>();

		public void OnAnimationEventTriggered(string eventName)
		{
			List<AnimationEvent> animEvent = animationEvents.Where(animationEvent => animationEvent.EventName == eventName).ToList();
			if (animEvent == null || !animEvent.Any())
				throw new NullReferenceException($"No animation event by \"{eventName}\" is registered");
			
			animEvent?.ForEach(execEvent => execEvent.TriggerEvent());
		}

		public void RegisterAnimationEvent(string eventName, UnityAction action)
		{
			AnimationEvent animEvent = animationEvents.Find(animationEvent => animationEvent.EventName == eventName);

			//If null create animation event
			if (animEvent == null)
			{
				animEvent = new AnimationEvent.Builder().InitEventName(eventName).Build(new AnimationEvent());
				animationEvents.Add(animEvent);
			}
			animEvent.AddListener(action);
		}

		public void DeregisterAnimationEvent(string eventName, UnityAction action)
		{
			AnimationEvent animEvent = animationEvents.Find(animationEvent => animationEvent.EventName == eventName);
			animEvent.RemoveListener(action);
			if (animEvent.RegisteredEventCount < 1)
				animationEvents.Remove(animEvent);
		}
	}

	[Serializable]
	public class AnimationEvent
	{
		[SerializeField] private string eventName;
		[SerializeField] private UnityEvent onAnimationEvent = new UnityEvent();

		public int RegisteredEventCount => onAnimationEvent.GetPersistentEventCount();

		public string EventName => eventName;
		public void AddListener(UnityAction action) => onAnimationEvent.AddListener(action);
		public void RemoveListener(UnityAction action) => onAnimationEvent.RemoveListener(action);
		public void TriggerEvent() => onAnimationEvent?.Invoke();

		public class Builder
		{
			private string _eventName;
			private UnityEvent events;

			public Builder InitEventName(string eventName)
			{
				_eventName = eventName;
				return this;
			}

			public AnimationEvent Build(AnimationEvent instance)
			{
				instance.eventName = _eventName;
				return instance;
			}
		}
	}
}