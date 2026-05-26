using System;
using UnityEngine;

namespace Astek.Gameplay.ImprovedTimer
{
	/// <summary>
	/// Timer that counts up from zero to infinity.  Great for measuring durations.
	/// </summary>
	public class StopwatchTimer : Timer
	{
		public Action OnTimerTick = delegate { };

		public StopwatchTimer() : base(0) { }

		public override void Tick()
		{
			if (!IsRunning)
				return;
			
			OnTimerTick.Invoke();
			CurrentTime += Time.deltaTime;
		}

		public override bool IsFinished => false;
	}
}