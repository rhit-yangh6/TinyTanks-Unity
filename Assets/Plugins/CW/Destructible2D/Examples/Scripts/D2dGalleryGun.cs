using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component puts the gallery gun sprite at the bottom of the screen and moves it based on the mouse position.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dGalleryGun")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Gallery Gun")]
	public class D2dGalleryGun : MonoBehaviour
	{
		/// <summary>The controls used to fire the gun.</summary>
		public CwInputManager.Trigger ShootControls = new CwInputManager.Trigger(true, true, KeyCode.None);

		/// <summary>How much the finger/mouse position relates to the gun position.</summary>
		public float MoveScale = 0.25f;

		/// <summary>How quickly the gun moves to its target position.</summary>
		public float MoveSpeed = 5.0f;

		/// <summary>The prefab spawned at the muzzle of the gun when shooting.</summary>
		public GameObject MuzzlePrefab;

		/// <summary>The prefab spawned at the mouse position when shooting.</summary>
		public GameObject BulletPrefab;

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			// Get all fingers + mouse + mouse hover
			var fingers = CwInputManager.GetFingers(true);

			// Loop through them
			foreach (var finger in fingers)
			{
				if (ShootControls.WentDown(finger) == true)
				{
					// Make sure the camera exists
					var camera = CwHelper.GetCamera(null);

					if (camera != null)
					{
						if (MuzzlePrefab != null)
						{
							Instantiate(MuzzlePrefab, transform.position, Quaternion.identity).SetActive(true);
						}

						if (BulletPrefab != null)
						{
							var position = camera.ScreenToWorldPoint(finger.ScreenPosition);

							Instantiate(BulletPrefab, position, Quaternion.identity).SetActive(true);
						}
					}
				}
			}

			// Move gun based on finger or mouse position
			if (fingers.Count > 0)
			{
				var finger        = fingers[0];
				var localPosition = transform.localPosition;
				var targetX       = (finger.ScreenPosition.x - Screen.width  / 2) * MoveScale;
				var targetY       = (finger.ScreenPosition.y - Screen.height / 2) * MoveScale;
				var factor        = CwHelper.DampenFactor(MoveSpeed, Time.deltaTime);

				localPosition.x = Mathf.Lerp(localPosition.x, targetX, factor);
				localPosition.y = Mathf.Lerp(localPosition.y, targetY, factor);

				transform.localPosition = localPosition;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dGalleryGun;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dGalleryGun_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("ShootControls", "The controls used to fire the gun.");

			Separator();

			Draw("MoveScale", "How much the finger/mouse position relates to the gun position.");
			Draw("MoveSpeed", "How quickly the gun moves to its target position.");
			Draw("MuzzlePrefab", "The prefab spawned at the muzzle of the gun when shooting.");
			Draw("BulletPrefab", "The prefab spawned at the mouse position when shooting.");
		}
	}
}
#endif