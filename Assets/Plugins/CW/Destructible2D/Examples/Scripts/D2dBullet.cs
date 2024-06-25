using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current GameObject into a 2D bullet that moves and collides with the world.</summary>
	[ExecuteInEditMode]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dBullet")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Bullet")]
	public class D2dBullet : MonoBehaviour
	{
		/// <summary>The tag this bullet cannot hit.</summary>
		public string IgnoreTag;
		
		/// <summary>The layers this bullet can hit.</summary>
		public LayerMask RaycastMask = -1;
		
		/// <summary>The prefab that gets spawned when this bullet hits something.</summary>
		public GameObject ExplosionPrefab;
		
		/// <summary>The distance this bullet moves each second.</summary>
		public float Speed;
		
		/// <summary>The maximum length of the bullet trail.</summary>
		public float MaxLength;
		
		/// <summary>The scale of the bullet after it's scaled up.</summary>
		public Vector3 MaxScale;
		
		private Vector3 oldPosition;
		
		protected virtual void Start()
		{
			oldPosition = transform.position;
		}
		
		protected virtual void FixedUpdate()
		{
			var newPosition  = transform.position;
			var rayLength    = (newPosition - oldPosition).magnitude;
			var rayDirection = (newPosition - oldPosition).normalized;
			var hit          = Physics2D.Raycast(oldPosition, rayDirection, rayLength, RaycastMask);
			
			// Update old position to trail behind 
			if (rayLength > MaxLength)
			{
				rayLength   = MaxLength;
				oldPosition = newPosition - rayDirection * rayLength;
			}
			
			transform.localScale = MaxScale * CwHelper.Divide(rayLength, MaxLength);
			
			if (hit.collider != null)
			{
				if (string.IsNullOrEmpty(IgnoreTag) == true || hit.collider.tag != IgnoreTag)
				{
					if (ExplosionPrefab != null)
					{
						Instantiate(ExplosionPrefab, hit.point, Quaternion.identity);
					}
					
					Destroy(gameObject);
				}
			}
		}
		
		protected virtual void Update()
		{
			transform.Translate(0.0f, Speed * Time.deltaTime, 0.0f);
		}
		
#if UNITY_EDITOR
		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawLine(transform.position, transform.TransformPoint(0.0f, -MaxLength, 0.0f));
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dBullet;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dBullet_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("IgnoreTag", "The tag this bullet cannot hit.");
			Draw("RaycastMask", "The layers this bullet can hit.");
			Draw("ExplosionPrefab", "The prefab that gets spawned when this bullet hits something.");
			Draw("Speed", "The distance this bullet moves each second.");
			Draw("MaxLength", "The maximum length of the bullet trail.");
			Draw("MaxScale", "The scale of the bullet after it's scaled up.");
		}
	}
}
#endif