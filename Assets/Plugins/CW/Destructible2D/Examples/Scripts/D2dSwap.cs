using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to swap the current <b>SpriteRenderer</b> sprite with the specified sprite when you manually call the <b>Swap</b> method. This can be done using <b>D2dRequirements</b> to show a different damage state.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dSwap")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Swap")]
	public class D2dSwap : MonoBehaviour
	{
		/// <summary>The visual sprite you want to swap in.</summary>
		public Sprite VisualSprite;

		[System.NonSerialized]
		private SpriteRenderer cachedSpriteRenderer;

		/// <summary>This will instantly trigger the swap.</summary>
		[ContextMenu("Swap")]
		public void Swap()
		{
			if (cachedSpriteRenderer == null) cachedSpriteRenderer = GetComponent<SpriteRenderer>();

			cachedSpriteRenderer.sprite = VisualSprite;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dSwap;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dSwap_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.VisualSprite == null));
				Draw("VisualSprite", "The visual sprite you want to swap in.");
			EndError();
		}
	}
}
#endif