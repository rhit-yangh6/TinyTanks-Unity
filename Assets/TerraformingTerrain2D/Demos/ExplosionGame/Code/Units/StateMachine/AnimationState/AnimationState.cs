using UnityEngine;

namespace ExplosionGame
{
    public class AnimationState : IAnimationState
    {
        private readonly Animator _animator;
        private readonly string _animationName;

        public AnimationState(Animator animator, string animationName)
        {
            _animator = animator;
            _animationName = animationName;
        }

        public void Play()
        {
            _animator.Play(_animationName);
        }
    }
}