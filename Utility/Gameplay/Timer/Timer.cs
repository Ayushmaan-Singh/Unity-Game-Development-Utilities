﻿using System;

namespace AstekUtility.Gameplay.Timer
{

	/// <summary>
	/// Base class for different types of timer classes
	/// </summary>
	public abstract class Timer
	{
		protected float _initialTime;
		public float Time { get; set; }
		public bool IsRunning { get; protected set; }

		public float Progress => Time / _initialTime;

		public Action OnTimerStart = delegate { };
		public Action OnTimerStop = delegate { };

		protected Timer(float value)
		{
			_initialTime = value;
			IsRunning = false;
		}

		public void Start()
		{
			Time = _initialTime;
			if (!IsRunning)
			{
				IsRunning = true;
				OnTimerStart.Invoke();
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				OnTimerStop.Invoke();
			}
		}

		public void Resume() => IsRunning = true;
		public void Pause() => IsRunning = false;

		public abstract void Tick(float deltaTime);
	}

	public class CountdownTimer : Timer
	{
		public new float Progress => (_initialTime - Time) / _initialTime;
		
		public CountdownTimer(float value) : base(value) { }

		public override void Tick(float deltaTime)
		{
			if (IsRunning && Time > 0)
			{
				Time -= deltaTime;
			}

			if (IsRunning && Time <= 0)
			{
				Stop();
			}
		}

		public bool IsFinished => Time <= 0;

		public void Reset() => Time = _initialTime;

		public void Reset(float newTime)
		{
			_initialTime = newTime;
			Reset();
		}
	}

	public class StopwatchTimer : Timer
	{
		public StopwatchTimer() : base(0) { }

		public override void Tick(float deltaTime)
		{
			if (!IsRunning)
				return;
			
			Time += deltaTime;
		}

		public void Reset() => Time = 0;

		public float GetTime() => Time;
	}
}