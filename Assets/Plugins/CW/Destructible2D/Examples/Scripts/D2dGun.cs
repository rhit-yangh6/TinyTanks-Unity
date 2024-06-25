using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component implements a basic 2D gun that spawns a prefab where you click on the screen.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dGun")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Gun")]
	public class D2dGun : MonoBehaviour
	{
		/// <summary>Minimum time between each shot in seconds.</summary>
		public float ShootDelay = 0.1f;

		/// <summary>The bullet prefab spawned when shooting.</summary>
		public GameObject BulletPrefab;

		/// <summary>The muzzle prefab spawned on the gun when shooting.</summary>
		public GameObject MuzzleFlashPrefab;

		// Seconds until next shot is available
		[SerializeField]
		private float cooldown;

		public bool CanShoot
		{
			get
			{
				return cooldown <= 0.0f;
			}
		}

		public void Shoot()
		{
			if (cooldown <= 0.0f)
			{
				cooldown = ShootDelay;

				if (BulletPrefab != null)
				{
					Instantiate(BulletPrefab, transform.position, transform.rotation);
				}

				if (MuzzleFlashPrefab != null)
				{
					Instantiate(MuzzleFlashPrefab, transform.position, transform.rotation);
				}
			}
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dGun;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dGun_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.ShootDelay < 0.0f));
				Draw("ShootDelay", "Minimum time between each shot in seconds.");
			EndError();
			Draw("BulletPrefab", "The bullet prefab spawned when shooting.");
			Draw("MuzzleFlashPrefab", "The muzzle prefab spawned on the gun when shooting.");
		}
	}
}
#endif