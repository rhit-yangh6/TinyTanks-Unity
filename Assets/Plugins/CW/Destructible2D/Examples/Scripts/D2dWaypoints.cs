using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component automatically moves the current GameObject between waypoints</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dWaypoints")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Waypoints")]
	public class D2dWaypoints : MonoBehaviour
	{
		/// <summary>The rate at which this GameObject accelerates toward its current target.</summary>
		public float Acceleration = 5.0f;

		/// <summary>The maximum speed at which this GameObject can move toward its current target.</summary>
		public float MaximumSpeed = 2.0f;

		/// <summary>The extra acceleration given to stop this gameObject orbiting its target.</summary>
		public float SpeedBoost = 2.0f;

		/// <summary>If this gameObject gets within this distance of its current target then it will switch target.</summary>
		public float MinimumDistance = 1.0f;

		/// <summary>All the point positions will be offset by this.</summary>
		public Vector2 PointOffset;

		/// <summary>The points this GameObject will randomly move between.</summary>
		public Vector2[] Points;

		[SerializeField]
		private Vector2 targetPoint;

		[System.NonSerialized]
		private Rigidbody2D body;

		protected virtual void Awake()
		{
			ChangeTargetPoint();
        }

		protected virtual void FixedUpdate()
		{
			var currentPoint = (Vector2)transform.position;
			var vector       = targetPoint - currentPoint;

			if (vector.magnitude <= MinimumDistance)
			{
				ChangeTargetPoint();

				vector = targetPoint - currentPoint;
			}

			// Limit target speed
			if (vector.magnitude > MaximumSpeed)
			{
				vector = vector.normalized * MaximumSpeed;
			}

			// Acceleration
			if (body == null) body = GetComponent<Rigidbody2D>();

			var factor = CwHelper.DampenFactor(Acceleration, Time.deltaTime);

			body.velocity = Vector2.Lerp(body.velocity, vector * SpeedBoost, factor);
		}

		private void ChangeTargetPoint()
		{
			if (Points != null && Points.Length > 0)
			{
				var newIndex = Random.Range(0, Points.Length);

				targetPoint = Points[newIndex] + PointOffset;
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (Points != null)
			{
				foreach (var point in Points)
				{
					Gizmos.DrawLine(point + PointOffset, transform.position);
				}
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dWaypoints;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dWaypoints_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Acceleration", "The rate at which this GameObject accelerates toward its current target.");
			Draw("MaximumSpeed", "The maximum speed at which this GameObject can move toward its current target.");
			Draw("SpeedBoost", "The extra acceleration given to stop this gameObject orbiting its target.");
			Draw("MinimumDistance", "If this gameObject gets within this distance of its current target then it will switch target.");
			Draw("PointOffset", "All the point positions will be offset by this.");
			BeginError(Any(tgts, t => t.Points == null || t.Points.Length == 0));
				Draw("Points", "The points this GameObject will randomly move between.");
			EndError();
		}
	}
}
#endif