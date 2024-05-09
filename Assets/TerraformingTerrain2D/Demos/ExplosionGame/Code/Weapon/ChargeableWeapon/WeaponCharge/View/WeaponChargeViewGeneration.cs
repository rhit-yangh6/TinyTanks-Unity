using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExplosionGame
{
    [Serializable]
    public class WeaponChargeViewGeneration
    {
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _targetColor;
        [SerializeField] private float _startScale = 1;
        [SerializeField] private float _targetScale = 2;
        [SerializeField] private float _offset;
        [SerializeField] private int _circlesCount;
        [SerializeField] private Transform[] _circles;
        [SerializeField] private Sprite _circleSprite;
        [SerializeField] private int _startSortingOrder = 1;

        public Transform[] Circles => _circles;

        public void Regenerate(Transform parent)
        {
            SetupChildArray();

            for (int i = 0; i < _circles.Length; ++i)
            {
                GameObject circle = new($"Circle = {i + 1}");
                float lerp = (float)i / _circlesCount;

                SetupSpriteRenderer(circle, lerp, i);
                SetupTransform(circle, lerp, i, parent);
            }
        }

        private void SetupTransform(GameObject circle, float lerp, int i, Transform transform)
        {
            circle.transform.parent = transform;
            circle.transform.localPosition = new Vector3(lerp * _offset, 0, 0);
            circle.transform.localScale = Vector3.Lerp(Vector3.one * _startScale, Vector3.one * _targetScale, lerp);
            _circles[i] = circle.transform;
        }

        private void SetupChildArray()
        {
            foreach (Transform child in _circles)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            _circles = new Transform[_circlesCount];
        }

        private void SetupSpriteRenderer(GameObject circle, float lerp, int i)
        {
            SpriteRenderer spriteRenderer = circle.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _circleSprite;

            Color color = Color.Lerp(_startColor, _targetColor, lerp);
            color.a = 255;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = _startSortingOrder + i;
        }
    }
}