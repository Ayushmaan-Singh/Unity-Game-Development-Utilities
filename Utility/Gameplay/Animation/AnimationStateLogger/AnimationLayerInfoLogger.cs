using System;
using UnityEngine;
namespace Astek.Gameplay
{
	public class AnimationLayerInfoLogger : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			AnimationLayerInfoReceiver infoReceiver =
				animator.GetComponentInChildren<AnimationLayerInfoReceiver>() ?? animator.GetComponentInParent<AnimationLayerInfoReceiver>();
			infoReceiver?.OnStateEnter(stateInfo, layerIndex);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			AnimationLayerInfoReceiver infoReceiver =
				animator.GetComponentInChildren<AnimationLayerInfoReceiver>() ?? animator.GetComponentInParent<AnimationLayerInfoReceiver>();
			infoReceiver?.OnStateUpdate(stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			AnimationLayerInfoReceiver infoReceiver =
				animator.GetComponentInChildren<AnimationLayerInfoReceiver>() ?? animator.GetComponentInParent<AnimationLayerInfoReceiver>();
			infoReceiver?.OnStateExit(stateInfo, layerIndex);
		}
	}
}