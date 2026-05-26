using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Astek.VisualFeedback
{
	public class SineWaveFX : MonoBehaviour
	{
		[SerializeField, ShowIf("@rectTransformToModify==null")] private Transform transformToModify;
		[SerializeField, ShowIf("@transformToModify==null")] private RectTransform rectTransformToModify;

		[SerializeField] private float deviationAmount;
		[SerializeField] private Vector3 deviationDirection;
		[SerializeField, Tooltip("Magnitude of change in angle per second")] private float deltaAnglePerSec = 10;

		[PropertySpace(40)]
		[SerializeField] private float initialAngle;
		[SerializeField] private float finalAngle;

		public event Action OnStart;
		public event Action OnFinish;
		public float Progress { get; private set; }

		private void Awake()
		{
			OnStart += () => { Progress = 0; };
			OnFinish += () => { Progress = 1f; };
		}

		public async Task ExecuteTransformFX()
		{
			if (transformToModify == null)
				throw new NullReferenceException("No Transform provided for modification");

			OnStart?.Invoke();
			await OscillateTransform(transformToModify, initialAngle, finalAngle);
		}

		public async Task ExecuteTransformFX(float angle1, float angle2)
		{
			if (transformToModify == null)
				throw new NullReferenceException("No Transform provided for modification");

			OnStart?.Invoke();
			await OscillateTransform(transformToModify, initial:angle1, final:angle2);
		}

		public async Task ExecuteRectTransformFX()
		{
			if (rectTransformToModify == null)
				throw new NullReferenceException("No RectTransform provided for modification");

			OnStart?.Invoke();
			await OscillateRectTransform(rectTransformToModify, initialAngle, finalAngle);
		}

		public async Task ExecuteRectTransformFX(float angle1, float angle2)
		{
			if (rectTransformToModify == null)
				throw new NullReferenceException("No RectTransform provided for modification");

			OnStart?.Invoke();
			await OscillateRectTransform(rectTransformToModify, initial:angle1, final:angle2);
		}


		private async Task OscillateTransform(Transform entity, float initial, float final)
		{
			float angle = initial;
			float totalTime = Mathf.Abs(final - initial) / deltaAnglePerSec;
			float timeCounter = 0;

			Vector3 initialPosition = entity.position;
			while (Progress <= 1f)
			{
				entity.position = initialPosition + (deviationAmount * deviationDirection * Mathf.Sin(angle * Mathf.Deg2Rad));

				timeCounter += Time.fixedDeltaTime;
				Progress = timeCounter / totalTime;
				angle = Mathf.Lerp(initial, final, Progress);

				await Awaitable.FixedUpdateAsync();
			}

			entity.position = initialPosition;
			OnFinish?.Invoke();
		}

		private async Task OscillateRectTransform(RectTransform entity, float initial, float final)
		{
			float angle = initial;
			float totalTime = (final - initial) / deltaAnglePerSec;
			float timeCounter = 0;

			Vector3 initialPosition = entity.anchoredPosition;
			while (angle <= final)
			{
				entity.anchoredPosition = initialPosition + (deviationAmount * deviationDirection * Mathf.Sin(angle * Mathf.Deg2Rad));

				timeCounter += Time.fixedDeltaTime;
				Progress = timeCounter / totalTime;
				angle = Mathf.Lerp(initial, final, Progress);

				await Awaitable.FixedUpdateAsync();
			}
			entity.position = initialPosition;
			OnFinish?.Invoke();
		}
	}
}