using System;
using AstekUtility.Gameplay.ImprovedTimer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AstekUtility
{
	[System.Serializable]
	public class DoubleTapInteraction : IInputInteraction<float>
	{
		public float TimeBetweenDoubleTaps = 0.4f;
		/// <summary>
		/// The time in seconds within which the control needs to be pressed and released to perform the interaction.
		/// </summary>
		/// <remarks>
		/// If this value is equal to or smaller than zero, the input system will use (<see cref="InputSettings.defaultTapTime"/>) instead.
		/// </remarks>
		[Tooltip("The maximum time (in seconds) allowed to elapse between pressing and releasing a control for it to register as a tap.")]
		public float TapTime;
		/// <summary>
		/// The time in seconds which is allowed to pass between taps.
		/// </summary>
		/// <remarks>
		/// If this time is exceeded, the multi-tap interaction is canceled.
		/// If this value is equal to or smaller than zero, the input system will use the duplicate value of <see cref="TapTime"/> instead.
		/// </remarks>
		[Tooltip("The maximum delay (in seconds) allowed between each tap. If this time is exceeded, the multi-tap is canceled.")]
		public float TapDelay;
		/// <summary>
		/// Magnitude threshold that must be crossed by an actuated control for the control to
		/// be considered pressed.
		/// </summary>
		/// <remarks>
		/// If this is less than or equal to 0 (the default), <see cref="InputSettings.defaultButtonPressPoint"/> is used instead.
		/// </remarks>
		/// <seealso cref="InputControl.EvaluateMagnitude()"/>
		public float PressPoint;
		private int _tapCount = 2;

		private double[] m_TapTimes;
		private int m_CurrentTap;
		private double m_FirstTapTime;
		private double _nextInputAllowedOn;

		private float tapTimeOrDefault => TapTime > 0.0 ? TapTime : InputSystem.settings.defaultTapTime;
		internal float tapDelayOrDefault => TapDelay > 0.0 ? TapDelay : InputSystem.settings.multiTapDelayTime;
		private float pressPointOrDefault => PressPoint > 0 ? PressPoint : InputSystem.settings.defaultButtonPressPoint;
		private float releasePointOrDefault => pressPointOrDefault * InputSystem.settings.buttonReleaseThreshold;

		public void Process(ref InputInteractionContext context)
		{
			if (Time.time < _nextInputAllowedOn)
				return;

			if (context.timerHasExpired)
			{
				// We use timers multiple times but no matter what, if they expire it means
				// that we didn't get input in time.
				context.Canceled();
				return;
			}

			switch (m_CurrentTapPhase)
			{
				case TapPhase.None:
					if (context.ControlIsActuated(pressPointOrDefault))
					{
						m_CurrentTapPhase = TapPhase.WaitingForNextRelease;
						m_CurrentTapStartTime = context.time;
						context.Started();

						float maxTapTime = tapTimeOrDefault;
						float maxDelayInBetween = tapDelayOrDefault;
						context.SetTimeout(maxTapTime);

						// We'll be using multiple timeouts so set a total completion time that
						// effects the result of InputAction.GetTimeoutCompletionPercentage()
						// such that it accounts for the total time we allocate for the interaction
						// rather than only the time of one single timeout.
						context.SetTotalTimeoutCompletionTime(maxTapTime * _tapCount + (_tapCount - 1) * maxDelayInBetween);
					}
					break;

				case TapPhase.WaitingForNextRelease:
					if (!context.ControlIsActuated(releasePointOrDefault))
					{
						if (context.time - m_CurrentTapStartTime <= tapTimeOrDefault)
						{
							++m_CurrentTapCount;
							if (m_CurrentTapCount >= _tapCount)
							{
								context.Performed();
								_nextInputAllowedOn = Time.time + TimeBetweenDoubleTaps;
							}
							else
							{
								m_CurrentTapPhase = TapPhase.WaitingForNextPress;
								m_LastTapReleaseTime = context.time;
								context.SetTimeout(tapDelayOrDefault);
							}
						}
						else
						{
							context.Canceled();
						}
					}
					break;

				case TapPhase.WaitingForNextPress:
					if (context.ControlIsActuated(pressPointOrDefault))
					{
						if (context.time - m_LastTapReleaseTime <= tapDelayOrDefault)
						{
							m_CurrentTapPhase = TapPhase.WaitingForNextRelease;
							m_CurrentTapStartTime = context.time;
							context.SetTimeout(tapTimeOrDefault);
						}
						else
						{
							context.Canceled();
						}
					}
					break;
			}
		}

		/// <inheritdoc />
		public void Reset()
		{
			m_CurrentTapPhase = TapPhase.None;
			m_CurrentTapCount = 0;
			m_CurrentTapStartTime = 0;
			m_LastTapReleaseTime = 0;
		}

		private TapPhase m_CurrentTapPhase;
		private int m_CurrentTapCount;
		private double m_CurrentTapStartTime;
		private double m_LastTapReleaseTime;

		private enum TapPhase
		{
			None,
			WaitingForNextRelease,
			WaitingForNextPress,
		}

		[RuntimeInitializeOnLoadMethod]
#if UNITY_EDITOR
		[InitializeOnLoadMethod]
#endif
		private static void Register()
		{
			InputSystem.RegisterInteraction<DoubleTapInteraction>();
		}
	}
}