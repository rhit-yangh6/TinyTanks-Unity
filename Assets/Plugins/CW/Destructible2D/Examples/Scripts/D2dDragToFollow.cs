using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to drag the mouse and have the target Rigidbody2D follow it.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dDragToFollow")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Drag To Follow")]
	public class D2dDragToFollow : MonoBehaviour
	{
		/// <summary>The controls used to trigger the follow.</summary>
		public CwInputManager.Trigger Controls = new CwInputManager.Trigger { UseFinger = true, UseMouse = true };

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The Rigidbody2D that will be dragged.</summary>
		public Rigidbody2D Target;

		/// <summary>The speed the object will follow.</summary>
		public float Damping = 10.0f;

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();
		}

		protected virtual void FixedUpdate()
		{
			var fingers = CwInputManager.GetFingers(true, true);

			if (fingers.Count > 0)
			{
				// Make sure the camera exists
				var camera = CwHelper.GetCamera(null);

				if (camera != null)
				{
					// Make sure the target exists
					if (Target != null)
					{
						// Grab world position and transition there
						var center   = CwInputManager.GetAveragePosition(fingers);
						var position = D2dCommon.ScreenToWorldPosition(center, Intercept, camera);
						var factor   = CwHelper.DampenFactor(Damping, Time.fixedDeltaTime);

						Target.velocity += (Vector2)(position - Target.transform.position) * factor;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dDragToFollow;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dDragToFollow_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the follow.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
			BeginError(Any(tgts, t => t.Target == null));
				Draw("Target", "The Rigidbody2D that will be dragged.");
			EndError();
			Draw("Damping", "The speed the object will follow.");
		}
	}
}
#endif