using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component spawns a prefab when under your finger/mouse when you tap/click or press a key.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dTapToSpawn")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Tap To Spawn")]
	public class D2dTapToSpawn : MonoBehaviour
	{
		class Link : CwInputManager.Link
		{
			public GameObject Visual;
			public float      Twist;
			public float      Cooldown;

			public override void Clear()
			{
				Destroy(Visual);

				Cooldown = 0.0f;
				Visual   = null;
			}
		}

		/// <summary>The controls used to trigger the spawn.</summary>
		public CwInputManager.Trigger Controls = new CwInputManager.Trigger(true, true, KeyCode.None);

		/// <summary>The prefab used to show what the stamp will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>The time in seconds between each spawn.
		/// 0 = Once per click.</summary>
		public float Interval;

		/// <summary>The prefab that gets spawned under the mouse when clicking.</summary>
		public GameObject Prefab;

		/// <summary>The angle of the stamp in degrees.</summary>
		public float Angle;

		/// <summary>The angle will offset by this minimum random value.</summary>
		public float TwistMin = 0.0f;

		/// <summary>The angle will offset by this maximum random value.</summary>
		public float TwistMax = 360.0f;

		/// <summary>The Z position the prefab should spawn at. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The camera used to calculate the spawn point.
		/// None/null = Main Camera.</summary>
		public Camera Camera;

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
					var twist = Random.Range(TwistMin, TwistMax);

					// Show a stamp indicator, and stamp later?
					if (IndicatorPrefab != null)
					{
						var link = CwInputManager.Link.Create(ref links, finger);

						link.Visual      = Instantiate(IndicatorPrefab);
						link.Twist       = twist;

						link.Visual.SetActive(true);
					}
					// Stamp immediately?
					else
					{
						DoSpawn(finger, twist);
					}
				}
			}

			// Loop through all links in reverse so they can be removed
			for (var i = links.Count - 1; i >= 0; i--)
			{
				var link = links[i];

				// Update indicator?
				if (link.Visual != null)
				{
					link.Visual.transform.position      = GetPosition(link.Finger.ScreenPosition);
					link.Visual.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Angle + link.Twist);
				}

				if (Interval > 0.0f)
				{
					link.Cooldown -= Time.deltaTime;

					if (link.Cooldown <= 0.0f)
					{
						link.Cooldown = Interval;

						DoSpawn(link.Finger, link.Twist);

						link.Twist = Random.Range(TwistMin, TwistMax);
					}
				}

				// Did this finger go up based on the current control settings?
				if (Controls.WentUp(link.Finger, true) == true)
				{
					if (Interval <= 0.0f)
					{
						DoSpawn(link.Finger, link.Twist);
					}

					// Destroy indicator
					CwInputManager.Link.ClearAndRemove(links, link);
				}
			}
		}

		private void DoSpawn(CwInputManager.Finger finger, float twist)
		{
			// Prefab exists?
			if (Prefab != null)
			{
				// Make sure the camera exists
				var camera = CwHelper.GetCamera(null);

				if (camera != null)
				{
					// World position of the mouse
					var position = GetPosition(finger.ScreenPosition);

					// Get a random rotation around the Z axis
					var rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, Angle + twist));

					// Spawn prefab here
					var clone = Instantiate(Prefab, position, rotation);

					clone.SetActive(true);
				}
			}
		}

		private Vector3 GetPosition(Vector2 screenPosition)
		{
			// Make sure the camera exists
			var camera = CwHelper.GetCamera(Camera);

			if (camera != null)
			{
				return D2dCommon.ScreenToWorldPosition(screenPosition, Intercept, camera);
			}

			return default(Vector3);
		}

		protected virtual void OnDestroy()
		{
			CwInputManager.Link.ClearAll(links);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dTapToSpawn;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dTapToSpawn_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the spawn.");
			Draw("IndicatorPrefab", "The prefab used to show what the stamp will look like.");
			Draw("Interval", "The time in seconds between each spawn.\n\n0 = Once per click.");

			Separator();

			BeginError(Any(tgts, t => t.Prefab == null));
				Draw("Prefab", "The prefab that gets spawned under the mouse when clicking.");
			EndError();
			Draw("Angle", "The angle of the prefab in degrees.");
			BeginIndent();
				Draw("TwistMin", "The angle will offset by this minimum random value.");
				Draw("TwistMax", "The angle will offset by this maximum random value.");
			EndIndent();

			Separator();

			Draw("Intercept", "The Z position the prefab should spawn at. For normal 2D scenes this should be 0.");
			Draw("Camera", "The camera used to calculate the spawn point.\n\nNone/null = Main Camera.");
		}
	}
}
#endif