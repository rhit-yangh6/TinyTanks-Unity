using UnityEngine;
using CW.Common;

namespace Destructible2D.Examples
{
	/// <summary>This component constantly stamps the current position, allowing you to make effects like melting.</summary>
	[HelpURL(D2dCommon.HelpUrlPrefix + "D2dRepeatStamp")]
	[AddComponentMenu(D2dCommon.ComponentMenuPrefix + "Repeat Stamp")]
	public class D2dRepeatStamp : MonoBehaviour, UnityEngine.ISerializationCallbackReceiver
	{
		/// <summary>The layers the stamp works on.</summary>
		public LayerMask Layers = -1;

		/// <summary>Should the stamp exclude a specific destructible object?</summary>
		public D2dDestructible Exclude;

		/// <summary>This allows you to change the painting type.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the stamp.</summary>
		public D2dShape StampShape;

		/// <summary>The shape of the stamp when it modifies destructible RGB data.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("Shape")]
		public Texture2D ColorShape;

		/// <summary>The shape of the stamp when it modifies destructible alpha data.</summary>
		[UnityEngine.Serialization.FormerlySerializedAs("Shape")]
		public Texture2D AlphaShape;

		/// <summary>The stamp shape will be multiplied by this.
		/// Solid White = No Change</summary>
		public Color Color = Color.white;

		/// <summary>The size of the stamp in world space.</summary>
		public Vector2 Size = Vector2.one;

		/// <summary>The delay between each repeat stamp.</summary>
		public float Delay = 0.25f;

		private float cooldown;

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (cooldown <= 0.0f)
			{
				cooldown = Delay;

				var angle = Random.Range(0.0f, 360.0f);

				D2dStamp.All(Paint, transform.position, Size, angle, StampShape, Color, Layers, Exclude);
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			try
			{
				if (ColorShape != null)
				{
					StampShape.Color = ColorShape; ColorShape = null;
				}

				if (AlphaShape != null)
				{
					StampShape.Alpha = AlphaShape; AlphaShape = null;
				}
			}
			catch {}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	using UnityEditor;
	using TARGET = D2dRepeatStamp;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class D2dRepeatStamp_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("Layers", "The layers the stamp works on.");
			Draw("Exclude", "Should the stamp exclude a specific destructible object?");

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			Draw("StampShape", "The shape of the stamp.");
			Draw("Color", "The stamp shape will be multiplied by this.\nSolid White = No Change");
			Draw("Size", "The size of the stamp in world space.");
			Draw("Delay", "The delay between each repeat stamp.");
		}
	}
}
#endif