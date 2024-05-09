using System.Collections.Generic;

namespace EditorWrapper
{
    public class Section : IDrawable
    {
        private readonly IDrawable _content;
        private readonly Label _label;
        private readonly Space _space;
        
        public Section(string name, int space, IEnumerable<IDrawable> components) : this(name, space, new DrawableComposite(components))
        {
        }
        
        public Section(string name, int space, IDrawable content)
        {
            _label = new Label(name);
            _space = new Space(space);
            _content = content;
        }

        public void Draw()
        {
            _label.Draw();
            _content.Draw();
            _space.Draw();
        }
    }
}