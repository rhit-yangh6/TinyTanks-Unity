using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component automatically shifts the current Transform based on the current camera position, allowing you to create an infinitely scrolling background.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dBackground")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Background")]
	public class D2dBackground : MonoBehaviour
	{
		/// <summary>The width of the original sprite in world space (width pixels / pixels per unit).</summary>
		public Vector2 Size;

		/// <summary>The distance this layer is from the main scene focal point, where positive values are in the distance, and negative values are closer to the camera.</summary>
		public float ParallaxMultiplier;

		protected virtual void Update()
		{
			UpdatePosition(Camera.main);
		}

		private void UpdatePosition(Camera camera)
		{
			if (camera != null)
			{
				// Grab current values
				var currentPosition = transform.position;
				var cameraPosition  = camera.transform.position;

				// Calculate parallax on all axes
				var parallax = cameraPosition * ParallaxMultiplier;

				// Calculate snap while factoring in horizontal parallax
				var snap = default(Vector2);

				if (Size.x > 0.0f)
				{
					snap.x = Mathf.RoundToInt((cameraPosition.x - parallax.x) / Size.x) * Size.x;
				}

				if (Size.y > 0.0f)
				{
					snap.y = Mathf.RoundToInt((cameraPosition.y - parallax.y) / Size.y) * Size.y;
				}

				// Write and update positions
				currentPosition.x = parallax.x + snap.x;
				currentPosition.y = parallax.y + snap.y;

				transform.position = currentPosition;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dBackground;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dBackground_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Size", "The width of the original sprite in world space (width pixels / pixels per unit).");
			Draw("ParallaxMultiplier", "The distance this layer is from the main scene focal point, where positive values are in the distance, and negative values are closer to the camera.");
		}
	}
}
#endif