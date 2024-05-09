using System;
using UnityEngine;

namespace DemosShared
{
    [Serializable]
    public class AnimationPath
    {
        [SerializeField] private Vector2 _start;
        [SerializeField] private Vector2 _end;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _time;

        public float Time => _time;

        public Vector2 Evaluate(float normalizedValue)
        {
            float lerp = _curve.Evaluate(normalizedValue);

            return Vector2.Lerp(_start, _end, lerp);
        }
    }
}