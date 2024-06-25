using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to slice all destructible sprites between the mouse down and mouse up points.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDragToStamp")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Drag To Stamp")]
	public class D2dDragToStamp : MonoBehaviour, UnityEngine.ISerializationCallbackReceiver
	{
		/// <summary>The controls used to trigger the slice.</summary>
		public CwInputManager.Trigger Controls = new CwInputManager.Trigger { UseFinger = true, UseMouse = true };

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The destructible sprite layers we want to slice.</summary>
		public LayerMask Layers = -1;

		/// <summary>Should the stamp exclude a specific destructible object?</summary>
		public D2dDestructible Exclude;

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

		/// <summary>Stretch the start and end points out to eliminate gaps?</summary>
		public float Extend;

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			// Loop through all fingers + mouse + mouse hover
			foreach (var finger in CwInputManager.GetFingers(true))
			{
				if (Controls.IsDown(finger) == true)
				{
					var positionOld = GetPosition(finger.ScreenPositionOld);
					var positionNew = GetPosition(finger.ScreenPosition);

					if (positionOld != positionNew)
					{
						var positionMid = (positionOld + positionNew) * 0.5f;
						var positionVec = positionNew - positionOld;
						var positionMag = Vector3.Distance(positionOld, positionNew) * 0.5f;

						positionOld = positionMid - positionVec * (positionMag + Extend);
						positionNew = positionMid + positionVec * (positionMag + Extend);

						D2dSlice.All(Paint, positionOld, positionNew, Thickness, StampShape, Color, Layers, Exclude);
					}
				}
			}
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
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dDragToStamp;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDragToStamp_Editor : CwEditor
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

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			Draw("StampShape", "The shape of the stamp.");
			Draw("Color", "The stamp shape will be multiplied by this.\n\nWhite = No Change.");
			BeginError(Any(tgts, t => t.Thickness == 0.0f));
				Draw("Thickness", "The thickness of the slice line in world space.");
			EndError();
			Draw("Extend", "Stretch the start and end points out to eliminate gaps?");
		}
	}
}
#endif