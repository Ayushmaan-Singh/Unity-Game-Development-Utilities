using System.Collections.Generic;

namespace Astek.Gameplay.ImprovedTimer
{
	public static class TimerManager
	{
		private static readonly List<Timer> _UPDATETIMERS = new List<Timer>();

		public static void RegisterTimer(Timer timer)
		{
			if (!_UPDATETIMERS.Contains(timer))
				_UPDATETIMERS.Add(timer);
		}
		public static void DeregisterTimer(Timer timer) => _UPDATETIMERS.Remove(timer);


		public static void UpdateTimers()
		{
			foreach (Timer timer in new List<Timer>(_UPDATETIMERS))
			{
				timer.Tick();
			}
		}

		public static void Clear() => _UPDATETIMERS.Clear();

	}
}