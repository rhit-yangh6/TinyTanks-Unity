using UnityEditor;

namespace EditorWrapper
{
    public class Property : IDrawable
    {
        private readonly SerializedProperty _property;

        public Property(SerializedProperty property)
        {
            _property = property;
        }

        public void Draw()
        {
            EditorGUILayout.PropertyField(_property);
        }
    }
}