using UnityEditor;

namespace EditorWrapper
{
    public abstract class PropertyChangedListener : IDrawable
    {
        private readonly IDrawable _drawable;

        protected PropertyChangedListener(IDrawable drawable)
        {
            _drawable = drawable;
        }

        public void Draw()
        {
            EditorGUI.BeginChangeCheck();

            _drawable.Draw();

            if (EditorGUI.EndChangeCheck())
            {
                OnPropertyChanged();
            }
        }

        protected abstract void OnPropertyChanged();
    }
}