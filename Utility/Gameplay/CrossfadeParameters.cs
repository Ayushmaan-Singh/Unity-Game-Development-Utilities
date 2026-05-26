using Sirenix.OdinInspector;
using UnityEngine;
namespace Astek.Gameplay
{
	[System.Serializable]
	public class CrossfadeParameters
	{
		[field:SerializeField] public string StateName { get; private set; }
		[field:SerializeField] public int LayerIndex = 0;
		[field:SerializeField] public bool CanTransitionToSelf = false;
		public int StateHashId => !string.IsNullOrEmpty(StateName) ? Animator.StringToHash(StateName) : 0;
		[field:SerializeField] public bool FixedTime = false;

		[field:SerializeField, Range(0f, 1f), ShowIf("@!FixedTime")] public float NormalizedTransitionDuration { get; private set; }
		[field:SerializeField, Range(0f, 1f), ShowIf("@!FixedTime")] public float NormalizedTimeOffset { get; private set; }

		[field:SerializeField, ShowIf("@FixedTime")] public float FixedTransitionDuration { get; private set; }
		[field:SerializeField, ShowIf("@FixedTime")] public float FixedTimeOffset { get; private set; }
	}
}