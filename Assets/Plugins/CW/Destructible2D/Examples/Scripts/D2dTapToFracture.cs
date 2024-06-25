using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to fracture a destructible sprite under the mouse.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dTapToFracture")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Tap To Fracture")]
	public class D2dTapToFracture : MonoBehaviour
	{
		public enum HitType
		{
			All,
			First
		}

		/// <summary>The controls used to trigger the fracture.</summary>
		public CwInputManager.Trigger Controls = new CwInputManager.Trigger(true, true, KeyCode.None);

		/// <summary>The destructible sprite layers we can click.</summary>
		public LayerMask Layers = -1;

		/// <summary>How many destructibles should be hit?</summary>
		public HitType Hit;

		/// <summary>The camera used to calculate the ray.
		/// None = MainCamera.</summary>
		public Camera Camera;

		/// <summary>The prefab that gets spawned under the mouse when clicking.</summary>
		public GameObject Prefab;

		/// <summary>Only fracture GameObjects that have the D2dFracturer component?</summary>
		public bool RequireFracturer = true;

		/// <summary>This lets you set how many fracture points there can be based on the amount of solid pixels.</summary>
		public float PointsPerSolidPixel = 0.001f;

		/// <summary>This lets you limit how many points the fracture can use.</summary>
		public int MaxPoints = 10;

		/// <summary>Automatically multiply the points by the D2dDestructible.AlphaSharpness value to account for optimizations?</summary>
		public bool FactorInSharpness = true;

		/// <summary>Fracturing can cause pixel islands to appear, should a split be triggered on each fractured part to check for these?</summary>
		public bool SplitAfterFracture;

		/// <summary>This allows you to set the Feather value used when splitting.</summary>
		public int SplitFeather = 3;

		/// <summary>This allows you to set the HealThreshold value used when splitting.</summary>
		public int SplitHealThreshold = -1;

		private static RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1024];

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
					var camera = CwHelper.GetCamera(Camera, gameObject);

					if (camera != null)
					{
						var ray   = camera.ScreenPointToRay(finger.ScreenPosition);
						var count = Physics2D.GetRayIntersectionNonAlloc(ray, raycastHit2Ds, float.PositiveInfinity, Layers);

						if (count > 0)
						{
							for (var i = 0; i < count; i++)
							{
								var raycastHit2D = raycastHit2Ds[i];
							
								if (TryFracture(raycastHit2D) == true)
								{
									// Spawn prefab?
									if (Prefab != null)
									{
										var clone = Instantiate(Prefab, raycastHit2D.point, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

										clone.SetActive(true);
									}

									if (Hit == HitType.First)
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

		private bool TryFracture(RaycastHit2D hit)
		{
			if (RequireFracturer == true)
			{
				var fracturer = hit.transform.GetComponentInParent<D2dFracturer>();

				if (fracturer != null && fracturer.enabled == true && fracturer.TryFracture() == true)
				{
					return true;
				}
			}
			else
			{
				var destructible = hit.transform.GetComponentInParent<D2dDestructible>();

				if (destructible != null)
				{
					var points = D2dFracturer.CalculatePointCount(destructible, PointsPerSolidPixel, FactorInSharpness, MaxPoints);

					if (D2dFracturer.TryFracture(destructible, points, SplitAfterFracture, SplitFeather, SplitHealThreshold) == true)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dTapToFracture;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dTapToFracture_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Controls", "The controls used to trigger the fracture.");
			BeginError(Any(tgts, t => t.Layers == 0));
				Draw("Layers", "The destructible sprite layers we can click.");
			EndError();
			Draw("Hit", "How many destructibles should be hit?");
			Draw("Camera", "The camera used to calculate the ray.\n\nNone = MainCamera.");
			Draw("Prefab", "The prefab that gets spawned under the mouse when clicking.");

			Separator();

			Draw("RequireFracturer", "Only fracture GameObjects that have the D2dFracturer component?");

			if (Any(tgts, t => t.RequireFracturer == false))
			{
				Draw("PointsPerSolidPixel", "This lets you set how many fracture points there can be based on the amount of solid pixels.");
				Draw("MaxPoints", "This lets you limit how many points the fracture can use.");
				Draw("FactorInSharpness", "Automatically multiply the points by the D2dDestructible.AlphaSharpness value to account for optimizations?");
				Draw("SplitAfterFracture", "Fracturing can cause pixel islands to appear, should a split be triggered on each fractured part to check for these?");

				if (Any(tgts, t => t.SplitAfterFracture == true))
				{
					BeginIndent();
						Draw("SplitFeather", "This allows you to set the Feather value used when splitting.");
						Draw("SplitHealThreshold", "This allows you to set the HealThreshold value used when splitting.");
					EndIndent();
				}
			}
		}
	}
}
#endif