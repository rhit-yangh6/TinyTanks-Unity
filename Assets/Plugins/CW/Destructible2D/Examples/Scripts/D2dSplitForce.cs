using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component will automatically add outward force to each chunk of a destruction after it gets split or fractured.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSplitForce")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Split Force")]
	public class D2dSplitForce : MonoBehaviour
	{
		/// <summary>Only apply force for certain kinds of splits?</summary>
		public D2dDestructible.SplitMode ApplyTo { set { applyTo = value; } get { return applyTo; } } [SerializeField] private D2dDestructible.SplitMode applyTo = D2dDestructible.SplitMode.Fracture;

		/// <summary>The force applied to split chunks when their distance to the center is 1.</summary>
		public float Force { set { force = value; } get { return force; } } [UnityEngine.Serialization.FormerlySerializedAs("Force")] [SerializeField] private float force = 1000.0f;

		/// <summary>The <b>Force</b> value is incremented by this based on the <b>AlphaCount</b> of the destructible.</summary>
		public float ForcePerSolidPixel { set { forcePerSolidPixel = value; } get { return forcePerSolidPixel; } } [SerializeField] private float forcePerSolidPixel = 0.01f;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[System.NonSerialized]
		private Rigidbody2D cachedRigidbody2D;

		[System.NonSerialized]
		private float strength;

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();
			if (cachedRigidbody2D  == null) cachedRigidbody2D  = GetComponent<Rigidbody2D>();

			cachedDestructible.OnSplitStart += HandleSplitStart;
			cachedDestructible.OnSplitEnd   += HandleSplitEnd;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnSplitStart -= HandleSplitStart;
			cachedDestructible.OnSplitEnd   -= HandleSplitEnd;
		}

		private void HandleSplitStart()
		{
			strength = force + forcePerSolidPixel * cachedDestructible.AlphaCount;
		}

		private void HandleSplitEnd(List<D2dDestructible> splitDestructibles, D2dDestructible.SplitMode mode)
		{
			if (splitDestructibles.Count > 0 && (applyTo == D2dDestructible.SplitMode.All || applyTo == mode))
			{
				var center = Vector2.zero;

				for (var i = splitDestructibles.Count - 1; i >= 0; i--)
				{
					var splitRigidbody2D = splitDestructibles[i].GetComponent<Rigidbody2D>();
					var splitCenter      = (Vector2)splitRigidbody2D.transform.TransformPoint(splitRigidbody2D.centerOfMass);

					center += splitCenter;
				}

				center /= splitDestructibles.Count;

				for (var i = splitDestructibles.Count - 1; i >= 0; i--)
				{
					var splitRigidbody2D = splitDestructibles[i].GetComponent<Rigidbody2D>();

					var splitCenter = (Vector2)splitRigidbody2D.transform.TransformPoint(splitRigidbody2D.centerOfMass);
					var vector      = splitCenter - center;
					var distance    = vector.magnitude;

					if (distance == 0.0f)
					{
						var a = Random.Range(-Mathf.PI, Mathf.PI);

						vector.x = Mathf.Sin(a);
						vector.y = Mathf.Cos(a);
					}

					splitRigidbody2D.AddForce(vector.normalized * strength, ForceMode2D.Impulse);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dSplitForce;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSplitForce_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("applyTo", "Only apply force for certain kinds of splits?");
			BeginError(Any(tgts, t => t.Force == 0.0f && t.ForcePerSolidPixel == 0.0f));
				Draw("force", "The force applied to split chunks when their distance to the center is 1.");
				Draw("forcePerSolidPixel", "The Force value is incremented by this based on the AlphaCount of the destructible.");
			EndError();
		}
	}
}
#endif