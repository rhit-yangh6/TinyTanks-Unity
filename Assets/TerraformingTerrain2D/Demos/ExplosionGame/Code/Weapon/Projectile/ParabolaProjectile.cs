using System.Collections;
using TerraformingTerrain2d;
using UnityEngine;

namespace ExplosionGame
{
    public class ParabolaProjectile : Projectile
    {
        [SerializeField] private float _speed = 1;

        public override void Shoot(Transform spawnPoint, float angle, float energy)
        {
            Destroy(gameObject, 6);

            StartCoroutine(MoveByParabola(spawnPoint.position, angle, energy));
        }

        private IEnumerator MoveByParabola(Vector2 startPosition, float angle, float energy)
        {
            ProjectilePositionCalculator positionCalculator = new(startPosition, energy, angle);
            float time = 0;

            while (true)
            {
                time += Time.deltaTime * _speed;
                Vector2 position = positionCalculator.Evaluate(time);
                Vector2 rotationDirection = (positionCalculator.Evaluate(time + 0.1f) - position).normalized;

                transform.up = rotationDirection;
                transform.position = position;

                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out TerraformingTerrain2dChunk chunk2D))
            {
                Explode(chunk2D);
            }
        }
    }
}