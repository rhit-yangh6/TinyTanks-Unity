using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component implements very basic physics for a 2D wheel in a top-down perspective.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dWheel")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Wheel")]
	public class D2dWheel : MonoBehaviour
	{
		/// <summary>The current rotational speed of the wheel.</summary>
		public float Speed;

		/// <summary>How quickly the wheel matches the ground speed.</summary>
		public float GripDamping = 1.0f;

		/// <summary>How quickly the wheels slow down.</summary>
		public float Friction = 0.1f;
		
		// Has oldPosition been set?
		[SerializeField]
		private bool oldPositionSet;

		// Stores the old position
		[SerializeField]
		private Vector2 oldPosition;

		// The rigidbody this wheel is attached to
		[System.NonSerialized]
		private Rigidbody2D cachedRigidbody2D;

		[System.NonSerialized]
		private float remainingAngularVelocity;

		public void AddTorque(float amount)
		{
			Speed += amount;
		}

		protected virtual void FixedUpdate()
		{
			if (cachedRigidbody2D == null) cachedRigidbody2D = GetComponentInParent<Rigidbody2D>();

			if (cachedRigidbody2D != null)
			{
				if (oldPositionSet == false)
				{
					oldPositionSet = true;
					oldPosition    = transform.position;
				}

				var newPosition   = (Vector2)transform.position;
				var deltaPosition = newPosition - oldPosition;
				var deltaSpeed    = deltaPosition.magnitude / Time.fixedDeltaTime;
				var expectedSpeed = deltaSpeed * Vector2.Dot(transform.up, cachedRigidbody2D.transform.up);

				oldPosition = newPosition;

				// Match ground speed
				Speed = Dampen(Speed, expectedSpeed, GripDamping, Time.fixedDeltaTime);

				// Apply speed difference
				var deltaWheel         = (Vector2)transform.up * Speed * Time.fixedDeltaTime;
				var oldAngularVelocity = cachedRigidbody2D.angularVelocity;

				cachedRigidbody2D.AddForceAtPosition(deltaWheel - deltaPosition, transform.position, ForceMode2D.Impulse);

				remainingAngularVelocity += cachedRigidbody2D.angularVelocity - oldAngularVelocity;

				cachedRigidbody2D.angularVelocity = oldAngularVelocity;

				// Slow wheel down
				Speed = Dampen(Speed, 0.0f, Friction, Time.fixedDeltaTime);
			}
		}

		protected virtual void Update()
		{
			if (cachedRigidbody2D != null)
			{
				cachedRigidbody2D.angularVelocity += remainingAngularVelocity;

				remainingAngularVelocity = 0;
			}
		}

		private static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0.0f)
		{
			var factor   = CwHelper.DampenFactor(dampening, elapsed);
			var maxDelta = Mathf.Abs(target - current) * factor + minStep * elapsed;
			
			return Mathf.MoveTowards(current, target, maxDelta);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dWheel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dWheel_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Speed", "The current rotational speed of the wheel.");
			Draw("GripDamping", "How quickly the wheel matches the ground speed.");
			Draw("Friction", "How quickly the wheels slow down.");
		}
	}
}
#endif