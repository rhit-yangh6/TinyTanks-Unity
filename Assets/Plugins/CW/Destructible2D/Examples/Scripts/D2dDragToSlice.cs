using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to slice all destructible sprites between the mouse down and mouse up points.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDragToSlice")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Drag To Slice")]
	public class D2dDragToSlice : MonoBehaviour, UnityEngine.ISerializationCallbackReceiver
	{
		class Link : CwInputManager.Link
		{
			public Vector3 Start;

			public GameObject Visual;

			public override void Clear()
			{
				Destroy(Visual);
			}
		}

		/// <summary>The controls used to trigger the slice.</summary>
		public CwInputManager.Trigger Controls = new CwInputManager.Trigger { UseFinger = true, UseMouse = true };

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The destructible sprite layers we want to slice.</summary>
		public LayerMask Layers = -1;

		/// <summary>Should the stamp exclude a specific destructible object?</summary>
		public D2dDestructible Exclude;

		/// <summary>The prefab used to show what the slice will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>This allows you to change the painting type.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the stamp.</summary>
		public D2dShape StampShape;

		/// <summary>The shape of the stamp when it modifies destructible RGB data.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("Shape")]
		public Texture2D ColorShape;

		/// <summary>The shape of the stamp when it modifies destructible alpha data.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("Shape")]
		public Texture2D AlphaShape;

		/// <summary>The stamp shape will be multiplied by this.
		/// White = No Change.</summary>
		public Color Color = Color.white;

		/// <summary>The thickness of the slice line in world space.</summary>
		public float Thickness = 1.0f;

		[SerializeField]
		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			// Loop through all fingers + mouse + mouse hover
			foreach (var finger in CwInputManager.GetFingers(true))
			{
				// Did this finger go down based on the current control settings?
				if (Controls.WentDown(finger) == true)
				{
					// Create a link with this finger and additional data
					var link = CwInputManager.Link.Create(ref links, finger);

					// Create an indicator for this link?
					if (IndicatorPrefab != null)
					{
						link.Visual = Instantiate(IndicatorPrefab);

						link.Visual.SetActive(true);
					}

					link.Start = GetPosition(finger.ScreenPosition);
				}
			}

			// Loop through all links in reverse so they can be removed
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link     = links[i];
				var position = GetPosition(link.Finger.ScreenPosition);

				// Update indicator?
				if (link.Visual != null)
				{
					var scale = Vector3.Distance(position, link.Start);
					var angle = CwHelper.Atan2(position - link.Start) * Mathf.Rad2Deg;

					link.Visual.transform.position   = link.Start;
					link.Visual.transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
					link.Visual.transform.localScale = new Vector3(Thickness, scale, scale);
				}

				// Did this finger go up based on the current control settings?
				if (Controls.WentUp(link.Finger, true) == true)
				{
					// Slice all objects in scene
					D2dSlice.All(Paint, link.Start, position, Thickness, StampShape, Color, Layers, Exclude);

					// Destroy indicator
					CwInputManager.Link.ClearAndRemove(links, link);
				}
			}
		}

		protected virtual void OnDestroy()
		{
			CwInputManager.Link.ClearAll(links);
		}

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

		private Vector3 GetPosition(Vector2 screenPosition)
		{
			// Make sure the camera exists
			var camera = CwHelper.GetCamera(null);

			if (camera != null)
			{
				return D2dCommon.ScreenToWorldPosition(screenPosition, Intercept, camera);
			}

			return default(Vector3);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dDragToSlice;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDragToSlice_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the stamp.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
			BeginError(Any(tgts, t => t.Layers == 0));
				Draw("Layers", "The destructible sprite layers we want to slice.");
			EndError();
			Draw("Exclude", "Should the stamp exclude a specific destructible object?");
			BeginError(Any(tgts, t => t.IndicatorPrefab == null));
				Draw("IndicatorPrefab", "The prefab used to show what the slice will look like.");
			EndError();

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			Draw("StampShape", "The shape of the stamp.");
			Draw("Color", "The stamp shape will be multiplied by this.\n\nWhite = No Change.");
			BeginError(Any(tgts, t => t.Thickness == 0.0f));
				Draw("Thickness", "The thickness of the slice line in world space.");
			EndError();
		}
	}
}
#endif