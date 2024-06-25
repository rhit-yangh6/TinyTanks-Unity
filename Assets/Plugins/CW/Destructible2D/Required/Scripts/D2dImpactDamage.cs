using UnityEngine;
using UnityEngine.Events;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component increments the attached D2dDamage.Damage value when other objects hit this.</summary>
	[RequireComponent(typeof(D2dDamage))]
	[RequireComponent(typeof(D2dCollisionHandler))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dImpactDamage")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Impact Damage")]
	public class D2dImpactDamage : MonoBehaviour
	{
		/// <summary>The collision layers you want to listen to.</summary>
		public LayerMask Mask { set { mask = value; } get { return mask; } } [SerializeField] private LayerMask mask = -1;

		/// <summary>The impact force required.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [SerializeField] private float threshold = 10.0f;

		/// <summary>This allows you to control the amount of damage inflicted relative to the force of the impact.</summary>
		public float Scale { set { scale = value; } get { return scale; } } [SerializeField] private float scale = 1.0f;

		/// <summary>This allows you to control the minimum amount of time between fissure creation in seconds.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.1f;

		/// <summary>This gets called when the prefab was spawned.</summary>
		public UnityEvent OnImpact { get { if (onImpact == null) onImpact = new UnityEvent(); return onImpact; } } [SerializeField] private UnityEvent onImpact;

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

			cachedCollisionHandler.OnCollision += Collision;
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}

		protected virtual void OnDisable()
		{
			cachedCollisionHandler.OnCollision -= Collision;
		}

		private void Collision(Collision2D collision)
		{
			if (cooldown <= 0.0f)
			{
				if (CwHelper.IndexInMask(collision.gameObject.layer, mask) == true)
				{
					var contacts = collision.contacts;

					for (var i = contacts.Length - 1; i >= 0; i--)
					{
						var normal = collision.relativeVelocity;
						var force  = normal.magnitude;

						if (force >= threshold)
						{
							cooldown = delay;

							cachedDamage.Damage += force * scale;

							if (onImpact != null)
							{
								onImpact.Invoke();
							}

							if (delay > 0.0f)
							{
								break;
							}
						}
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
	using TARGET = D2dImpactDamage;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dImpactDamage_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Mask == 0));
				Draw("mask", "The collision layers you want to listen to.");
			EndError();
			BeginError(Any(tgts, t => t.Threshold < 0.0f));
				Draw("threshold", "The impact force required.");
			EndError();
			Draw("scale", "This allows you to control the amount of damage inflicted relative to the force of the impact.");
			Draw("delay", "This allows you to control the minimum amount of time between damage infliction in seconds.");

			Separator();

			Draw("onImpact");
		}
	}
}
#endif