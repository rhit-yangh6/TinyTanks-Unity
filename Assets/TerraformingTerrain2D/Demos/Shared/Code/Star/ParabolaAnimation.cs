using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class ParabolaAnimation : IRestart
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly AnimationCurve _projectileCurve;
        private readonly Transform _transform;
        private Coroutine _animation;

        public ParabolaAnimation(Transform transform, ICoroutineRunner coroutineRunner, AnimationCurve projectileCurve)
        {
            _coroutineRunner = coroutineRunner;
            _projectileCurve = projectileCurve;
            _transform = transform;
        }

        public IEnumerator Play(Vector3 start, Vector3 target, float duration)
        {
            Restart();
            yield return _animation = _coroutineRunner.StartCoroutine(MoveToTarget(start, target, duration));
        }

        private IEnumerator MoveToTarget(Vector3 start, Vector3 target, float duration)
        {
            float lerp = 0;
            float time = 0;
            float curveDuration = _projectileCurve[_projectileCurve.length - 1].time;
            Vector3 direction = target - start;
            Vector3 perpendicular = new Vector2(-direction.y, direction.x).normalized;

            while (lerp < 1)
            {
                time += Time.deltaTime;
                lerp = time / duration;
                float curveTime = Mathf.Lerp(0, curveDuration, lerp);

                Vector3 horizontalPosition = Vector3.Lerp(start, target, lerp);
                _transform.position = horizontalPosition + perpendicular * _projectileCurve.Evaluate(curveTime);

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