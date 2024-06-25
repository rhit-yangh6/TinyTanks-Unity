using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current Rigidbody2D into a spaceship that can be controlled with the <b>Horizontal</b> and <b>Vertical</b> and <b>Jump</b> input axes.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dPlayerSpaceship")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Player Spaceship")]
	public class D2dPlayerSpaceship : MonoBehaviour
	{
		/// <summary>The controls used to turn left and right.</summary>
		public CwInputManager.Axis TurnControls = new CwInputManager.Axis(1, false, CwInputManager.AxisGesture.HorizontalPull, 0.01f, KeyCode.A, KeyCode.D, KeyCode.LeftArrow, KeyCode.RightArrow, 1.0f);

		/// <summary>The controls used to accelerate and reverse.</summary>
		public CwInputManager.Axis MoveControls = new CwInputManager.Axis(1, false, CwInputManager.AxisGesture.VerticalPull, 0.01f, KeyCode.S, KeyCode.W, KeyCode.DownArrow, KeyCode.UpArrow, 1.0f);

		/// <summary>The controls used to make the spaceship shoot.</summary>
		public CwInputManager.Trigger ShootControls = new CwInputManager.Trigger(true, false, KeyCode.Space);

		/// <summary>Minimum time between each shot in seconds.</summary>
		public float ShootDelay = 0.1f;

		/// <summary>The left gun.</summary>
		public D2dGun LeftGun;

		/// <summary>The right gun.</summary>
		public D2dGun RightGun;

		/// <summary>The left thruster.</summary>
		public D2dThruster LeftThruster;
		
		/// <summary>The right thruster.</summary>
		public D2dThruster RightThruster;
		
		// Cached rigidbody of this spaceship
		[System.NonSerialized]
		private Rigidbody2D cachedRigidbody2D;

		[SerializeField]
		private int burstRemaining;
		
		// Seconds until next shot is available
		[SerializeField]
		private float cooldown;

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();

			if (cachedRigidbody2D == null) cachedRigidbody2D = GetComponent<Rigidbody2D>();
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			// Can we shoot again in this burst?
			if (burstRemaining > 0 && cooldown <= 0.0f)
			{
				cooldown        = ShootDelay;
				burstRemaining -= 1;

				// Shoot left gun?
				if (LeftGun != null && LeftGun.CanShoot == true)
				{
					LeftGun.Shoot();
				}
				// Shoot right gun?
				else if (RightGun != null && RightGun.CanShoot == true)
				{
					RightGun.Shoot();
				}
			}

			// Set thrusters based on finger drag
			var deltaX      = TurnControls.GetValue(1.0f);
			var deltaY      = MoveControls.GetValue(1.0f);
			var targetSteer = Mathf.Clamp(deltaX, -1.0f, 1.0f);
			var targetDrive = Mathf.Clamp(deltaY, -1.0f, 1.0f);

			// Begin burst fire?
			foreach (var finger in CwInputManager.GetFingers(true))
			{
				if (ShootControls.IsDown(finger) == true)
				{
					burstRemaining = 5;
				}
			}

			if (LeftThruster != null)
			{
				LeftThruster.Throttle = targetDrive + targetSteer * 0.5f;
			}

			if (RightThruster != null)
			{
				RightThruster.Throttle = targetDrive - targetSteer * 0.5f;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dPlayerSpaceship;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dPlayerSpaceship_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("TurnControls", "The controls used to turn left and right.");
			Draw("MoveControls", "The controls used to accelerate and reverse.");
			Draw("ShootControls", "The controls used to make the spaceship shoot.");

			Separator();

			BeginError(Any(tgts, t => t.ShootDelay < 0.0f));
				Draw("ShootDelay", "Minimum time between each shot in seconds.");
			EndError();
			Draw("LeftGun", "The left gun.");
			Draw("RightGun", "The right gun.");
			Draw("LeftThruster", "The left thruster.");
			Draw("RightThruster", "The right thruster.");
		}
	}
}
#endif