using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component listens for collision events and sends them to other components via the <b>OnCollision</b> event.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dCollisionHandler")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Collision Handler")]
	public class D2dCollisionHandler : MonoBehaviour
	{
		/// <summary>This is invoked once for each collision.</summary>
		public event System.Action<Collision2D> OnCollision;

		/// <summary>This is invoked once for each collision.</summary>
		public event System.Action<Collider2D> OnOverlap;

		// Show enable/disable toggle
		protected virtual void OnEnable()
		{
		}

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			if (enabled == true)
			{
				if (OnCollision != null)
				{
					OnCollision(collision);
				}
			}
		}

		protected virtual void OnTriggerStay2D(Collider2D collider)
		{
			if (enabled == true)
			{
				if (OnOverlap != null)
				{
					OnOverlap(collider);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dCollisionHandler;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dCollisionHandler_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Info("This component listens for collision events and sends them to other components.");
		}
	}
}
#endif