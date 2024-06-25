using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component spawns the specified prefab, and respawns it if it's been destroyed.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dFixedSpawner")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Fixed Spawner")]
	public class D2dFixedSpawner : MonoBehaviour
	{
		/// <summary>The prefab that will be spawned.</summary>
		public GameObject Prefab;

		/// <summary>The delay in seconds between the spawned object being deleted, and a new clone being spawned.</summary>
		public float RespawnDelay = 1.0f;

		[SerializeField]
		private GameObject clone;

		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			if (clone == null)
			{
				cooldown -= Time.deltaTime;

				if (cooldown <= 0.0f)
				{
					if (Prefab != null)
					{
						cooldown = RespawnDelay;
						clone    = Instantiate(Prefab, transform.position, transform.rotation);

						clone.SetActive(true);
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
	using TARGET = D2dFixedSpawner;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dFixedSpawner_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Prefab", "The prefab that will be spawned.");
			Draw("RespawnDelay", "The delay in seconds between the spawned object being deleted, and a new clone being spawned.");
		}
	}
}
#endif