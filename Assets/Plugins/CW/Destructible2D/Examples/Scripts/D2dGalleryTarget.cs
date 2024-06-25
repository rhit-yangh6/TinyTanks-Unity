using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to create moving targets that randomly flip around to become indestructible.</summary>
	[ExecuteInEditMode]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dGalleryTarget")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Gallery Target")]
	public class D2dGalleryTarget : MonoBehaviour
	{
		/// <summary>Is the target facing forward?</summary>
		public bool FrontShowing;

		/// <summary>How fast the target can flip sides.</summary>
		public float FlipSpeed = 10.0f;

		/// <summary>The minimum time the target can face forward in seconds.</summary>
		public float FrontTimeMin = 1.0f;

		/// <summary>The maximum time the target can face forward in seconds.</summary>
		public float FrontTimeMax = 2.0f;

		/// <summary>The minimum time the target can be hidden in seconds.</summary>
		public float BackTimeMin = 1.0f;

		/// <summary>The maximum time the target can be hidden in seconds.</summary>
		public float BackTimeMax = 10.0f;

		/// <summary>The start position of the target in local space.</summary>
		public Vector3 StartPosition;

		/// <summary>The end position of the target in local space.</summary>
		public Vector3 EndPosition;

		/// <summary>The current movement progress in local space.</summary>
		public float MoveProgress;

		/// <summary>The maximum speed this target can move in local space.</summary>
		public float MoveSpeed;

		/// <summary>The destructible of this target.</summary>
		public D2dDestructible Destructible;

		// Seconds until a flip
		private float cooldown;

		// Current angle of the flipping
		private float angle;

		protected virtual void Awake()
		{
			ResetCooldown();
		}

		protected virtual void Update()
		{
			// Update flipping if the game is playing
			if (Application.isPlaying == true)
			{
				cooldown -= Time.deltaTime;

				// Flip?
				if (cooldown <= 0.0f)
				{
					FrontShowing = !FrontShowing;

					ResetCooldown();
				}
			}

			// Get target angle based on flip state
			var targetAngle = FrontShowing == true ? 0.0f : 180.0f;

			// Slowly rotate to the target angle if the game is playing
			if (Application.isPlaying == true)
			{
				var factor = CwHelper.DampenFactor(FlipSpeed, Time.deltaTime);

				angle = Mathf.Lerp(angle, targetAngle, factor);
			}
			// Instantly rotate if it's not
			else
			{
				angle = targetAngle;
			}

			transform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);

			// Make the destructible indestructible if it's past 90 degrees
			if (Destructible != null)
			{
				Destructible.Indestructible = targetAngle >= 90.0f;
			}

			// Update movement
			if (Application.isPlaying == true)
			{
				MoveProgress += MoveSpeed * Time.deltaTime;
			}

			var moveDistance = (EndPosition - StartPosition).magnitude;

			if (moveDistance > 0.0f)
			{
				var progress01 = Mathf.PingPong(MoveProgress / moveDistance, 1.0f);

				transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, Mathf.SmoothStep(0.0f, 1.0f, progress01));
			}
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (transform.parent != null)
			{
				Gizmos.matrix = transform.parent.localToWorldMatrix;
			}

			Gizmos.DrawLine(StartPosition, EndPosition);
		}

		private void ResetCooldown()
		{
			if (FrontShowing == true)
			{
				cooldown = Random.Range(FrontTimeMin, FrontTimeMax);
			}
			else
			{
				cooldown = Random.Range(BackTimeMin, BackTimeMax);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dGalleryTarget;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dMovingTarget_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("FrontShowing", "Is the target facing forward?");
			Draw("FlipSpeed", "How fast the target can flip sides.");
			BeginError(Any(tgts, t => t.FrontTimeMin < 0.0f || (t.FrontTimeMin > t.FrontTimeMax)));
				Draw("FrontTimeMin", "The minimum time the target can face forward in seconds.");
				Draw("FrontTimeMax", "The maximum time the target can face forward in seconds.");
			EndError();
			BeginError(Any(tgts, t => t.BackTimeMin < 0.0f || (t.BackTimeMin > t.BackTimeMax)));
				Draw("BackTimeMin", "The minimum time the target can be hidden in seconds.");
				Draw("BackTimeMax", "The maximum time the target can be hidden in seconds.");
			EndError();
			BeginError(Any(tgts, t => t.StartPosition == t.EndPosition));
				Draw("StartPosition", "The start position of the target in local space.");
				Draw("EndPosition", "The end position of the target in local space.");
			EndError();
			Draw("MoveProgress", "The current movement progress in local space.");
			Draw("MoveSpeed", "The maximum speed this target can move in local space.");
			BeginError(Any(tgts, t => t.Destructible == null));
				Draw("Destructible", "The destructible of this target.");
			EndError();
		}
	}
}
#endif