using System;
using System.Collections.Generic;
using Astek.DesignPattern.ServiceLocatorTool;
using Astek.Gameplay.ImprovedTimer;
using Astek.DesignPattern.Blackboard;
using UnityEngine;
namespace Astek.Gameplay
{
	[RequireComponent(typeof(Animator))]
	public class EntityAnimator : MonoBehaviour
	{
		[SerializeField] private AnimationLayerInfoReceiver animStateInfoReceiver;
		[SerializeField] private CrossfadeAnimData defaultAnim;

		private CountdownTimer _animationTimer;
		private Animator _animator;
		private bool _isLocked;
		private CrossfadeAnimData _activeAnim;

		private void Awake()
		{
			ServiceLocator.For(this).Register(this);
			_animator = GetComponent<Animator>();
			_activeAnim = defaultAnim;
		}

		private void OnEnable()
		{
			_animator.enabled = true;
		}

		private void OnDisable()
		{
			_animator.enabled = false;
		}

		public void DefaultAnimationState()
		{
			_isLocked = false;
			CrossFade(defaultAnim);
		}

		public void CrossFade(CrossfadeAnimData animData, float normalizedTimeOffset = 0, float fixedTimeOffset = 0, bool isCritical = false)
		{
			if (_isLocked && !isCritical)
				return;

			_animationTimer = new CountdownTimer(animData.ClipLength)
			{
				OnTimerStart = () =>
				{
					_isLocked = animData.IsLocked;
					ChangeAnim(animData, normalizedTimeOffset, fixedTimeOffset);
				},

				OnTimerStop = () =>
				{
					if (_isLocked && !animData.IsLooping)
					{
						_isLocked = false;
						ChangeAnim(defaultAnim);
					}
				}
			};
			_animationTimer.Start();
		}

		private void ChangeAnim(CrossfadeAnimData animData, float normalizedTimeOffset = 0, float fixedTimeOffset = 0)
		{
			_activeAnim = animData;
			int layerIndex = animData.LayerIndex;

			animData.GetWeights.ForEach(keyValue => SetWeight(keyValue.Key, keyValue.Value));
			if (animData.UseNormalizedTime)
				_animator.CrossFade(animData.StateID, animData.NormalizedTimeDuration, layerIndex, normalizedTimeOffset);
			else
				_animator.CrossFadeInFixedTime(animData.StateID, animData.FixedTimeDuration, layerIndex, fixedTimeOffset);
		}

		//================================= Animator Functions =================================//

		public void SetWeight(int index, float amount) => _animator.SetLayerWeight(index, amount);

		public void SetFloat(string varName, float val) => _animator.SetFloat(varName, val);
		public void SetFloat(int varID, float val) => _animator.SetFloat(varID, val);

		public float GetFloat(string varName) => _animator.GetFloat(varName);
		public float GetFloat(int varID) => _animator.GetFloat(varID);

		public void SetInt(string varName, int val) => _animator.SetInteger(varName, val);
		public void SetInt(int varID, int val) => _animator.SetInteger(varID, val);

		public int GetInt(string varName) => _animator.GetInteger(varName);
		public int GetInt(int varID) => _animator.GetInteger(varID);

		public void SetTrigger(string varName) => _animator.SetTrigger(varName);
		public void SetTrigger(int varID) => _animator.SetTrigger(varID);

		public void SetBool(string varName, bool val) => _animator.SetBool(varName, val);
		public void SetBool(int varID, bool val) => _animator.SetBool(varID, val);

		public bool GetBool(string varName) => _animator.GetBool(varName);
		public bool GetBool(int varID) => _animator.GetBool(varID);
	}
}