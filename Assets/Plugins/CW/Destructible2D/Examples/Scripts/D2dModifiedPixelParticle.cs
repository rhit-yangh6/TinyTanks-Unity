using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component spawns a particle every time you destroy a pixel.
	/// NOTE: This requires you to enable the <b>D2dDestructibleSprite.MonitorPixels</b> setting.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dModifiedPixelParticle")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Modified Pixel Particle")]
	public class D2dModifiedPixelParticle : MonoBehaviour
	{
		/// <summary>The particles will be emitted from this <b>ParticleSystem</b>.</summary>
		public ParticleSystem Emitter { set { emitter = value; } get { return emitter; } } [SerializeField] private ParticleSystem emitter;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		protected virtual void OnEnable()
		{
			cachedDestructible = GetComponent<D2dDestructible>();

			cachedDestructible.OnModifiedPixels += HandleModifiedPixels;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnModifiedPixels -= HandleModifiedPixels;
		}

		private void HandleModifiedPixels(List<int> indices)
		{
			if (emitter != null)
			{
				/*
				var matrix   = cachedDestructible.AlphaToLocalMatrix;
				var particle = new ParticleSystem.EmitParams();
				var width    = cachedDestructible.AlphaWidth;
				var scaleX   = 1.0f / width;
				var scaleY   = 1.0f / cachedDestructible.AlphaHeight;

				foreach (var index in indices)
				{
					var x = index % width + 0.5f;
					var y = index / width + 0.5f;
					var p = new Vector3(x * scaleX, y * scaleY);
					var l = matrix.MultiplyPoint(p);
					var w = cachedDestructible.transform.TransformPoint(l);

					particle.position = w;

					emitter.Emit(particle, 1);
				}
				*/
				var matrix   = cachedDestructible.PixelToWorldMatrix;
				var particle = new ParticleSystem.EmitParams();
				var width    = cachedDestructible.AlphaWidth;

				foreach (var index in indices)
				{
					var x = index % width;
					var y = index / width;
					var p = new Vector3(x, y);
					var w = matrix.MultiplyPoint(p);

					particle.position = w;

					emitter.Emit(particle, 1);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dModifiedPixelParticle;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dModifiedPixelParticle_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Emitter == null));
				Draw("emitter", "The particles will be emitted from this ParticleSystem.");
			EndError();
		}
	}
}
#endif