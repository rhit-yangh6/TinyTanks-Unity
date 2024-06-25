using UnityEngine;
using UnityEngine.Events;
using CW.Common;

namespace Destructible2D.Examples
{
	/// /// <summary>This component reads the current destructible sprite color under your finger/mouse when you tap/click or press a key.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dTapToRead")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Tap To Read")]
	public class D2dTapToRead : MonoBehaviour
	{
		[System.Serializable] public class ColorEvent : UnityEvent<Color> {};

		[System.Serializable] public class D2dDestructibleColorEvent : UnityEvent<D2dDestructible, Color> {};

		/// <summary>The controls used to trigger the read.</summary>
		public CwInputManager.Trigger Controls = new CwInputManager.Trigger(true, true, KeyCode.None);

		/// <summary>The z position the prefab should spawn at.</summary>
		public float Intercept;

		/// <summary>When a color is read, this event will be invoked with the color.</summary>
		public ColorEvent OnColor { get { if (onColor == null) onColor = new ColorEvent(); return onColor; } } [SerializeField] private ColorEvent onColor;

		/// <summary>When a color is read, this event will be invoked with the destructible and color.</summary>
		public D2dDestructibleColorEvent OnDestructibleColor { get { if (onDestructibleColor == null) onDestructibleColor = new D2dDestructibleColorEvent(); return onDestructibleColor; } } [SerializeField] private D2dDestructibleColorEvent onDestructibleColor;

		/// <summary>When no color is read, this event will be invoked.</summary>
		public UnityEvent OnNothing { get { if (onNothing == null) onNothing = new UnityEvent(); return onNothing; } } [SerializeField] private UnityEvent onNothing;

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			// Loop through all fingers + mouse + mouse hover
			foreach (var finger in CwInputManager.GetFingers(true))
			{
				if (Controls.WentDown(finger) == true)
				{
					ReadNow(finger);
				}
			}
		}

		private void ReadNow(CwInputManager.Finger finger)
		{
			// Make sure the camera exists
			var camera = CwHelper.GetCamera(null);

			if (camera != null)
			{
				// World position of the mouse
				var position = D2dCommon.ScreenToWorldPosition(finger.ScreenPosition, Intercept, camera);

				// Read the destructible and alpha at this position
				var destructible = default(D2dDestructible);
				var alpha        = default(Color32);

				if (D2dDestructible.TrySampleAlphaAll(position, ref destructible, ref alpha) == true)
				{
					if (onColor != null)
					{
						onColor.Invoke(alpha);
					}

					if (onDestructibleColor != null)
					{
						onDestructibleColor.Invoke(destructible, alpha);
					}
				}
				else
				{
					if (onNothing != null)
					{
						onNothing.Invoke();
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
	using TARGET = D2dTapToRead;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dTapToRead_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the fracture.");
			Draw("Intercept", "The z position the prefab should spawn at.");
			Draw("onColor");
			Draw("onDestructibleColor");
			Draw("onNothing");
		}
	}
}
#endif