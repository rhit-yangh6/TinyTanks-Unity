using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component automatically adds shake to the <b>D2dCameraShake</b> component.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dShake")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Shake")]
	public class D2dShake : MonoBehaviour
	{
		/// <summary>The amount of shake this applies to the D2dCameraShake component.</summary>
		public float Shake;

		protected virtual void Awake()
		{
			for (var i = D2dCameraShake.Instances.Count - 1; i >= 0; i--)
			{
				D2dCameraShake.Instances[i].Shake += Shake;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dShake;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dShake_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Shake <= 0.0f));
				Draw("Shake", "The amount of shake this applies to the D2dCameraShake component.");
			EndError();
		}
	}
}
#endif