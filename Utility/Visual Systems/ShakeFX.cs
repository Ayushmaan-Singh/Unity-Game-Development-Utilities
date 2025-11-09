using Unity.Cinemachine;
using UnityEngine;

namespace AstekUtility.VisualFeedback
{
	public class ShakeFX : VisualFX
	{
		[Header("FX")]
		[SerializeField] private CinemachineImpulseSource impulseSource;
		[SerializeField] private float power;

		public float SetImpulseDuration
		{
			set
			{
				impulseSource.ImpulseDefinition.ImpulseDuration = value;
			}
		}

		public override void Play()
		{
			impulseSource.GenerateImpulse(power);
		}
		public override void Stop()
		{
			CinemachineImpulseManager.Instance.Clear();
		}
	}
}