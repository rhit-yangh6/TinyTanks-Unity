using UnityEngine;
using System.Collections;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component can be used to slice between two points and spawn particles between the slice points.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSlicer")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Slicer")]
	public class D2dSlicer : MonoBehaviour, UnityEngine.ISerializationCallbackReceiver
	{
		/// <summary>The slice will start from this local point.</summary>
		public Vector3 LocalStart;

		/// <summary>The slice will end at this local point.</summary>
		public Vector3 LocalEnd = Vector3.up;

		/// <summary>If you want to continuously slice then set the slice interval here in seconds.
		/// -1 = Manual
		/// 0 = Start</summary>
		public float Interval;

		/// <summary>The paint type used when stamping.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the stamp.</summary>
		public D2dShape StampShape;

		/// <summary>The shape of the stamp when it modifies destructible RGB data.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("Shape")]
		public Texture2D ColorShape;

		/// <summary>The shape of the stamp when it modifies destructible alpha data.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("Shape")]
		public Texture2D AlphaShape;

		/// <summary>The thickness of the slice in world space.</summary>
		public float Thickness = 0.25f;

		/// <summary>The color tint of the slice.</summary>
		public Color Color = Color.white;

		/// <summary>The layers that will be sliced.</summary>
		public LayerMask Layers = -1;

		/// <summary>Should the stamp exclude a specific destructible object?</summary>
		public D2dDestructible Exclude;

		/// <summary>If you want particles to be spawned along the slice line, specify the particle system here.</summary>
		public ParticleSystem ParticleSystem;

		/// <summary>The amount of particles that will be spawned per world unit length of the slice line.</summary>
		public float ParticlesPerUnit = 10.0f;

		/// <summary>If you want the particles to spawn at random points along the line, enable this.</summary>
		public bool ParticlesRandom = true;

		/// <summary>The amount of outward force that will be added to sliced objects.</summary>
		public float Force = 10.0f;

		/// <summary>The type of force that will be added.</summary>
		public ForceMode2D ForceMode;

		private float cooldown;

		public void Slice()
		{
			var positionA = transform.TransformPoint(LocalStart);
			var positionB = transform.TransformPoint(LocalEnd  );

			D2dSlice.All(Paint, positionA, positionB, Thickness, StampShape, Color, Layers, Exclude);

			// The slice won't happen until next frame, so delay the force application
			if (Force != 0.0f)
			{
				StartCoroutine(DelayedForce(positionA, positionB));
			}

			if (ParticleSystem != null && ParticlesPerUnit > 0.0f)
			{
				var particleCount = Mathf.CeilToInt(Vector3.Distance(positionA, positionB) * ParticlesPerUnit);

				if (particleCount > 0.0f)
				{
					var emitParams = new ParticleSystem.EmitParams();
					var positionD  = positionB - positionA;

					if (ParticlesRandom == true)
					{
						for (var i = 0; i < particleCount; i++)
						{
							emitParams.position = positionA + positionD * Random.value;
							emitParams.velocity = Random.insideUnitSphere;

							ParticleSystem.Emit(emitParams, 1);
						}
					}
					else
					{
						var step = positionD / particleCount;

						emitParams.position = positionA + step * 0.5f;

						for (var i = 0; i < particleCount; i++)
						{
							emitParams.velocity = Random.insideUnitSphere;

							ParticleSystem.Emit(emitParams, 1);

							emitParams.position += step;
						}
					}
				}
			}
		}

		/// <summary>This will transform the current slicer between the input positions.</summary>
		public void SetTransform(Vector2 positionA, Vector2 positionB)
		{
			var scale = Vector2.Distance(positionB, positionA);
			var angle = CwHelper.Atan2(positionB - positionA) * Mathf.Rad2Deg;

			// Transform the indicator so it lines up with the slice
			transform.position   = positionA;
			transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
			transform.localScale = new Vector3(Thickness, scale, scale);
		}

		protected virtual void Start()
		{
			if (Interval == 0)
			{
				Slice();
			}
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (Interval > 0.0f)
			{
				if (cooldown <= 0.0f)
				{
					cooldown = Interval;

					Slice();
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			var pointA = transform.TransformPoint(LocalStart);
			var pointB = transform.TransformPoint(LocalEnd  );

			Gizmos.DrawWireSphere(pointA, UnityEditor.HandleUtility.GetHandleSize(pointA) * 0.25f);
			Gizmos.DrawWireSphere(pointB, UnityEditor.HandleUtility.GetHandleSize(pointB) * 0.25f);

			Gizmos.DrawLine(pointA, pointB);
		}
#endif

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			try
			{
				if (ColorShape != null)
				{
					StampShape.Color = ColorShape; ColorShape = null;
				}

				if (AlphaShape != null)
				{
					StampShape.Alpha = AlphaShape; AlphaShape = null;
				}
			}
			catch {}
		}

		private IEnumerator DelayedForce(Vector3 oldPosition, Vector3 newPosition)
		{
			yield return new WaitForEndOfFrame();

			D2dSlice.ForceAll(oldPosition, newPosition, Thickness, Force, ForceMode, Layers);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dSlicer;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSlicer_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("LocalStart", "The slice will start from this local point.");
			Draw("LocalEnd", "The slice will end at this local point.");
			Draw("Interval", "If you want to continuously slice then set the slice interval here in seconds.\n-1 = Manual\n0 = Start");
			Draw("Thickness", "The thickness of the slice in world space.");
			BeginError(Any(tgts, t => t.Layers == 0));
				Draw("Layers", "The layers that will be sliced.");
			EndError();
			Draw("Exclude", "Should the stamp exclude a specific destructible object?");

			Separator();

			Draw("Paint", "The paint type used when stamping.");
			Draw("StampShape", "The shape of the stamp.");
			Draw("Color", "The color tint of the slice.");

			Separator();

			Draw("ParticleSystem", "If you want particles to be spawned along the slice line, specify the particle system here.");
			BeginError(Any(tgts, t => t.ParticlesPerUnit <= 0.0f));
				Draw("ParticlesPerUnit", "The amount of particles that will be spawned per world unit length of the slice line.");
			EndError();
			Draw("ParticlesRandom", "If you want the particles to spawn at random points along the line, enable this.");

			Separator();

			Draw("Force", "The amount of outward force that will be added to sliced objects.");
			Draw("ForceMode", "The type of force that will be added.");
		}
	}
}
#endif