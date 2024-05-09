using UnityEditor;

namespace EditorWrapper
{
    public class Space : IDrawable
    {
        private readonly int _space;

        public Space(int space)
        {
            _space = space;
        }

        public void Draw()
        {
            EditorGUILayout.Space(_space);
        }
    }
}