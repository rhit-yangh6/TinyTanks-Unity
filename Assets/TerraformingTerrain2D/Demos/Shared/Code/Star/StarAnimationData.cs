using System;
using UnityEngine;

namespace DemosShared
{
    [Serializable]
    public class StarAnimationData
    {
        [SerializeField] private float _startDelay = 0.1f;
        [SerializeField] private float _rotationSpeed = 2f;
        [SerializeField] private float _movingSpeed = 2f;
        [SerializeField] private float _movingOffset = 0.2f;
        [SerializeField] private float _scalingOffset = 0.2f;
        [SerializeField] private float _rate = 0.3f;
        [SerializeField] private float _rotationAngle = 30;
        [SerializeField] private AnimationCurve _curve;

        public float StartDelay => _startDelay;
        public float RotationSpeed => _rotationSpeed;
        public float MovingSpeed => _movingSpeed;
        public float MovingOffset => _movingOffset;
        public float ScalingOffset => _scalingOffset;
        public float Rate => _rate;
        public float RotationAngle => _rotationAngle;
        public AnimationCurve Curve => _curve;
    }
}