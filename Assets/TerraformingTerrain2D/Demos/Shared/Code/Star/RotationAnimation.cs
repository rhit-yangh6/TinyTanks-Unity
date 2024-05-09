using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class RotationAnimation : IRestart
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly Transform _transform;
        private readonly float _speed;
        private Coroutine _animation;

        public RotationAnimation(Transform transform, ICoroutineRunner coroutineRunner, float speed)
        {
            _coroutineRunner = coroutineRunner;
            _transform = transform;
            _speed = speed;
        }

        public void Play()
        {
            Restart();
            _animation = _coroutineRunner.StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            while (true)
            {
                _transform.rotation *= Quaternion.Euler(0, 0, _speed * Time.deltaTime);
                yield return null;
            }
        }

        public void Restart()
        {
            if (_animation != null)
            {
                _coroutineRunner.StopCoroutine(_animation);
            }
        }
    }
}