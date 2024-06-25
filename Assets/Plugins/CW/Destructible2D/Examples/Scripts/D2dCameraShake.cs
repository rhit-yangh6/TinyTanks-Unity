using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component adds camera shake to the current GameObject.
	/// NOTE: This component takes over the current Transform.localPosition, so you must nest it under another GameObject if you want to move it normally.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dCameraShake")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Camera Shake")]
	public class D2dCameraShake : MonoBehaviour
	{
		/// <summary>All active and enabled D2dCameraShake instances in the scene.</summary>
		public static List<D2dCameraShake> Instances = new List<D2dCameraShake>();

		/// <summary>The current shake strength. This gets reduced automatically.</summary>
		public float Shake;

		/// <summary>The speed at which the Shake value gets reduced.</summary>
		public float ShakeDamping = 10.0f;

		/// <summary>The amount this camera shakes relative to the Shake value.</summary>
		public float ShakeScale = 1.0f;

		/// <summary>The frequency of the camera shake.</summary>
		public float ShakeSpeed = 10.0f;

		// Used to seed/offset the Perlin lookup
		[SerializeField]
		private float offsetX;

		[SerializeField]
		private float offsetY;

		protected virtual void Awake()
		{
			offsetX = Random.Range(-1000.0f, 1000.0f);
			offsetY = Random.Range(-1000.0f, 1000.0f);
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}

		protected virtual void LateUpdate()
		{
			var factor = CwHelper.DampenFactor(ShakeDamping, Time.deltaTime, 0.1f);

			Shake = Mathf.Lerp(Shake, 0.0f, factor);

			var shakeStrength = Shake * ShakeScale;
			var shakeTime     = Time.time * ShakeSpeed;
			var localPosition = transform.localPosition;

			localPosition.x = Mathf.PerlinNoise(offsetX, shakeTime) * shakeStrength;
			localPosition.y = Mathf.PerlinNoise(offsetY, shakeTime) * shakeStrength;

			transform.localPosition = localPosition;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dCameraShake;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dCameraShake_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Shake", "The current shake strength. This gets reduced automatically.");
			Draw("ShakeDamping", "The speed at which the Shake value gets reduced.");
			Draw("ShakeScale", "The amount this camera shakes relative to the Shake value.");
			Draw("ShakeSpeed", "The frequency of the camera shake.");
		}
	}
}
#endif