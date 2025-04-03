using System;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.Gameplay.Timer;
using UnityEngine;

namespace AstekUtility.Gameplay
{
	public class LerpOnPath
	{
		//Lerp property for this run
		private readonly LinkedList<Vector3> _pathPoints = new LinkedList<Vector3>();
		private float _speed = 5f;
		private Transform _source;

		//For internal processing
		private LinkedListNode<Vector3> _currentSegment;
		private float _totalLength;
		private float _currentSegmentLength;
		private float _segmentLengthCovered;
		private readonly StopwatchTimer _stopwatchTimer = new StopwatchTimer();

		public float Progress { get; private set; }
		public bool IsRunning => _stopwatchTimer.IsRunning;
		public bool IsFinished = false;
		public float TimePassed => IsRunning ? _stopwatchTimer.Time : 0;
		public Vector3 Velocity { get; private set; }

		public void Start(Transform source, float speed, params Vector3[] pathPoints)
		{
			if (pathPoints.Length < 2)
				throw new IndexOutOfRangeException("Path must have at least 2 points.");

			_source = source;
			_totalLength = 0;
			pathPoints.ForEach(pathPoint =>
			{
				if (_pathPoints.Count == 0 || (_pathPoints.Count > 0 && _pathPoints.Last.Value != pathPoint))
					_pathPoints.AddLast(pathPoint);
				if (_pathPoints.Count > 1 && _pathPoints.Last.Value != pathPoint)
					_totalLength += _pathPoints.Last.Value.SqrMagnitudeDistance(_pathPoints.Last.Previous.Value);
			});
			_speed = speed;
			_currentSegment = _pathPoints.First;
			Progress = 0;
			_segmentLengthCovered = 0;
			IsFinished = false;


			// Initialize the first segment
			_currentSegmentLength = _currentSegment.Value.SqrMagnitudeDistance(_currentSegment.Next.Value);
			Velocity = (_currentSegment.Next.Value - _currentSegment.Value) * speed;
			_stopwatchTimer.Start();
		}

		public void Update()
		{
			if (!_stopwatchTimer.IsRunning)
				return;

			if (_currentSegment.Next == null)
			{
				// Reached the end of the path
				IsFinished = true;
				Progress = 1f;
				_source.position = _currentSegment.Value;
				_source = null;
				_pathPoints.Clear();
				_speed = 0;
				_currentSegment = null;
				_stopwatchTimer.Stop();
				return;
			}

			// Calculate the distance covered in the current segment
			float distanceCovered = _stopwatchTimer.Time * _speed;

			// Calculate the fraction of the current segment completed
			float interpolationThisSegment = (distanceCovered - _segmentLengthCovered) / _currentSegmentLength;
			Progress = distanceCovered / _totalLength;

			// Lerp between the current segment's start and end points
			Vector3 test = Vector3.Lerp(_currentSegment.Value, _currentSegment.Next.Value, interpolationThisSegment);
			_source.position = Vector3.Lerp(_currentSegment.Value, _currentSegment.Next.Value, interpolationThisSegment);
			Velocity = (_currentSegment.Next.Value - _currentSegment.Value) * _speed;

			// Move to the next segment if the current one is complete
			if (interpolationThisSegment >= 1f)
			{
				_currentSegment = _currentSegment.Next;
				_segmentLengthCovered += _currentSegmentLength;
				if (_currentSegment.Next != null)
				{
					// Initialize the next segment
					_currentSegmentLength = _currentSegment.Value.SqrMagnitudeDistance(_currentSegment.Next.Value);
				}
			}

			_stopwatchTimer.Tick(Time.fixedDeltaTime);
		}
	}
}