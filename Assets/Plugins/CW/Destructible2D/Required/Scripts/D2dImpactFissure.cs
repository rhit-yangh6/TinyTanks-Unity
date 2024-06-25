using UnityEngine;
using UnityEngine.Events;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component slices a shape at the collision impact point when another object hits this destructible object.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(D2dCollisionHandler))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dImpactFissure")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Impact Fissure")]
	public class D2dImpactFissure : MonoBehaviour, UnityEngine.ISerializationCallbackReceiver
	{
		/// <summary>The collision layers you want to listen to.</summary>
		public LayerMask Mask { set { mask = value; } get { return mask; } } [SerializeField] private LayerMask mask = -1;

		/// <summary>The impact force required.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [SerializeField] private float threshold = 10.0f;

		/// <summary>This allows you to control the minimum amount of time between fissure creation in seconds.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 0.1f;

		/// <summary>If you want a prefab to spawn at the impact point, set it here.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		/// <summary>The paint type.</summary>
		public D2dDestructible.PaintType Paint { set { paint = value; } get { return paint; } } [SerializeField] private D2dDestructible.PaintType paint;

		/// <summary>The shape of the stamp.</summary>
		public D2dShape StampShape;

		/// <summary>The shape of the stamp when it modifies destructible RGB data.</summary>
		public Texture2D ColorShape { set { colorShape = value; } get { return colorShape; } } [SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("shape")] private Texture2D colorShape;

		/// <summary>The shape of the stamp when it modifies destructible alpha data.</summary>
		public Texture2D AlphaShape { set { alphaShape = value; } get { return alphaShape; } } [SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("shape")] private Texture2D alphaShape;

		/// <summary>The stamp shape will be multiplied by this. Solid White = No Change</summary>
		public Color Color { set { color = value; } get { return color; } } [SerializeField] private Color color = Color.white;

		/// <summary>This allows you to control the width of the fissure.</summary>
		public float Thickness { set { thickness = value; } get { return thickness; } } [SerializeField] private float thickness = 0.25f;

		/// <summary>This allows you to control how deep into the impact point the fissure will go.</summary>
		public float Depth { set { depth = value; } get { return depth; } } [SerializeField] private float depth = 5.0f;

		/// <summary>This allows you to move the start point of the fissure back a bit.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset = 1.0f;

		/// <summary>Use the surface normal instead of the impact velocity normal?</summary>
		public bool UseSurfaceNormal { set { useSurfaceNormal = value; } get { return useSurfaceNormal; } } [SerializeField] private bool useSurfaceNormal;

		/// <summary>This gets called when the prefab was spawned.</summary>
		public UnityEvent OnImpact { get { if (onImpact == null) onImpact = new UnityEvent(); return onImpact; } } [SerializeField] private UnityEvent onImpact;

		[System.NonSerialized]
		private D2dCollisionHandler cachedCollisionHandler;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[SerializeField]
		private float cooldown;

		protected virtual void OnEnable()
		{
			if (cachedCollisionHandler == null) cachedCollisionHandler = GetComponent<D2dCollisionHandler>();
			if (cachedDestructible     == null) cachedDestructible     = GetComponent<D2dDestructible>();

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

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			try
			{
				if (colorShape != null)
				{
					StampShape.Color = colorShape; colorShape = null;
				}

				if (alphaShape != null)
				{
					StampShape.Alpha = alphaShape; alphaShape = null;
				}
			}
			catch {}
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
						var contact = contacts[i];
						var normal  = collision.relativeVelocity;
						var force   = normal.magnitude;

						if (force >= threshold)
						{
							if (useSurfaceNormal == true)
							{
								normal = contact.normal;
							}
							else
							{
								normal /= force;
							}

							var point  = contact.point;
							var pointA = point - normal * offset;
							var pointB = point + normal * depth;
							var matrix = D2dSlice.CalculateMatrix(pointA, pointB, thickness);

							cooldown = delay;

							cachedDestructible.Paint(paint, matrix, StampShape, color);

							if (prefab != null)
							{
								Instantiate(prefab, point, Quaternion.identity);
							}

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
	using TARGET = D2dImpactFissure;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dImpactFissure_Editor : CwEditor
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
			Draw("delay", "This allows you to control the minimum amount of time between fissure creation in seconds.");

			Separator();

			Draw("paint", "The paint type.");
			Draw("StampShape", "The shape of the stamp.");
			Draw("color", "The stamp shape will be multiplied by this.\nSolid White = No Change");
			Draw("prefab", "If you want a prefab to spawn at the impact point, set it here.");
			Draw("thickness", "This allows you to control the width of the fissure.");
			Draw("depth", "This allows you to control how deep into the impact point the fissure will go.");
			Draw("offset", "This allows you to move the start point of the fissure back a bit.");
			Draw("useSurfaceNormal", "Use the surface normal instead of the impact velocity normal?");

			Separator();

			Draw("onImpact");
		}
	}
}
#endif