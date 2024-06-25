using UnityEngine;

namespace Destructible2D
{
	/// <summary>This struct stores the stamp shape color and alpha pair.</summary>
	[System.Serializable]
	public struct D2dShape
	{
		public Texture2D Color;
		public Texture2D Alpha;
		
		public D2dShape(Texture2D newColor, Texture2D newAlpha)
		{
			Color = newColor;
			Alpha = newAlpha;
		}

		public override string ToString()
		{
			return string.Format("(Color={0}, Alpha={1})", Color, Alpha);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D
{
	using CW.Common;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(D2dShape))]
	public class D2dShapeDrawer : PropertyDrawer
	{
		[System.NonSerialized]
		private int index;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var width = (position.width - EditorGUIUtility.labelWidth) / 2;
			var rect1 = position; rect1.xMax = position.xMax - width;
			var rect2 = position; rect2.xMin = position.xMax - width + 2;

			EditorGUI.PropertyField(rect1, property.FindPropertyRelative("Color"), label);

			label.text = "Alpha";

			CwEditor.BeginLabelWidth(60);
				EditorGUI.PropertyField(rect2, property.FindPropertyRelative("Alpha"), label);
			CwEditor.EndLabelWidth();
		}
	}
}
#endif