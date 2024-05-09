using System;

namespace EditorWrapper
{
    public class ConditionalDraw : IDrawable
    {
        private readonly IDrawable _trueDrawable;
        private readonly IDrawable _falseDrawable;
        private readonly Func<bool> _condition;

        public ConditionalDraw(IDrawable trueDrawable, IDrawable falseDrawable, Func<bool> condition)
        {
            _trueDrawable = trueDrawable;
            _falseDrawable = falseDrawable;
            _condition = condition;
        }
        
        public void Draw()
        {
            if (_condition())
            {
                _trueDrawable.Draw();
            }
            
            else
            {
                _falseDrawable.Draw();
            }
        }
    }
}