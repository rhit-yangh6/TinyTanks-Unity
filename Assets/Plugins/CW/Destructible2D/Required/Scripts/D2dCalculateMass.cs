using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component automatically sets the Rigidbody2D.mass based on the D2dDestructible.AlphaCount.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dCalculateMass")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Calculate Mass")]
	public class D2dCalculateMass : MonoBehaviour
	{
		/// <summary>The amount of mass added to the Rigidbody2D by each solid pixel in the attached D2dDestructible.</summary>
		public float MassPerSolidPixel { set { massPerSolidPixel = value; } get { return massPerSolidPixel; } } [SerializeField] private float massPerSolidPixel = 0.01f;

		/// <summary>Automatically multiply the mass by the D2dDestructible.AlphaSharpness value to account for optimizations?</summary>
		public bool FactorInSharpness { set { factorInSharpness = value; } get { return factorInSharpness; } } [SerializeField] private bool factorInSharpness = true;

		[System.NonSerialized]
		private Rigidbody2D cachedRigidbody2D;

		[System.NonSerialized]
		private bool cachedRigidbody2DSet;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[System.NonSerialized]
		private bool cachedDestructibleSet;

		[System.NonSerialized]
		private float lastSetMass = -1.0f;

		[ContextMenu("Update Mass")]
		public void UpdateMass()
		{
			if (cachedRigidbody2DSet == false)
			{
				cachedRigidbody2D    = GetComponent<Rigidbody2D>();
				cachedRigidbody2DSet = true;
			}

			if (cachedDestructibleSet == false)
			{
				cachedDestructible    = GetComponent<D2dDestructible>();
				cachedDestructibleSet = true;
			}

			var newMass = cachedDestructible.AlphaCount * MassPerSolidPixel;

			if (factorInSharpness == true)
			{
				newMass *= cachedDestructible.AlphaSharpness * cachedDestructible.AlphaSharpness;
			}

			if (newMass != lastSetMass)
			{
				cachedRigidbody2D.mass = lastSetMass = newMass;
			}
		}

		protected virtual void OnEnable()
		{
			if (cachedDestructibleSet == false)
			{
				cachedDestructible    = GetComponent<D2dDestructible>();
				cachedDestructibleSet = true;
			}

			cachedDestructible.OnModified += HandleModified;
			cachedDestructible.OnRebuilt  += UpdateMass;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnModified -= HandleModified;
			cachedDestructible.OnRebuilt  -= UpdateMass;
		}

		protected virtual void Start()
		{
			UpdateMass();
		}

		private void HandleModified(D2dRect rect)
		{
			UpdateMass();
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dCalculateMass;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dCalculateMass_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("massPerSolidPixel", "The amount of mass added to the Rigidbody2D by each solid pixel in the attached D2dDestructible.");
			Draw("factorInSharpness", "Automatically multiply the mass by the D2dDestructible.AlphaSharpness value to account for optimizations?");
		}
	}
}
#endif