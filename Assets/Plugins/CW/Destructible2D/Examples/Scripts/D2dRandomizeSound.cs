using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component randomly changes the sound's pitch and clip to give lots of sound variation.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(AudioSource))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dRandomizeSound")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Randomize Sound")]
	public class D2dRandomizeSound : MonoBehaviour
	{
		/// <summary>The minimum pitch of this sound.</summary>
		public float PitchMin = 0.9f;

		/// <summary>The maximum pitch of this sound.</summary>
		public float PitchMax = 1.1f;

		/// <summary>The audio clips that can be given to this sound.</summary>
		public AudioClip[] Clips;

		protected virtual void Awake()
		{
			var audioSource = GetComponent<AudioSource>();

			audioSource.pitch = Random.Range(PitchMin, PitchMax);

			if (Clips != null && Clips.Length > 0)
			{
				audioSource.clip = Clips[Random.Range(0, Clips.Length)];
			}

			audioSource.Play();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			var audioSource = GetComponent<AudioSource>();

			audioSource.playOnAwake = false;

			Clips = new AudioClip[] { audioSource.clip };
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dRandomizeSound;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dRandomizeSound_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("PitchMin", "The minimum pitch of this sound.");
			Draw("PitchMax", "The maximum pitch of this sound.");
			Draw("Clips", "The audio clips that can be given to this sound.");
		}
	}
}
#endif