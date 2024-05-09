using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class StarAnimation : IInitializable, IRestart
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly StarAnimationData _animationData;
        private readonly Transform _transform;
        private Coroutine _rotationAnimation;
        private Coroutine _movingAnimation;

        public StarAnimation(Transform transform, ICoroutineRunner coroutineRunner, StarAnimationData animationData)
        {
            _coroutineRunner = coroutineRunner;
            _animationData = animationData;
            _transform = transform;
        }

        void IInitializable.Initialize()
        {
            Play();
        }

        private void Play()
        {
            Stop();
            _rotationAnimation = _coroutineRunner.StartCoroutine(StartRotationLoop());
            _movingAnimation = _coroutineRunner.StartCoroutine(StartMoving());
        }

        private void Stop()
        {
            if (_rotationAnimation != null)
                _coroutineRunner.StopCoroutine(_rotationAnimation);

            if (_movingAnimation != null)
                _coroutineRunner.StopCoroutine(_movingAnimation);
        }

        private IEnumerator StartMoving()
        {
            Vector3 startPosition = _transform.position;
            Vector3 startScale = _transform.localScale;
            Vector3 targetPosition = startPosition + _transform.up * _animationData.MovingOffset;
            Vector3 targetScale = startScale + Vector3.one * _animationData.ScalingOffset;

            while (true)
            {
                float input = Mathf.Abs(Time.time * _animationData.MovingSpeed % 2 - 1); // 0 - 1 - 0
                float value = _animationData.Curve.Evaluate(input);

                _transform.position = Vector3.Lerp(startPosition, targetPosition, value);
                _transform.localScale = Vector3.Lerp(startScale, targetScale, value);

                yield return null;
            }
        }

        private IEnumerator StartRotationLoop()
        {
            yield return new WaitForSeconds(_animationData.StartDelay);

            while (true)
            {
                yield return _coroutineRunner.StartCoroutine(Rotate(-_animationData.RotationAngle));
                yield return _coroutineRunner.StartCoroutine(Rotate(_animationData.RotationAngle));
                yield return _coroutineRunner.StartCoroutine(Rotate(-_animationData.RotationAngle));
                yield return _coroutineRunner.StartCoroutine(Rotate(0));

                yield return new WaitForSeconds(_animationData.Rate);
            }
        }

        private IEnumerator Rotate(float rotation)
        {
            Quaternion startRotation = _transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, rotation);
            float lerp = 0;

            while (lerp < 1)
            {
                lerp += Time.deltaTime * _animationData.RotationSpeed;
                float value = _animationData.Curve.Evaluate(lerp);

                _transform.rotation = Quaternion.Lerp(startRotation, targetRotation, value);

                yield return null;
            }
        }

        void IRestart.Restart()
        {
            Play();
        }
    }
}