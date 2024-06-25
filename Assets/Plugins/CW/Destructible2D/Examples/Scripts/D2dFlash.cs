using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component automatically adds flash to the <b>D2dCameraFlash</b> component.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dFlash")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Flash")]
	public class D2dFlash : MonoBehaviour
	{
		[Tooltip("The amount of flash this applies to the D2dCameraFlash component")]
		public float Flash;

		protected virtual void Awake()
		{
			for (var i = D2dCameraFlash.Instances.Count - 1; i >= 0; i--)
			{
				D2dCameraFlash.Instances[i].Flash += Flash;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dFlash;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dFlash_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Flash <= 0.0f));
				Draw("Flash");
			EndError();
		}
	}
}
#endif