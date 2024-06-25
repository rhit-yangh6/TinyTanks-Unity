using UnityEngine;
using CW.Common;

namespace Destructible2D
{
	/// <summary>This class stores a snapshot of a D2dSprite's current state of destruction.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSnapshot")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Snapshot")]
	public class D2dSnapshot : MonoBehaviour
	{
		/// <summary>This gives you the snapshot data in a normal class.</summary>
		public D2dSnapshotData Data { set { data = value; } get { if (data == null) data = new D2dSnapshotData(); return data; } } [SerializeField] private D2dSnapshotData data;

		/// <summary>This allows you to get the data value without causing it to automatically initialize.</summary>
		public D2dSnapshotData DataRaw { set { data = value; } get { return data; } }

		/// <summary>This will clear all snapshot data.</summary>
		[ContextMenu("Clear")]
		public void Clear()
		{
			if (data != null)
			{
				data.Clear();
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Inspector
{
	using UnityEditor;
	using TARGET = D2dSnapshot;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSnapshot_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var destructible = (D2dDestructible)EditorGUILayout.ObjectField("Save", default(D2dDestructible), typeof(D2dDestructible), true);

			if (destructible != null)
			{
				Each(tgts, t => t.Data.Save(destructible), true);
			}

			Separator();

			if (Button("Clear") == true)
			{
				Each(tgts, t => t.Clear(), true);
			}

			Separator();

			if (tgt.Data.Ready == true)
			{
				BeginDisabled();
					EditorGUILayout.IntField("Alpha Width", tgt.Data.AlphaWidth);
					EditorGUILayout.IntField("Alpha Height", tgt.Data.AlphaHeight);
				EndDisabled();
			}
		}
	}
}
#endif