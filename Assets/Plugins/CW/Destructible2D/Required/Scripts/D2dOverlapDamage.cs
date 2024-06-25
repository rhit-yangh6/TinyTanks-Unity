using UnityEngine;
using UnityEngine.Events;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component increments the attached D2dDamage.Damage value when this object overlaps a trigger.</summary>
	[RequireComponent(typeof(D2dDamage))]
	[RequireComponent(typeof(D2dCollisionHandler))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dOverlapDamage")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Overlap Damage")]
	public class D2dOverlapDamage : MonoBehaviour
	{
		/// <summary>The collision layers you want to listen to.</summary>
		public LayerMask Mask { set { mask = value; } get { return mask; } } [SerializeField] private LayerMask mask = -1;

		/// <summary>This allows you to control the amount of damage inflicted per second of overlap.</summary>
		public float DamagePerSecond { set { damagePerSecond = value; } get { return damagePerSecond; } } [SerializeField] private float damagePerSecond = 1.0f;

		/// <summary>This allows you to control the minimum amount of time between damage in seconds.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.1f;

		/// <summary>This gets called when the prefab was spawned.</summary>
		public UnityEvent OnOverlap { get { if (onOverlap == null) onOverlap = new UnityEvent(); return onOverlap; } } [SerializeField] private UnityEvent onOverlap;

		[System.NonSerialized]
		private D2dCollisionHandler cachedCollisionHandler;

		[System.NonSerialized]
		private D2dDamage cachedDamage;

		[SerializeField]
		private float cooldown;

		protected virtual void OnEnable()
		{
			if (cachedCollisionHandler == null) cachedCollisionHandler = GetComponent<D2dCollisionHandler>();
			if (cachedDamage           == null) cachedDamage           = GetComponent<D2dDamage>();

			cachedCollisionHandler.OnOverlap += Overlap;
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}

		protected virtual void OnDisable()
		{
			cachedCollisionHandler.OnOverlap -= Overlap;
		}

		private void Overlap(Collider2D collider)
		{
			if (CwHelper.IndexInMask(collider.gameObject.layer, mask) == true)
			{
				cachedDamage.Damage += damagePerSecond * Time.deltaTime;

				if (cooldown <= 0.0f)
				{
					cooldown = delay;

					if (onOverlap != null)
					{
						onOverlap.Invoke();
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dOverlapDamage;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dOverlapDamage_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Mask == 0));
				Draw("mask", "The collision layers you want to listen to.");
			EndError();
			Draw("damagePerSecond", "This allows you to control the amount of damage inflicted relative to the force of the impact.");
			Draw("delay", "This allows you to control the minimum amount of time between damage in seconds");

			Separator();

			Draw("onOverlap");
		}
	}
}
#endif