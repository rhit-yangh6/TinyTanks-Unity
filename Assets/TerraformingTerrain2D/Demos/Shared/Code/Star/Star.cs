using System;
using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class Star : MonoBehaviourWrapper
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private StarAnimationData _animationData;
        [SerializeField] private AnimationCurve _projectileCurve;
        [SerializeField] private TrailRenderer _trailRenderer;
        private ParabolaAnimation _parabolaAnimation;
        private RotationAnimation _rotationAnimation;
        private StarWaterFill _waterFill;

        public event Action<Star> Collected;

        public void Compose()
        {
            SetDependencies(new IUnityCallback[]
            {
                new TransformRestart(transform, _trailRenderer),
                new StarAnimation(transform, this, _animationData),
                new StarBallCollect(InvokeCollectEvent),
                _waterFill = new StarWaterFill(_spriteRenderer, InvokeCollectEvent),
                _parabolaAnimation = new ParabolaAnimation(transform, this, _projectileCurve),
                _rotationAnimation = new RotationAnimation(transform, this, 500),
            });
        }

        private void InvokeCollectEvent()
        {
            Collected?.Invoke(this);
        }

        public IEnumerator Fly(Vector3 target, float duration)
        {
            StopAllCoroutines();
            _waterFill.ShowStar();
            _rotationAnimation.Play();
            yield return _parabolaAnimation.Play(transform.position, target, duration);
        }
    }
}