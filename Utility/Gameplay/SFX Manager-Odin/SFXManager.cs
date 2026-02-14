#if ODIN_INSPECTOR
using System.Collections.Generic;
using Astek.DesignPattern.ServiceLocatorTool;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Astek.Odin.SFX
{
	public class SFXManager : Singleton<SFXManager>
	{
		[HorizontalGroup("Audio Source")]
		[SerializeField] private AudioSource defaultAudioSource;

		//TODO:Add more as required
		[field:SerializeField, TabGroup("UI"), AssetList(Path = "/_Audio/UI SFX", AutoPopulate = true)] public List<SFXClip> UiSFX { get; private set; }
		[field:SerializeField, TabGroup("Ambient"), AssetList(Path = "/_Audio/Ambient SFX", AutoPopulate = true)] public List<SFXClip> AmbientSFX { get; private set; }
		[field:SerializeField, TabGroup("Weapons"), AssetList(Path = "/_Audio/Weapon SFX", AutoPopulate = true)] public List<SFXClip> WeaponSFX { get; private set; }

		public static void PlaySFX(SFXClip sfx, bool waitToFinish, AudioSource audioSource = null)
		{
			audioSource = (audioSource ?? ServiceLocator.Global.Get<SFXManager>().defaultAudioSource);
			if (audioSource == null)
			{
				Debug.LogError("Default audio source not provided");
				return;
			}

			if (audioSource.isPlaying || waitToFinish)
				return;
			
			audioSource.clip = sfx.clip;
			audioSource.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
			audioSource.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
			audioSource.Play();
		}

		[HorizontalGroup("AudioSource"),ShowIf("@defaultAudioSource == null"),GUIColor(1f,0.5f,0.5f,1f),Button]
		private void AddAudioSource()
		{
			defaultAudioSource = gameObject.GetComponent<AudioSource>();
			if (defaultAudioSource == null)
				defaultAudioSource = gameObject.AddComponent<AudioSource>();
		}
		
		public enum SFXType
		{
			UI,
			Ambient,
			Weapons
		}
	}
}
#endif