using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component stores numerical damage for the current GameObject. This damage can then be used to swap the sprite to show different damage states.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDamage")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Damage")]
	public class D2dDamage : MonoBehaviour
	{
		[System.Serializable]
		public class State
		{
			public float  DamageMin;
			public Sprite Sprite;
			public float  DamageMax;
		}

		/// <summary>This is invoked when the damage field is modified.</summary>
		public event System.Action OnDamageChanged;

		/// <summary>This tells you how much numerical damage this sprite has taken. This is automatically increased by nearby explosions and such.</summary>
		public float Damage { set { if (damage != value) { damage = value; InvokeDamageChanged(); } } get { return damage; } } [SerializeField] private float damage;

		/// <summary>The incoming damage must be at least this value to change this component's damage value.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [SerializeField] private float threshold;

		/// <summary>This allows you to reduce or increase the rate at which damage changes.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier = 1.0f;

		/// <summary>This allows you to modify the damage value directly without invoking NotifyDamageChanged/OnDamageChanged.</summary>
		public List<State> States { set { states = value; } get { if (states == null) states = new List<State>(); return states; } } [SerializeField] private List<State> states;

		[System.NonSerialized]
		private SpriteRenderer cachedSpriteRenderer;

		/// <summary>This method allows you to add to the damage value.
		/// NOTE: The <b>Multiplier</b> and <b>Threshold</b> values will be taken into account.</summary>
		public void Add(float value)
		{
			if (value > threshold)
			{
				Damage += value * multiplier;
			}
		}

		/// <summary>Call this if you manually modified the damage value.</summary>
		public void InvokeDamageChanged()
		{
			if (OnDamageChanged != null)
			{
				OnDamageChanged();
			}
		}

		protected virtual void OnEnable()
		{
			if (cachedSpriteRenderer == null) cachedSpriteRenderer = GetComponent<SpriteRenderer>();
		}

		protected virtual void Update()
		{
			var bestSprite = default(Sprite);

			if (states != null)
			{
				for (var i = states.Count - 1; i >= 0; i--)
				{
					var state = states[i];

					if (state != null && Damage >= state.DamageMin && Damage < state.DamageMax)
					{
						bestSprite = state.Sprite;
					}
				}
			}

			if (bestSprite != null)
			{
				cachedSpriteRenderer.sprite = bestSprite;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dDamage;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDamage_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var damageChanged = false;

			Draw("damage", ref damageChanged, "This tells you how much numerical damage this sprite has taken. This is automatically increased by nearby explosions and such. NOTE: This is separate to the visual damage.");
			Draw("threshold", "The incoming damage must be at least this value to change this component's damage value.");
			Draw("multiplier", "This allows you to reduce or increase the rate at which damage changes.");

			Separator();

			Draw("states", "This allows you to modify the damage value directly without invoking NotifyDamageChanged/OnDamageChanged.");

			if (damageChanged == true)
			{
				Each(tgts, t => t.InvokeDamageChanged(), true);
			}
		}
	}
}
#endif