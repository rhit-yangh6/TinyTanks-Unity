using UnityEngine;

namespace ExplosionGame
{
    public readonly struct ProjectilePositionCalculator
    {
        private readonly Vector2 _startPosition;
        private readonly float _startSpeed;
        private readonly float _angle;
        private const float G = 9.81f;

        public ProjectilePositionCalculator(Vector2 startPosition, float startSpeed, float angle)
        {
            _startPosition = startPosition;
            _startSpeed = startSpeed;
            _angle = angle;
        }

        public Vector2 Evaluate(float time)
        {
            Vector2 position = _startPosition;

            // x = V * cosA * t
            // y = h + V * sinA * t + gt^2 / 2
            position.x += _startSpeed * Mathf.Cos(_angle) * time;
            position.y += _startSpeed * Mathf.Sin(_angle) * time - G * time * time / 2;

            return position;
        }
    }
}