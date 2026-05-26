using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Astek.VisualFeedback
{
	public class DissolveUniversalFX : MonoBehaviour
	{
		[SerializeField] private Renderer objRenderer;
		[SerializeField] private string dissolveAmountPropertyName;
		[SerializeField, Tooltip("Enter percent per sec")] private float rateOfDissolve;
		[SerializeField, Tooltip("Enter percent per sec")] private float rateOfReappear;

		private MaterialPropertyBlock _matPropertyBlock;
		private int dissolveAmountPropertyID => !string.IsNullOrEmpty(dissolveAmountPropertyName) ? Shader.PropertyToID(dissolveAmountPropertyName) : 0;

		public event Action OnDissolveStart;
		public event Action OnDissolveFinish;

		public event Action OnReappearStart;
		public event Action OnReappearFinish;

		public float Progress { get; private set; }

		private void Awake()
		{
			OnDissolveStart += () => { Progress = 0; };
			OnDissolveFinish += () => { Progress = 1f; };

			OnReappearStart += () => { Progress = 0; };
			OnReappearFinish += () => { Progress = 1f; };
		}

		public async Task Dissolve(CancellationToken ctk = default(CancellationToken))
		{
			OnDissolveStart?.Invoke();

			float amountDissolved = 0;
			_matPropertyBlock = new MaterialPropertyBlock();

			while (amountDissolved <= 1)
			{
				if (ctk.IsCancellationRequested)
					return;

				amountDissolved += rateOfDissolve * Time.deltaTime;
				Progress = amountDissolved;

				_matPropertyBlock.SetFloat(dissolveAmountPropertyID, amountDissolved);
				objRenderer.SetPropertyBlock(_matPropertyBlock);

				await Awaitable.FixedUpdateAsync(ctk);
			}

			_matPropertyBlock.SetFloat(dissolveAmountPropertyID, 1);
			objRenderer.SetPropertyBlock(_matPropertyBlock);
			OnDissolveFinish?.Invoke();
		}

		public async Task Reappear(CancellationToken ctk = default(CancellationToken))
		{
			OnReappearStart?.Invoke();
			float amountReappeared = 0;
			_matPropertyBlock = new MaterialPropertyBlock();

			while (amountReappeared <= 1)
			{
				if (ctk.IsCancellationRequested)
					return;

				amountReappeared += rateOfReappear * Time.deltaTime;
				Progress = amountReappeared;

				//1- is done to reverse dissolve
				_matPropertyBlock.SetFloat(dissolveAmountPropertyID, 1 - amountReappeared);
				objRenderer.SetPropertyBlock(_matPropertyBlock);

				await Awaitable.FixedUpdateAsync(ctk);
			}
			_matPropertyBlock.SetFloat(dissolveAmountPropertyID, 0);
			objRenderer.SetPropertyBlock(_matPropertyBlock);
			OnReappearFinish?.Invoke();
		}

		public void SetDissolveAmount(float amount)
		{
			_matPropertyBlock = new MaterialPropertyBlock();
			_matPropertyBlock.SetFloat(dissolveAmountPropertyID, amount);
			objRenderer.SetPropertyBlock(_matPropertyBlock);
		}
	}
}