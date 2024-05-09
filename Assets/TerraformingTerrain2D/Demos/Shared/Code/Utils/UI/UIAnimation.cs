using System.Collections;
using UnityEngine;

namespace DemosShared
{
    public class UIAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationPath _showAnimation;
        [SerializeField] private AnimationPath _hideAnimation;
        private Coroutine _currentAnimation;

        public void Show()
        {
            StartAnimation(_showAnimation);
        }

        public void Hide()
        {
            StartAnimation(_hideAnimation);
        }

        private void StartAnimation(AnimationPath path)
        {
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            _currentAnimation = StartCoroutine(Animate(path));
        }

        private IEnumerator Animate(AnimationPath path)
        {
            float timer = 0;

            while (timer < path.Time)
            {
                timer += Time.deltaTime;
                Vector2 position = path.Evaluate(timer / path.Time);

                ((RectTransform)transform).anchoredPosition = position;

                yield return null;
            }
        }
    }
}