using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace AstekUtility.VisualFeedback
{
	/// <summary>
	/// To dissolve => dissolveFX.SetFXState(FXState.Dissolve).Play();
	/// To appear => dissolveFX.SetFXState(FXState.Appear).Play();
	/// </summary>
	public class DissolveFX : MonoBehaviour
	{
		public enum InterpolationMode
		{
			Constant,
			Curve
		}
		public enum FXState
		{
			Dissolve,
			Appear
		}

		[SerializeField] private Renderer objRenderer;

		#if UNITY_EDITOR
		[ValueDropdown("@OdinMethodExtension.MaterialProperties(objRenderer)")]
		#endif
		[SerializeField] private int dissolveAmountPropertyID;
		[SerializeField] private FXState currentState = FXState.Dissolve;
		[SerializeField] private InterpolationMode interpolationMode = InterpolationMode.Constant;

		//Dissolve Properties Curve
		[SerializeField, BoxGroup("Dissolve Property Curve"), ShowIf("@interpolationMode==InterpolationMode.Curve")]
		private AnimationCurve dissolveCurve;
		[SerializeField, BoxGroup("Dissolve Property Curve"), ShowIf("@interpolationMode==InterpolationMode.Curve")]
		private float dissolveDuration = 0.5f;

		//Dissolve Properties Constant
		[SerializeField, BoxGroup("Dissolve Property Constant"), ShowIf("@interpolationMode==InterpolationMode.Constant")]
		private float dissolveAmountPerSec = 2;

		//Appear Properties Curve
		[SerializeField, BoxGroup("Reappear Property Curve"), ShowIf("@interpolationMode==InterpolationMode.Curve")]
		private AnimationCurve reappearCurve;
		[SerializeField, BoxGroup("Reappear Property Curve"), ShowIf("@interpolationMode==InterpolationMode.Curve")]
		private float reappearDuration = 0.5f;

		//Appear Properties Constant
		[SerializeField, BoxGroup("Reappear Property Constant"), ShowIf("@interpolationMode==InterpolationMode.Constant")]
		private float reappearAmountPerSec = 2;

		private CancellationTokenSource _cts;
		private Task _currentTask;
		public bool IsRunning => _currentTask != null && (_currentTask.IsCompleted || _currentTask.IsCanceled);

		[ShowInInspector] public float DissolveAmount
		{
			get
			{
				if (objRenderer != null && Application.isPlaying)
				{
					MaterialPropertyBlock _cachedPropertyBlock = new MaterialPropertyBlock();
					objRenderer.GetPropertyBlock(_cachedPropertyBlock);

					// 1. Check MaterialPropertyBlock override
					if (_cachedPropertyBlock.HasProperty(dissolveAmountPropertyID))
						return _cachedPropertyBlock.GetFloat(dissolveAmountPropertyID);

					// 2. Fall back to shared material value
					return GetComponent<Renderer>().sharedMaterial.GetFloat(dissolveAmountPropertyID);
				}
				else
					return 0;
			}
		}

		public float Progress { get; private set; }

		public async Task Play()
		{
			// Cancel and wait for the previous task to finish
			if (_currentTask != null && !_currentTask.IsCompleted)
			{
				_cts?.Cancel();
				try
				{
					await _currentTask; // Wait for cancellation to complete
				}
				catch (OperationCanceledException)
				{
					/* Ignore */
				}
				finally
				{
					_cts?.Dispose();
					_cts = null;
				}
			}

			// Start the new effect
			_cts = new CancellationTokenSource();

			try
			{
				// Directly assign the effect task to _currentTask
				_currentTask = currentState switch
				{
					FXState.Dissolve when !Mathf.Approximately(DissolveAmount, 1) =>
						interpolationMode == InterpolationMode.Constant
							? DissolveConstant(_cts.Token)
							: DissolveCurve(_cts.Token),

					FXState.Appear when DissolveAmount != 0 =>
						interpolationMode == InterpolationMode.Constant
							? AppearConstant(_cts.Token)
							: AppearCurve(_cts.Token),

					_ => throw new InvalidDataException("Invalid FXState or already at target")
				};

				await _currentTask; // Await the effect task
			}
			catch (OperationCanceledException)
			{
				//Add logic to handle cancellation
			}
		}
		public void Stop()
		{
			_cts?.Cancel();
			_cts?.Dispose();
		}

		#region Dissolve And Appear Operations

		//Dissolve
		private async Task DissolveCurve(CancellationToken ctx)
		{
			float timeCounter = 0;

			//Reappear was completed last time
			if (Mathf.Approximately(Progress, 1f))
			{
				timeCounter = 0;
				Progress = 0;
			}
			//Reappear was not completed last time
			else
			{
				Progress = Mathf.Clamp01(dissolveCurve.FindTimeAtValue(DissolveAmount) / dissolveDuration);
				timeCounter = dissolveDuration * Progress;
			}

			MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
			objRenderer.GetPropertyBlock(propertyBlock);

			while (dissolveDuration > 0 && timeCounter / dissolveDuration < 1
			                            && !ctx.IsCancellationRequested
			                            && !destroyCancellationToken.IsCancellationRequested)
			{
				propertyBlock.SetFloat(dissolveAmountPropertyID,
					dissolveCurve.Evaluate(timeCounter / dissolveDuration));
				objRenderer.SetPropertyBlock(propertyBlock);

				timeCounter += Time.deltaTime;
				Progress = Mathf.Clamp01(timeCounter / dissolveDuration);
				await Awaitable.NextFrameAsync(_cts.Token);
			}
			propertyBlock.SetFloat(dissolveAmountPropertyID,
				dissolveCurve.Evaluate(1));
			objRenderer.SetPropertyBlock(propertyBlock);
			Progress = 1f;
		}

		private async Task DissolveConstant(CancellationToken ctx)
		{
			float initialDissolveAmount = DissolveAmount;
			float totalTime = Mathf.Clamp01(1 - DissolveAmount) / dissolveAmountPerSec;
			float timeCounter = 0;
			Progress = 0;

			MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
			objRenderer.GetPropertyBlock(propertyBlock);

			while (totalTime > 0 && timeCounter / totalTime < 1
			                     && !ctx.IsCancellationRequested
			                     && !destroyCancellationToken.IsCancellationRequested)
			{

				propertyBlock.SetFloat(dissolveAmountPropertyID,
					Mathf.Lerp(initialDissolveAmount, 1, timeCounter / totalTime));
				objRenderer.SetPropertyBlock(propertyBlock);

				timeCounter += Time.deltaTime;
				Progress = Mathf.Clamp01(timeCounter / totalTime);
				await Awaitable.NextFrameAsync(_cts.Token);
			}

			propertyBlock.SetFloat(dissolveAmountPropertyID,
				Mathf.Lerp(initialDissolveAmount, 1, 1));
			objRenderer.SetPropertyBlock(propertyBlock);
			Progress = 1f;
		}

		//Appear
		private async Task AppearCurve(CancellationToken ctx)
		{
			float timeCounter = 0;

			//Reappear was completed last time
			if (Mathf.Approximately(Progress, 0f))
			{
				timeCounter = 0;
				Progress = 0;
			}
			//Reappear was not completed last time
			else
			{
				Progress = Mathf.Clamp01(reappearCurve.FindTimeAtValue(DissolveAmount) / reappearDuration);
				timeCounter = reappearDuration * Progress;
			}

			MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
			objRenderer.GetPropertyBlock(propertyBlock);

			while (dissolveDuration > 0 && timeCounter / reappearDuration < 1
			                            && !ctx.IsCancellationRequested
			                            && !destroyCancellationToken.IsCancellationRequested)
			{
				propertyBlock.SetFloat(dissolveAmountPropertyID,
					reappearCurve.Evaluate(timeCounter / reappearDuration));
				objRenderer.SetPropertyBlock(propertyBlock);

				timeCounter += Time.deltaTime;
				Progress = Mathf.Clamp01(timeCounter / reappearDuration);
				await Awaitable.NextFrameAsync(_cts.Token);
			}
			propertyBlock.SetFloat(dissolveAmountPropertyID,
				reappearCurve.Evaluate(1));
			objRenderer.SetPropertyBlock(propertyBlock);
			Progress = 1f;
			propertyBlock.Clear();
		}

		private async Task AppearConstant(CancellationToken ctx)
		{
			float initialDissolveAmount = DissolveAmount;
			float totalTime = DissolveAmount / reappearAmountPerSec;
			float timeCounter = 0;
			Progress = 0f;

			MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
			objRenderer.GetPropertyBlock(propertyBlock);

			while (totalTime > 0 && timeCounter / totalTime < 1
			                     && !ctx.IsCancellationRequested
			                     && !destroyCancellationToken.IsCancellationRequested)
			{

				propertyBlock.SetFloat(dissolveAmountPropertyID,
					Mathf.Lerp(initialDissolveAmount, 0, timeCounter / totalTime));
				objRenderer.SetPropertyBlock(propertyBlock);

				timeCounter += Time.deltaTime;
				Progress = timeCounter / totalTime;
				await Awaitable.NextFrameAsync(_cts.Token);
			}

			propertyBlock.SetFloat(dissolveAmountPropertyID,
				Mathf.Lerp(initialDissolveAmount, 0, 1));
			objRenderer.SetPropertyBlock(propertyBlock);
			Progress = 1f;
		}

		#endregion

		#region Setters

		public DissolveFX SetInterpolationMode(InterpolationMode mode)
		{
			interpolationMode = mode;
			return this;
		}
		public DissolveFX SetDissolveCurve(AnimationCurve curve)
		{
			dissolveCurve = curve;
			return this;
		}
		public DissolveFX SetDissolveDuration(float duration)
		{
			this.dissolveDuration = duration;
			return this;
		}
		public DissolveFX SetDissolveAmountPerSec(float amountPerSec)
		{
			dissolveAmountPerSec = amountPerSec;
			return this;
		}
		public DissolveFX SetFXState(FXState state)
		{
			currentState = state;
			return this;
		}
		public DissolveFX SetDissolveAmount(float amount)
		{
			MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
			objRenderer.GetPropertyBlock(propertyBlock);
			propertyBlock.SetFloat(dissolveAmountPropertyID, Mathf.Clamp01(amount));
			objRenderer.SetPropertyBlock(propertyBlock);
			return this;
		}

		#endregion
	}
}