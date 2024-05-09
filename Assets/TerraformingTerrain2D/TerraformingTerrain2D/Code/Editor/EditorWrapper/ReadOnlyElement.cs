using UnityEngine;

namespace EditorWrapper
{
    public class ReadOnlyElement : IDrawable
    {
        private readonly IDrawable _drawable;

        public ReadOnlyElement(IDrawable drawable)
        {
            _drawable = drawable;
        }
        
        public void Draw()
        {
            GUI.enabled = false;
            _drawable.Draw();
            GUI.enabled = true;
        }
    }   
}