using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This is the base class for all collider types.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	public abstract class D2dCollider : MonoBehaviour
	{
		/// <summary>This allows you to control the <b>density</b> setting on each generated collider.</summary>
		public float Density { set { density = value; Refresh(); } get { return density; } } [SerializeField] protected float density = 1.0f;

		/// <summary>This allows you to set the <b>Density</b> value without causing <b>Refresh</b> to be called.</summary>
		public void SetDensity(float value) { density = value; }

		/// <summary>This allows you to set the <b>material</b> setting on each generated collider.</summary>
		public PhysicsMaterial2D Material { set { material = value; Refresh(); } get { return material; } } [SerializeField] protected PhysicsMaterial2D material;

		/// <summary>This allows you to set the <b>Material</b> value without causing <b>Refresh</b> to be called.</summary>
		public void SetMaterial(PhysicsMaterial2D value) { material = value; }

		/// <summary>This allows you to set the <b>isTrigger</b> setting on each generated collider.</summary>
		public bool IsTrigger { set { isTrigger = value; Refresh(); } get { return isTrigger; } } [SerializeField] protected bool isTrigger;

		/// <summary>This allows you to set the <b>IsTrigger</b> value without causing <b>Refresh</b> to be called.</summary>
		public void SetIsTrigger(bool value) { isTrigger = value; }

		/// <summary>This allows you to set the <b>usedByEffector</b> setting on each generated collider.</summary>
		public bool UsedByEffector { set { usedByEffector = value; Refresh(); } get { return usedByEffector; } } [SerializeField] protected bool usedByEffector;

		/// <summary>This allows you to set the <b>UsedByEffector</b> value without causing <b>Refresh</b> to be called.</summary>
		public void SetUsedByEffector(bool value) { usedByEffector = value; }

		/// <summary>This allows you to set the <b>usedByComposite</b> setting on each generated collider.</summary>
		public bool UsedByComposite { set { usedByComposite = value; Refresh(); } get { return usedByComposite; } } [SerializeField] protected bool usedByComposite;

		/// <summary>This allows you to set the <b>UsedByComposite</b> value without causing <b>Refresh</b> to be called.</summary>
		public void SetUsedByComposite(bool value) { usedByComposite = value; }

		[SerializeField]
		protected GameObject child;

		[System.NonSerialized]
		protected D2dDestructible cachedDestructible;

		[System.NonSerialized]
		protected bool cachedDestructibleSet;

		[SerializeField]
		protected bool awoken;

		[System.NonSerialized]
		private GameObject tempChild;

		[System.NonSerialized]
		private Rigidbody2D cachedRigidbody;

		public D2dDestructible CachedDestructible
		{
			get
			{
				if (cachedDestructibleSet == false)
				{
					cachedDestructible    = GetComponent<D2dDestructible>();
					cachedDestructibleSet = true;
				}

				return cachedDestructible;
			}
		}

		public bool UseAutoMass
		{
			get
			{
				return TryGetComponent(out cachedRigidbody) == true && cachedRigidbody.useAutoMass == true;
			}
		}

		public abstract void Refresh();

		[ContextMenu("Rebuild")]
		public void Rebuild()
		{
			if (CachedDestructible.Ready == true)
			{
				UpdateBeforeBuild();

				DoRebuild();
			}
			else
			{
				child = CwHelper.Destroy(child);
			}
		}

		public void DestroyChild()
		{
			if (child != null)
			{
				child = CwHelper.Destroy(child);
			}
		}

		protected abstract void DoModified(D2dRect rect);

		protected abstract void DoRebuild();

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();

			cachedDestructible.OnRebuilt    += HandleRebuilt;
			cachedDestructible.OnModified   += HandleModified;
			cachedDestructible.OnSplitStart += HandleSplitStart;
			cachedDestructible.OnSplitEnd   += HandleSplitEnd;

			if (child != null)
			{
				child.SetActive(true);
			}
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnRebuilt    -= HandleRebuilt;
			cachedDestructible.OnModified   -= HandleModified;
			cachedDestructible.OnSplitStart -= HandleSplitStart;
			cachedDestructible.OnSplitEnd   -= HandleSplitEnd;

			if (child != null)
			{
				child.SetActive(false);
			}

			// If the collider was disabled while splitting then run this special code to destroy the children
			if (cachedDestructible.IsOnStartSplit == true)
			{
				if (child != null)
				{
					child.transform.SetParent(null, false);

					child = CwHelper.Destroy(child);
				}

				if (tempChild != null)
				{
					tempChild = CwHelper.Destroy(tempChild);
				}
			}
		}

		protected virtual void Awake()
		{
			// Auto destroy all default collider2Ds
			if (GetComponent<Collider2D>() != null)
			{
				var collider2Ds = GetComponents<Collider2D>();

				for (var i = collider2Ds.Length - 1; i >= 0; i--)
				{
					CwHelper.Destroy(collider2Ds[i]);
				}
			}
		}

		protected virtual void Start()
		{
			if (awoken == false)
			{
				awoken = true;

				HandleRebuilt();
			}
		}

		protected virtual void Update()
		{
			if (child == null)
			{
				HandleRebuilt();
			}
		}

		protected virtual void OnDestroy()
		{
			DestroyChild();
		}

		private void HandleRebuilt()
		{
			UpdateBeforeBuild();

			DoRebuild();
		}

		private void HandleModified(D2dRect rect)
		{
			UpdateBeforeBuild();

			DoModified(rect);
		}

		protected virtual void HandleSplitStart()
		{
			if (child != null)
			{
				child.transform.SetParent(null, false);

				tempChild = child;
				child     = null;
			}
		}

		protected virtual void HandleSplitEnd(List<D2dDestructible> splitDestructibles, D2dDestructible.SplitMode mode)
		{
			ReconnectChild();
		}

		private void UpdateBeforeBuild()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();

			if (child == null)
			{
				ReconnectChild();

				if (child == null)
				{
					child = new GameObject("Collider");

					child.layer = transform.gameObject.layer;

					child.transform.SetParent(transform, false);
				}
			}

			if (cachedDestructible.Ready == true)
			{
				var w = cachedDestructible.AlphaScale.x / cachedDestructible.AlphaWidth;
				var h = cachedDestructible.AlphaScale.y / cachedDestructible.AlphaHeight;

				var offsetX = cachedDestructible.AlphaOffset.x + w * 0.5f;
				var offsetY = cachedDestructible.AlphaOffset.y + h * 0.5f;
				var scaleX  = w / 255.0f;
				var scaleY  = h / 255.0f;

				child.transform.localPosition = new Vector3(offsetX, offsetY, 0.0f);
				child.transform.localScale    = new Vector3(scaleX, scaleY, 1.0f);
			}
		}

		private void ReconnectChild()
		{
			if (tempChild != null)
			{
				child = tempChild;

				child.transform.SetParent(transform, false);

				tempChild = null;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dCollider;

	public class D2dCollider_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var refresh = false;

			if (Any(tgts, t => t.UseAutoMass == true))
			{
				Draw("density", ref refresh, "This allows you to set the density setting on each generated collider.");
			}
			Draw("material", ref refresh, "This allows you to set the sharedMaterial setting on each generated collider.");
			Draw("isTrigger", ref refresh, "This allows you to set the isTrigger setting on each generated collider.");
			Draw("usedByEffector", ref refresh, "This allows you to set the usedByEffector setting on each generated collider.");
			Draw("usedByComposite", ref refresh, "This allows you to set the usedByComposite setting on each generated collider.");

			if (refresh == true) Each(tgts, t => t.Refresh(), true);
		}
	}
}
#endif