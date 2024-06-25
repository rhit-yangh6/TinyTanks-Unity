using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current Rigidbody2D into a snake that can be controlled with the <b>Horizontal</b> and <b>Vertical</b> input axes.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dPlayerSnake")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Player Snake")]
	public class D2dPlayerSnake : MonoBehaviour
	{
		/// <summary>The fingers/keys used to move left and right.</summary>
		public CwInputManager.Axis HorizontalControls = new CwInputManager.Axis(1, false, CwInputManager.AxisGesture.HorizontalPull, 0.01f, KeyCode.A, KeyCode.D, KeyCode.LeftArrow, KeyCode.RightArrow, 1.0f);

		/// <summary>The fingers/keys used to move down and up.</summary>
		public CwInputManager.Axis VerticalControls = new CwInputManager.Axis(1, false, CwInputManager.AxisGesture.VerticalPull, 0.01f, KeyCode.S, KeyCode.W, KeyCode.DownArrow, KeyCode.UpArrow, 1.0f);

		/// <summary>The maximum speed.</summary>
		public float Speed = 100.0f;

		/// <summary>This allows you to set the GameObject that contains the D2dRepeatStamp component that will be used to dig in front of the snake.</summary>
		public GameObject DigRoot;

		/// <summary>This allows you to set the distance from snake head the DigRoot will be placed relative to the desired movement direction.</summary>
		public float DigDistance = 0.25f;

		/// <summary>This allows you to set the amount of rays that will be fired around the snake head to handle movement.</summary>
		public int RayCount = 8;

		/// <summary>This allows you to set the distance from the snake head the rays will fire in world space.</summary>
		public float RayRange = 1.0f;

		/// <summary>This allows you to set the Rigidbody2D.AddForce acceleration applied per raycast hit.</summary>
		public float HitMovement = 1.0f;

		/// <summary>This allows you to set the Rigidbody2D.drag applied per raycast hit.</summary>
		public float HitDrag = 1.0f;

		/// <summary>This allows you to set the prefab that will be used when spawning tail segments.</summary>
		public Transform TailPrefab;

		/// <summary>This allows you to set how many tail segments will be initially spawned with the snake.</summary>
		public int TailInitial = 10;

		/// <summary>This allows you to set the maximum distance between each snake tail segment in world space.</summary>
		public float TailSpacing = 0.5f;

		/// <summary>This stores a list of all tail segments that have been spawned.</summary>
		public List<Transform> TailSegments;

		[System.NonSerialized]
		private Rigidbody2D cachedBody;

		private static RaycastHit2D[] hits = new RaycastHit2D[64];

		[ContextMenu("Add Tail Segment")]
		public void AddTailSegment()
		{
			if (TailPrefab != null)
			{
				var segment = Instantiate(TailPrefab, transform.position, transform.rotation);

				if (TailSegments == null)
				{
					TailSegments = new List<Transform>();
				}

				TailSegments.Add(segment);

				segment.gameObject.SetActive(true);
			}
		}

		protected virtual void OnEnable()
		{
			CwInputManager.EnsureThisComponentExists();

			cachedBody = GetComponent<Rigidbody2D>();
		}

		protected virtual void Start()
		{
			for (var i = 0; i < TailInitial; i++)
			{
				AddTailSegment();
			}
		}

		protected virtual void Update()
		{
			// Update input
			if (TailSegments != null)
			{
				var prev = transform;

				for (var i = 0; i < TailSegments.Count; i++)
				{
					var cur  = TailSegments[i];
					var dist = Vector3.Distance(prev.position, cur.position);

					if (dist > TailSpacing)
					{
						cur.position = Vector3.MoveTowards(cur.position, prev.position, dist - TailSpacing);
					}

					prev = cur;
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			var hitCount = 0;

			for (var i = 0; i < RayCount; i++)
			{
				var dir   = GetRayDirection(i);
				var count = Physics2D.RaycastNonAlloc(transform.position, dir, hits, RayRange);

				for (var j = 0; j < count; j++)
				{
					var hit = hits[j];

					if (hit.transform != transform)
					{
						hitCount += 1; break;
					}
				}
			}

			cachedBody.drag = HitDrag * hitCount;

			var delta = default(Vector2);

			delta.x = HorizontalControls.GetValue(1.0f);
			delta.y =   VerticalControls.GetValue(1.0f);

			if (delta.magnitude > 1.0f)
			{
				delta /= delta.magnitude;
			}

			delta *= Speed;

			delta.x = Mathf.Clamp(delta.x / 100.0f, -1.0f, 1.0f);
			delta.y = Mathf.Clamp(delta.y / 100.0f, -1.0f, 1.0f);

			cachedBody.AddForce(delta * HitMovement * hitCount);

			if (DigRoot != null)
			{
				DigRoot.SetActive(delta != Vector2.zero);

				DigRoot.transform.localPosition = delta * DigDistance;
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			for (var i = 0; i < RayCount; i++)
			{
				var dst = transform.position + GetRayDirection(i) * RayRange;

				Gizmos.DrawLine(transform.position, dst);
			}
		}
#endif

		private Vector3 GetRayDirection(int index)
		{
			var a = (360.0f / RayCount) * Mathf.Deg2Rad * index;

			return new Vector3(Mathf.Sin(a), Mathf.Cos(a));
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dPlayerSnake;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dPlayerSnake_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("HorizontalControls", "The fingers/keys used to move left and right.");
			Draw("VerticalControls", "The fingers/keys used to move down and up.");
			Draw("Speed", "The maximum speed.");

			Separator();

			BeginError(Any(tgts, t => t.DigRoot == null));
				Draw("DigRoot", "This allows you to set the GameObject that contains the D2dRepeatStamp component that will be used to dig in front of the snake.");
			EndError();
			Draw("DigDistance", "This allows you to set the distance from snake head the DigRoot will be placed relative to the desired movement direction.");

			Separator();

			Draw("RayCount", "This allows you to set the amount of rays that will be fired around the snake head to handle movement.");
			Draw("RayRange", "This allows you to set the distance from the snake head the rays will fire in world space.");
			Draw("HitMovement", "This allows you to set the Rigidbody2D.AddForce acceleration applied per raycast hit.");
			Draw("HitDrag", "This allows you to set the Rigidbody2D.drag applied per raycast hit.");

			Separator();

			BeginError(Any(tgts, t => t.TailPrefab == null));
				Draw("TailPrefab", "This allows you to set the prefab that will be used when spawning tail segments.");
			EndError();
			Draw("TailInitial", "This allows you to set how many tail segments will be initially spawned with the snake.");
			Draw("TailSegments", "This allows you to set the maximum distance between each snake tail segment in world space.");
			BeginError(Any(tgts, t => t.TailSpacing <= 0.0f));
				Draw("TailSpacing", "This stores a list of all tail segments that have been spawned.");
			EndError();
		}
	}
}
#endif