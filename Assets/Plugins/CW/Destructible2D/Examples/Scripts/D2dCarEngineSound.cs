using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component implements very basic physics for a 2D wheel in a top-down perspective.</summary>
	[RequireComponent(typeof(AudioSource))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dCarEngineSound")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Car Engine Sound")]
	public class D2dCarEngineSound : MonoBehaviour
	{
		/// <summary>The wheel whose speed we will use for pitch.</summary>
		public D2dWheel Wheel;

		/// <summary>The sound pitch when not moving.</summary>
		public float IdlePitch = 1.0f;

		/// <summary>The sound pitch shift for each given speed value.</summary>
		public float SpeedPitch = 0.05f;

		[System.NonSerialized]
		private AudioSource cachedAudioSource;

		protected virtual void OnEnable()
		{
			if (cachedAudioSource == null) cachedAudioSource = GetComponent<AudioSource>();
		}

		protected virtual void Update()
		{
			if (Wheel != null)
			{
				cachedAudioSource.pitch = IdlePitch + SpeedPitch * Mathf.Abs(Wheel.Speed);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dCarEngineSound;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dCarEngineSound_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Wheel", "The wheel whose speed we will use for pitch.");
			Draw("IdlePitch", "The sound pitch when not moving.");
			Draw("SpeedPitch", "The sound pitch shift for each given speed value.");
		}
	}
}
#endif