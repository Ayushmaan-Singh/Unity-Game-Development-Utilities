using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Astek
{
	public static class AnimationCurveExtension
	{
		#region Time From Value

		private const int BinarySearchIterations = 32;
		private const float DefaultTolerance = 0.0001f;

		public static float FindTimeAtValue(this AnimationCurve curve, float targetValue, float tolerance = DefaultTolerance)
		{
			if (curve.length == 0) return -1f;

			// 1. Check for exact matches at keyframes first
			for (int i = 0; i < curve.length; i++)
			{
				if (Math.Abs(curve[i].value - targetValue) < tolerance)
					return curve[i].time;
			}

			// 2. Prepare data for parallel processing
			NativeArray<KeyframeData> keyframeData = new NativeArray<KeyframeData>(
				curve.length, Allocator.TempJob);

			for (int i = 0; i < curve.length; i++)
			{
				keyframeData[i] = new KeyframeData(curve[i]);
			}

			// 3. Create and schedule jobs
			NativeArray<float> results = new NativeArray<float>(
				Math.Max(0, curve.length - 1), Allocator.TempJob);

			var job = new CurveSearchJob
			{
				Keyframes = keyframeData,
				TargetValue = targetValue,
				Tolerance = tolerance,
				Results = results
			};

			JobHandle handle = job.Schedule(results.Length, 32);
			handle.Complete();

			// 4. Find earliest valid result
			float earliestTime = float.MaxValue;
			foreach (float time in results)
			{
				if (time >= 0f && time < earliestTime)
					earliestTime = time;
			}

			// 5. Cleanup
			keyframeData.Dispose();
			results.Dispose();

			return earliestTime < float.MaxValue ? earliestTime : -1f;
		}

		[BurstCompile]
		private struct CurveSearchJob : IJobParallelFor
		{
			[ReadOnly] public NativeArray<KeyframeData> Keyframes;
			public float TargetValue;
			public float Tolerance;
			public NativeArray<float> Results;

			public void Execute(int index)
			{
				if (index >= Keyframes.Length - 1) return;

				KeyframeData start = Keyframes[index];
				KeyframeData end = Keyframes[index + 1];
				Results[index] = FindInSegment(start, end);
			}

			private float FindInSegment(KeyframeData start, KeyframeData end)
			{
				float low = start.Time;
				float high = end.Time;
				float bestTime = -1f;
				float bestError = float.MaxValue;

				for (int i = 0; i < BinarySearchIterations; i++)
				{
					float mid = (low + high) * 0.5f;
					float value = EvaluateSegment(start, end, mid);
					float error = Math.Abs(value - TargetValue);

					if (error < Tolerance)
					{
						return mid;
					}

					if (error < bestError)
					{
						bestError = error;
						bestTime = mid;
					}

					bool isAscending = end.Value > start.Value;
					if ((value < TargetValue && isAscending) ||
					    (value > TargetValue && !isAscending))
					{
						low = mid;
					}
					else
					{
						high = mid;
					}
				}

				return bestError < Tolerance * 2 ? bestTime : -1f;
			}

			private float EvaluateSegment(KeyframeData start, KeyframeData end, float time)
			{
				float t = Mathf.Clamp01((time - start.Time) / (end.Time - start.Time));
				return CubicHermite(start, end, t);
			}

			private float CubicHermite(KeyframeData a, KeyframeData b, float t)
			{
				float t2 = t * t;
				float t3 = t2 * t;

				return (2 * t3 - 3 * t2 + 1) * a.Value +
				       (t3 - 2 * t2 + t) * a.OutTangent +
				       (-2 * t3 + 3 * t2) * b.Value +
				       (t3 - t2) * b.InTangent;
			}
		}

		private struct KeyframeData
		{
			public readonly float Time;
			public readonly float Value;
			public readonly float InTangent;
			public readonly float OutTangent;

			public KeyframeData(Keyframe keyframe)
			{
				Time = keyframe.time;
				Value = keyframe.value;
				InTangent = keyframe.inTangent;
				OutTangent = keyframe.outTangent;
			}
		}

		#endregion
	}
}