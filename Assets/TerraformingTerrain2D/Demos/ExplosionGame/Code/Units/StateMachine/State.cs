using System;

namespace ExplosionGame
{
    public abstract class State
    {
        private readonly IAnimationState _animationState;

        protected State(IAnimationState animationState)
        {
            _animationState = animationState;
        }

        public event Action Entered;
        public event Action Exited;

        public void Enter()
        {
            Entered?.Invoke();
            _animationState.Play();

            OnEnter();
        }

        public void Exit()
        {
            Exited?.Invoke();
            OnExit();
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Update()
        {
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnExit()
        {
        }
    }
}