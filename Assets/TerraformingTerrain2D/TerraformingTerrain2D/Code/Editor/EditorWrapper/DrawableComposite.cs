using System.Collections.Generic;

namespace EditorWrapper
{
    public class DrawableComposite : IDrawable
    {
        private readonly IEnumerable<IDrawable> _drawables;

        public DrawableComposite(IEnumerable<IDrawable> drawables)
        {
            _drawables = drawables;
        }

        public void Draw()
        {
            foreach (IDrawable drawable in _drawables)
            {
                drawable.Draw();
            }
        }
    }
}