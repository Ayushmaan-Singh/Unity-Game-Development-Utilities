using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor.Animations;
using UnityEngine;

namespace AstekUtility.Gameplay
{
	public class CrossfadeAnimData : SerializedScriptableObject
	{
		[SerializeField] private string crossFadeAnimStateName;
		[field:SerializeField] public int LayerIndex { get; private set; }
		[SerializeField] private AnimationClip animationClip;
		[field:SerializeField] public bool IsLooping { get; private set; } = false;

		#region Weight

		[OdinSerialize, InlineProperty] private Dictionary<int, Value> weightsPerLayer;
		public Dictionary<int, float> GetWeights
		{
			get
			{
				Dictionary<int, float> weights = new Dictionary<int, float>();
				weightsPerLayer.ForEach(keyValue => weights.Add(keyValue.Key, keyValue.Value.Weight));
				return weights;
			}
		}

		private struct Value
		{
			[InlineProperty, Range(0f, 1f)] public float Weight;
		}

		#endregion

		[field:SerializeField] public bool IsLocked { get; private set; } = false;

		[PropertySpace]
		[field:SerializeField] public bool UseNormalizedTime { get; private set; } = true;

		[Title("Normalized Transition settings"), PropertySpace]
		[field:SerializeField, Range(0f, 1f), ShowIf("@UseNormalizedTime==true")] public float NormalizedTimeDuration { get; private set; }

		[Title("Fixed Time Transition settings")]
		[field:SerializeField, ShowIf("@UseNormalizedTime==false")] public float FixedTimeDuration { get; private set; }

		public int StateID => Animator.StringToHash(crossFadeAnimStateName);
		public float ClipLength => animationClip != null ? animationClip.length : 0f;

		#region Equating

		public override int GetHashCode()
		{
			return StateID + LayerIndex;
		}
		public override bool Equals(object other)
		{
			return other is CrossfadeAnimData cs && cs == this;
		}

		public static bool operator ==(CrossfadeAnimData c1, CrossfadeAnimData c2) => c1?.LayerIndex == c2?.LayerIndex && c1?.StateID == c2?.StateID;
		public static bool operator !=(CrossfadeAnimData c1, CrossfadeAnimData c2) => !(c1 == c2);

		#endregion
	}
}