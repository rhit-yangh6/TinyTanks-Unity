using UnityEngine;
using System.Collections.Generic;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component will automatically spawn the specified prefab when the attached destructible object splits.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(D2dSplitter))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSplitSpawner")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Split Spawner")]
	public class D2dSplitSpawner : MonoBehaviour
	{
		[Tooltip("The prefab spawned on split")]
		public GameObject Prefab;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();

			cachedDestructible.OnSplitEnd += HandleSplitEnd;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnSplitEnd -= HandleSplitEnd;
		}

		private void HandleSplitEnd(List<D2dDestructible> splitDestructibles, D2dDestructible.SplitMode mode)
		{
			if (Prefab != null)
			{
				var clone = Instantiate(Prefab, transform.position, Quaternion.identity);

				clone.SetActive(true);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dSplitSpawner;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSplitSpawner_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Prefab == null));
				Draw("Prefab");
			EndError();
		}
	}
}
#endif