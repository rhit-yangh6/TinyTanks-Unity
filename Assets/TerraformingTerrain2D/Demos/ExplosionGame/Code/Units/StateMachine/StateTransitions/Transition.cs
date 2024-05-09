using System;

namespace ExplosionGame
{
    public class Transition
    {
        public readonly State StateFrom;
        public readonly State StateTo;
        private readonly Func<bool> _condition;

        public Transition(State stateFrom, State stateTo, Func<bool> condition)
        {
            StateFrom = stateFrom;
            StateTo = stateTo;
            _condition = condition;
        }

        public event Action TransitionHappened;

        public bool CheckCondition()
        {
            bool transitionHappened = _condition();

            if (transitionHappened)
            {
                TransitionHappened?.Invoke();
            }

            return transitionHappened;
        }
    }
}