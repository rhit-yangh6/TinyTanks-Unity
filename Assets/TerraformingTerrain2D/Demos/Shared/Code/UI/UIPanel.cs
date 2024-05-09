using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class UIPanel : MonoBehaviour, IRestart
    {
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float _duration;
        [SerializeField] private Vector2 _showPosition;
        [SerializeField] private Vector2 _hidePosition;
        private Vector2 _startPosition;

        private void Start()
        {
            _startPosition = ((RectTransform)transform).anchoredPosition;
        }

        [ContextMenu("Show")]
        public void Show()
        {
            StopAllCoroutines();
            StartCoroutine(PlayAnimation(_hidePosition, _showPosition));
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(PlayAnimation(_showPosition, _hidePosition));
        }

        private IEnumerator PlayAnimation(Vector3 startPosition, Vector3 targetPosition)
        {
            float time = 0;
            float lerp = 0;

            while (lerp < 1)
            {
                time += Time.deltaTime;
                lerp = time / _duration;

                float lerpPosition = _animationCurve.Evaluate(lerp);
                ((RectTransform)transform).anchoredPosition =
                    Vector3.LerpUnclamped(startPosition, targetPosition, lerpPosition);

                yield return null;
            }
        }

        void IRestart.Restart()
        {
            StopAllCoroutines();
            ((RectTransform)transform).anchoredPosition = _startPosition;
            OnRestart();
        }

        protected virtual void OnRestart()
        {
        }
    }
}