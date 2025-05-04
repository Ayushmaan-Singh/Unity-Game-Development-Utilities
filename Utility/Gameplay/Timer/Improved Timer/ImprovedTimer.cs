using System;
using UnityEngine;
namespace AstekUtility.Gameplay.ImprovedTimer
{
	public abstract class Timer : IDisposable
	{
		public float CurrentTime { get; protected set; }
		public bool IsRunning { get; private set; }

		protected float _initializeTime;

		public float Progress => Mathf.Clamp(CurrentTime / _initializeTime, 0, 1);

		public Action OnTimerStart = delegate { };
		public Action OnTimerStop = delegate { };

		protected Timer(float value) => _initializeTime = value;


		public void Start()
		{
			CurrentTime = _initializeTime;
			if (!IsRunning)
			{
				IsRunning = true;
				TimerManager.RegisterTimer(this);
				OnTimerStart.Invoke();
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				TimerManager.DeregisterTimer(this);
				OnTimerStop.Invoke();
			}
		}

		public abstract void Tick();
		//Required because some timer like stopwatch count can go for ever
		public abstract bool IsFinished { get; }

		public void Resume() => IsRunning = true;
		public void Pause() => IsRunning = false;

		public virtual void Reset() => CurrentTime = _initializeTime;
		public virtual void Reset(float newTime)
		{
			_initializeTime = newTime;
			Reset();
		}

		private bool _disposed;
		~Timer()
		{
			Dispose(false);
		}

		//Call dispose to ensure deregistration of the timer from the TimerManager
		//when the consumer is done with the timer or being destroyed
		public void Dispose()
		{
			Dispose(true);
			//Tells GC that no need to call the finalizer since all resources have been freed already
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
				TimerManager.DeregisterTimer(this);

			_disposed = true;
		}
	}
}