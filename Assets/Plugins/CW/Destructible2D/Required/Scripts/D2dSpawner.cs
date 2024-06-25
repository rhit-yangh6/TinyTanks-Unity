using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows you to spawn the specified prefab by manually calling the Spawn method.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSpawner")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Spawner")]
	public class D2dSpawner : MonoBehaviour
	{
		/// <summary>This allows you to control the minimum amount of time between prefab creation in seconds.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 1.0f;

		/// <summary>This allows you to control the amount of seconds between spawns.
		/// -1 = Spawn once only.</summary>
		public float Interval { set { interval = value; } get { return interval; } } [SerializeField] private float interval = 1.0f;

		/// <summary>The amount of extra times this component can spawn.
		/// -1 = Unlimited.</summary>
		public int Remaining { set { remaining = value; } get { return remaining; } } [SerializeField] private int remaining = 1;

		/// <summary>This allows you to control how far the spawned object can randomly rotate from its initial rotation.</summary>
		public float Spread { set { spread = value; } get { return spread; } } [SerializeField] private float spread;

		/// <summary>This allows you to specify which direction is forward for your sprite when applying the speed.</summary>
		public Vector2 Forward { set { forward = value; } get { return forward; } } [SerializeField] private Vector2 forward = Vector2.up;

		/// <summary>This allows you to set the minimum random speed applied to the spawned object if it has a Rigidbody2D.</summary>
		public float SpeedMin { set { speedMin = value; } get { return speedMin; } } [SerializeField] private float speedMin;

		/// <summary>This allows you to set the maximum random speed applied to the spawned object if it has a Rigidbody2D.</summary>
		public float SpeedMax { set { speedMax = value; } get { return speedMax; } } [SerializeField] private float speedMax;

		/// <summary>If you want a prefab to spawn at the impact point, set it here.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		protected virtual void Update()
		{
			if (remaining != 0)
			{
				delay -= Time.deltaTime;

				if (delay <= 0.0f)
				{
					delay = interval;

					if (remaining > 0)
					{
						remaining -= 1;
					}

					Spawn();
				}
			}
		}

		[ContextMenu("Spawn")]
		public void Spawn()
		{
			if (prefab != null)
			{
				var rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(-1.0f, 1.0f) * spread) * transform.rotation;
				var clone    = Instantiate(prefab, transform.position, rotation);
				var body     = clone.GetComponent<Rigidbody2D>();

				clone.SetActive(true);

				if (body != null)
				{
					body.velocity = rotation * forward.normalized * Random.Range(speedMin, speedMax);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dSpawner;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSpawner_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("delay", "This allows you to control the minimum amount of time between prefab creation in seconds.");
			Draw("interval", "This allows you to control the amount of seconds between spawns.\n\n-1 = Spawn once only.");
			Draw("remaining", "The amount of extra times this component can spawn.\n\n-1 = Unlimited.");

			Separator();

			Draw("spread", "This allows you to control how far the spawned object can randomly rotate from its initial rotation.");
			Draw("forward", "This allows you to specify which direction is forward for your sprite when applying the speed.");
			Draw("speedMin", "This allows you to set the minimum random speed applied to the spawned object if it has a Rigidbody2D.");
			Draw("speedMax", "This allows you to set the maximum random speed applied to the spawned object if it has a Rigidbody2D.");

			Separator();

			BeginError(Any(tgts, t => t.Prefab == null));
				Draw("prefab", "If you want a prefab to spawn at the impact point, set it here.");
			EndError();
		}
	}
}
#endif