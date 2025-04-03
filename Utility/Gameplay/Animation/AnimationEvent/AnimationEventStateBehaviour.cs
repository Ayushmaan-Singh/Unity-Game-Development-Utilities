using UnityEngine;
namespace AstekUtility.Gameplay
{
	public class AnimationEventStateBehaviour : StateMachineBehaviour
	{
		[field:SerializeField] public string EventName { get; private set; }
		[field:SerializeField, Range(0f, 1f)] public float TriggerTime { get; private set; }

		private int _executedInCycle = -1;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_executedInCycle = -1;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float currentTime = stateInfo.normalizedTime % 1f;

			if (currentTime >= TriggerTime && ((int)stateInfo.normalizedTime) > _executedInCycle)
			{
				NotifyReceiver(animator);
				_executedInCycle = (int)stateInfo.normalizedTime;
			}
		}

		private void NotifyReceiver(Animator animator)
		{
			AnimationEventReceiver receiver = animator.GetComponentInChildren<AnimationEventReceiver>() ?? animator.GetComponentInParent<AnimationEventReceiver>();

			if (receiver != null)
			{
				receiver.OnAnimationEventTriggered(EventName);
			}
		}
	}
}