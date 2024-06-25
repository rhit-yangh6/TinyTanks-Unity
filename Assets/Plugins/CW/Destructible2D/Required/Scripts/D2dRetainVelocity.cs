using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This component allows a sprite to maintain its velocity after being split.</summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dRetainVelocity")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Retain Velocity")]
	public class D2dRetainVelocity : MonoBehaviour
	{
		[System.NonSerialized]
		private Rigidbody2D cachedBody;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[System.NonSerialized]
		private Vector2 velocity;

		[System.NonSerialized]
		private float angularVelocity;

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();

			cachedDestructible.OnSplitStart += HandleSplitStart;
			cachedDestructible.OnSplitEnd   += HandleSplitEnd;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnSplitStart -= HandleSplitStart;
			cachedDestructible.OnSplitEnd   -= HandleSplitEnd;
		}

		protected virtual void HandleSplitStart()
		{
			if (cachedBody == null) cachedBody = GetComponent<Rigidbody2D>();

			velocity        = cachedBody.velocity;
			angularVelocity = cachedBody.angularVelocity;
		}

		protected virtual void HandleSplitEnd(List<D2dDestructible> splitDestructibles, D2dDestructible.SplitMode mode)
		{
			for (var i = splitDestructibles.Count - 1; i >= 0; i--)
			{
				var splitDestructible = splitDestructibles[i];

				if (splitDestructible.gameObject != gameObject)
				{
					var splitRigidbody2D = splitDestructible.GetComponent<Rigidbody2D>();

					if (splitRigidbody2D != null)
					{
						splitRigidbody2D.velocity        += velocity;
						splitRigidbody2D.angularVelocity += angularVelocity;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dRetainVelocity;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dRetainVelocity_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Info("This component allows a sprite to maintain its velocity after being split.");
		}
	}
}
#endif