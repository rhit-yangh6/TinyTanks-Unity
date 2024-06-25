using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current Rigidbody2D into a car that can be controlled with the <b>Horizontal</b> and <b>Vertical</b> input axes.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dPlayerCar")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Player Car")]
	public class D2dPlayerCar : MonoBehaviour
	{
		/// <summary>The fingers/keys used to turn left and right.</summary>
		public CwInputManager.Axis TurnControls = new CwInputManager.Axis(1, false, CwInputManager.AxisGesture.HorizontalPull, 0.01f, KeyCode.A, KeyCode.D, KeyCode.LeftArrow, KeyCode.RightArrow, 1.0f);

		/// <summary>The fingers/keys used to accelerate and reverse.</summary>
		public CwInputManager.Axis MoveControls = new CwInputManager.Axis(1, false, CwInputManager.AxisGesture.VerticalPull, 0.01f, KeyCode.S, KeyCode.W, KeyCode.DownArrow, KeyCode.UpArrow, 1.0f);

		/// <summary>The wheels used to steer this car.</summary>
		public D2dWheel[] SteerWheels;

		/// <summary>The maximum +- angle of turning.</summary>
		public float SteerAngleMax = 20.0f;

		/// <summary>How quickly the steering wheels turn to their target angle.</summary>
		public float SteerAngleDamping = 5.0f;

		/// <summary>The wheels used to move this car.</summary>
		public D2dWheel[] DriveWheels;

		/// <summary>The maximum torque that can be applied to each drive wheel.</summary>
		public float DriveTorque = 1.0f;

		/// <summary>How quickly the drive wheels get to their target torque.</summary>
		public float DriveDamping = 5.0f;

		// Current steering angle
		[SerializeField]
		private float currentSteer;

		[SerializeField]
		private float currentDrive;

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();
		}

		protected virtual void Update()
		{
			// Calculate control values from fingers/mouse
			var deltaX      = TurnControls.GetValue(1.0f);
			var deltaY      = MoveControls.GetValue(1.0f);
			var targetSteer = Mathf.Clamp(deltaX, -1.0f, 1.0f) * SteerAngleMax;
			var targetDrive = Mathf.Clamp(deltaY, -1.0f, 1.0f) * DriveTorque;

			// Smooth to target values
			var steerFactor = CwHelper.DampenFactor(SteerAngleDamping, Time.deltaTime);
			var driveFactor = CwHelper.DampenFactor(     DriveDamping, Time.deltaTime);

			currentSteer = Mathf.Lerp(currentSteer, targetSteer, steerFactor);
			currentDrive = Mathf.Lerp(currentDrive, targetDrive, driveFactor);

			// Apply steering
			for (var i = 0; i < SteerWheels.Length; i++)
			{
				SteerWheels[i].transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -currentSteer);
			}
		}

		protected virtual void FixedUpdate()
		{
			// Apply drive
			for (var i = 0; i < DriveWheels.Length; i++)
			{
				DriveWheels[i].AddTorque(currentDrive * Time.fixedDeltaTime);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dPlayerCar;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dPlayerCar_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("TurnControls", "The fingers/keys used to turn left and right.");
			Draw("MoveControls", "The fingers/keys used to accelerate and reverse.");

			Separator();

			Draw("SteerWheels", "The wheels used to steer this car.");
			Draw("SteerAngleMax", "The maximum +- angle of turning.");
			Draw("SteerAngleDamping", "How quickly the steering wheels turn to their target angle.");

			Separator();

			Draw("DriveWheels", "The wheels used to move this car.");
			Draw("DriveTorque", "The maximum torque that can be applied to each drive wheel.");
			Draw("DriveDamping", "How quickly the drive wheels get to their target torque.");
		}
	}
}
#endif