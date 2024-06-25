using UnityEngine;
using System.Collections;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component will stamp and damage any nearby Destructibles, add physics forces to nearby rigidbody2Ds, and destroy the current GameObject after a set time.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dExplosion")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Explosion")]
	public class D2dExplosion : MonoBehaviour, UnityEngine.ISerializationCallbackReceiver
	{
		/// <summary>The layers the explosion should work on.</summary>
		public LayerMask Mask = -1;

		/// <summary>Should the explosion stamp a shape?</summary>
		public bool Stamp = true;

		/// <summary>Should the stamp exclude a specific destructible object?</summary>
		public D2dDestructible StampExclude;

		/// <summary>The paint type.</summary>
		public D2dDestructible.PaintType StampPaint;

		/// <summary>The shape of the stamp.</summary>
		public D2dShape StampShape;

		/// <summary>The shape of the stamp when it modifies destructible RGB data.</summary>
		public Texture2D StampColorShape;

		/// <summary>The shape of the stamp when it modifies destructible alpha data.</summary>
		public Texture2D StampAlphaShape;

		/// <summary>The stamp shape will be multiplied by this.
		/// Solid White = No Change</summary>
		public Color StampColor = Color.white;

		/// <summary>The size of the explosion stamp in world space.</summary>
		public Vector2 StampSize = new Vector2(1.0f, 1.0f);

		/// <summary>Randomly rotate the stamp?</summary>
		public bool StampRandomDirection = true;

		/// <summary>Should the explosion cast rays?</summary>
		public bool Raycast = true;

		/// <summary>The size of the explosion raycast sphere.</summary>
		public float RaycastRadius = 1.0f;

		/// <summary>The amount of raycasts sent out.</summary>
		public int RaycastCount = 32;

		/// <summary>The amount of force added to objects that the raycasts hit.</summary>
		public float ForcePerRay = 1.0f;

		/// <summary>The amount of damage added to objects that the raycasts hit.</summary>
		public float DamagePerRay = 1.0f;

		/// <summary>Should the explosion damage be full at the center, and fall off to 0 if the hit point is near the outer radius of the explosion?</summary>
		public bool DamageFalloff = true;

		protected virtual void Start()
		{
			if (Stamp == true)
			{
				var stampPosition = transform.position;
				var stampAngle    = StampRandomDirection == true ? Random.Range(-180.0f, 180.0f) : 0.0f;

				D2dStamp.All(StampPaint, stampPosition, StampSize, stampAngle, StampShape, StampColor, Mask, StampExclude);
			}

			if (Raycast == true && RaycastCount > 0)
			{
				StartCoroutine(DelayForce());
			}
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(transform.position, RaycastRadius);
		}
#endif
		private IEnumerator DelayForce()
		{
			yield return new WaitForEndOfFrame();

			// Add force?
			if (ForcePerRay != 0.0f)
			{
				var angleStep = 360.0f / RaycastCount;

				for (var i = 0; i < RaycastCount; i++)
				{
					var angle     = i * angleStep;
					var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var hit       = Physics2D.Raycast(transform.position, direction, RaycastRadius, Mask);
					var collider  = hit.collider;

					// Make sure the raycast hit something, and that it wasn't a trigger
					if (collider != null && collider.isTrigger == false)
					{
						var strength    = 1.0f - hit.fraction;
						var rigidbody2D = collider.attachedRigidbody;
						var damage      = collider.GetComponentInParent<D2dDamage>();

						// // Do less damage if the hit point is far from the explosion?
						if (DamageFalloff == true)
						{
							strength -= hit.fraction;
						}

						if (rigidbody2D != null)
						{
							var force = direction * ForcePerRay * strength;

							rigidbody2D.AddForceAtPosition(force, hit.point);
						}

						if (damage != null)
						{
							damage.Damage += DamagePerRay * strength;
						}
					}
				}
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			try
			{
				if (StampColorShape != null)
				{
					StampShape.Color = StampColorShape; StampColorShape = null;
				}

				if (StampAlphaShape != null)
				{
					StampShape.Alpha = StampAlphaShape; StampAlphaShape = null;
				}
			}
			catch {}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dExplosion;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dExplosion_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Mask", "The layers the explosion should work on.");

			Separator();

			Draw("Stamp", "Should the explosion stamp a shape?");

			if (Any(tgts, t => t.Stamp == true))
			{
				BeginIndent();
					Draw("StampExclude", "Should the stamp exclude a specific destructible object?");
					Draw("StampPaint", "The paint type.");
					Draw("StampShape", "The shape of the stamp.");
					Draw("StampColor", "The stamp shape will be multiplied by this.\nSolid White = No Change");
					Draw("StampSize", "The size of the explosion stamp in world space.");
					Draw("StampRandomDirection", "Randomly rotate the stamp?");
				EndIndent();
			}

			Separator();

			Draw("Raycast", "Should the explosion cast rays?");

			if (Any(tgts, t => t.Raycast == true))
			{
				BeginIndent();
					Draw("RaycastRadius", "The size of the explosion raycast sphere.");
					Draw("RaycastCount", "The amount of raycasts sent out.");
					Draw("ForcePerRay", "The amount of force added to objects that the raycasts hit.");
					Draw("DamagePerRay", "The amount of damage added to objects that the raycasts hit.");
					Draw("DamageFalloff", "Should the explosion damage be full at the center, and fall off to 0 if the hit point is near the outer radius of the explosion?");
				EndIndent();
			}
		}
	}
}
#endif