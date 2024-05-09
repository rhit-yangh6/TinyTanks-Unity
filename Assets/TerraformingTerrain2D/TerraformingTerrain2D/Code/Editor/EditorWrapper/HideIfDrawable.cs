using System;

namespace EditorWrapper
{ 
    public class HideIfDrawable : IDrawable
    {
        private readonly IDrawable _conditionalDrawable;
        
        public HideIfDrawable(IDrawable drawable, Func<bool> hideCondition)
        {
            _conditionalDrawable = new ConditionalDraw(new Dummy(), drawable, hideCondition);
        }
        
        public void Draw()
        {
            _conditionalDrawable.Draw();
        }
    }
}