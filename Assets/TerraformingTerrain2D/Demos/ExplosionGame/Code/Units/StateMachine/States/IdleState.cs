using UnityEngine;

namespace ExplosionGame
{
    public class IdleState : State
    {
        private readonly Rigidbody2D _rigidbody2D;

        public IdleState(IAnimationState animationState, Rigidbody2D rigidbody2D) : base(animationState)
        {
            _rigidbody2D = rigidbody2D;
        }

        protected override void OnEnter()
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
}