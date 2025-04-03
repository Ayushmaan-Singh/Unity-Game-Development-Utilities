using System;
using System.Collections.Generic;
using AstekUtility.DesignPattern.ServiceLocatorTool;
using UnityEngine;
using UnityEngine.Events;
namespace AstekUtility.Gameplay
{
	public class AnimationLayerInfoReceiver : MonoBehaviour
	{
		private readonly Dictionary<int, AnimatorStateInfo> _activeStateInfo = new Dictionary<int, AnimatorStateInfo>();

		[field:SerializeField] public UnityEvent<AnimatorStateInfo, int> OnStateEnterEvent { get; private set; }
		[field:SerializeField] public UnityEvent<AnimatorStateInfo, int> OnStateUpdateEvent { get; private set; }
		[field:SerializeField] public UnityEvent<AnimatorStateInfo, int> OnStateExitEvent { get; private set; }

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
		}

		public void OnStateEnter(AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!_activeStateInfo.TryAdd(layerIndex, stateInfo))
				_activeStateInfo[layerIndex] = stateInfo;

			OnStateEnterEvent?.Invoke(stateInfo, layerIndex);
		}
		public void OnStateUpdate(AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!_activeStateInfo.TryAdd(layerIndex, stateInfo))
				_activeStateInfo[layerIndex] = stateInfo;

			OnStateUpdateEvent?.Invoke(stateInfo, layerIndex);
		}
		public void OnStateExit(AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_activeStateInfo[layerIndex].shortNameHash == stateInfo.shortNameHash)
				_activeStateInfo.Remove(layerIndex);

			OnStateExitEvent?.Invoke(stateInfo, layerIndex);
		}

		public bool InProgress(string stateName, int layerIndex) => _activeStateInfo.TryGetValue(layerIndex, out AnimatorStateInfo stateInfo) && stateInfo.IsName(stateName)
		                                                                                                                                      && !(stateInfo.normalizedTime >= 1f || stateInfo.normalizedTime.Approx(1f));
		public bool InProgress(int stateHash, int layerIndex) => _activeStateInfo.TryGetValue(layerIndex, out AnimatorStateInfo stateInfo) && stateInfo.shortNameHash == stateHash
		                                                                                                                                   && !(stateInfo.normalizedTime >= 1f || stateInfo.normalizedTime.Approx(1f));

		public bool IsLooping(int layerIndex) => _activeStateInfo.TryGetValue(layerIndex, out AnimatorStateInfo stateInfo) && stateInfo.loop;
	}
}