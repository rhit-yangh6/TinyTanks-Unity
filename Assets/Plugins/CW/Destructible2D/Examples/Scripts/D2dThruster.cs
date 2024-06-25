using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current GameObject into a thruster that can apply forces to its parent <b>Rigidbody2D</b>.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dThruster")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Thruster")]
	public class D2dThruster : MonoBehaviour
	{
		/// <summary>The current throttle amount.</summary>
		public float Throttle;

		/// <summary>The scale of this thruster when throttle is 1.</summary>
		public Vector3 MaxScale = Vector3.one;
		
		/// <summary>How quickly the throttle scales to the desired value.</summary>
		public float Damping = 10.0f;

		/// <summary>The amount of force applied to the rigidbody2D when throttle is 1.</summary>
		public float MaxForce = 1.0f;

		/// <summary>The amount the thruster effect can flicker.</summary>
		public float Flicker = 0.1f;

		// The rigidbody this thruster is attached to
		[System.NonSerialized]
		private Rigidbody2D body;

		// The current interpolated throttle value
		[SerializeField]
		private float currentThrottle;

		protected virtual void FixedUpdate()
		{
			if (body == null) body = GetComponentInParent<Rigidbody2D>();

			if (body != null)
			{
				body.AddForceAtPosition(transform.up * MaxForce * -Throttle, transform.position, ForceMode2D.Force);
			}
		}

		protected virtual void Update()
		{
			var factor = CwHelper.DampenFactor(Damping, Time.deltaTime);

			currentThrottle = Mathf.Lerp(currentThrottle, Throttle, factor);
			
			transform.localScale = MaxScale * Random.Range(1.0f - Flicker, 1.0f + Flicker) * currentThrottle;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dThruster;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dThruster_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Throttle", "The current throttle amount.");
			Draw("MaxScale", "The scale of this thruster when throttle is 1.");
			BeginError(Any(tgts, t => t.Damping < 0.0f));
				Draw("Damping", "How quickly the throttle scales to the desired value.");
			EndError();
			Draw("MaxForce", "The amount of force applied to the rigidbody2D when throttle is 1.");
			Draw("Flicker", "The amount the thruster effect can flicker.");
		}
	}
}
#endif