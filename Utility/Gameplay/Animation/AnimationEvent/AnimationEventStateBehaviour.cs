using System;
using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class AnimationEventStateBehaviour : StateMachineBehaviour
	{
		[field:SerializeField] public string EventName { get; private set; }
		[field:SerializeField, Range(0f, 1f)] public float TriggerTime { get; private set; }
		[SerializeField] private bool _runOnce = false;

		private bool _isExecuted = false;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_isExecuted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float time = _runOnce ? stateInfo.normalizedTime : stateInfo.normalizedTime - ((int)stateInfo.normalizedTime);
			if (time >= TriggerTime)
				NotifyReceiver(animator);
		}

		private void NotifyReceiver(Animator animator)
		{
			AnimationEventReceiver receiver = animator.GetComponentInChildren<AnimationEventReceiver>() ?? animator.GetComponentInParent<AnimationEventReceiver>();

			if (receiver == null)
				throw new NullReferenceException("No receiver found for this event execution");

			receiver.OnAnimationEventTriggered(EventName);
			_isExecuted = true;
		}
	}
}