using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DemosShared
{
    public class UIStar : MonoBehaviour, IRestart
    {
        [SerializeField] private Image _image;
        [SerializeField] private AnimationCurve _scaleCurve;
        private Color _startColor;

        private void Start()
        {
            _startColor = _image.color;
        }

        public Vector3 GetWorldPosition()
        {
            RectTransform rectTransform = (RectTransform)transform;
            Rect screenSpaceRect = RectTransformToScreenSpace(rectTransform);
            Vector2 screenSpaceCentre = screenSpaceRect.center;

            return Camera.main.ScreenToWorldPoint(screenSpaceCentre);
        }

        private Rect RectTransformToScreenSpace(RectTransform rectTransform)
        {
            Vector2 size = Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale);
            return new Rect((Vector2)rectTransform.position - (size * 0.5f), size);
        }

        public IEnumerator PlayAnimation()
        {
            _image.color = Color.white;
            yield return StartCoroutine(PlayScaleAnimation());
        }

        private IEnumerator PlayScaleAnimation()
        {
            float time = 0;
            float lerp = 0;
            float duration = 0.5f;

            while (lerp < 1)
            {
                time += Time.deltaTime;
                lerp = time / duration;

                float scale = _scaleCurve.Evaluate(lerp);
                transform.localScale = Vector3.one * scale;

                yield return null;
            }
        }

        void IRestart.Restart()
        {
            StopAllCoroutines();
            _image.color = _startColor;
            transform.localScale = Vector3.one;
        }
    }
}