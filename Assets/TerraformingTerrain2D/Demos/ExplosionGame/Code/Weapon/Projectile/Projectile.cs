using System;
using TerraformingTerrain2d;
using UnityEngine;

namespace ExplosionGame
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private float _explosionRadius = 2f;
        [SerializeField] private Explosion _explosionPrefab;
        private bool _isExploding;

        public event Action Exploded;

        private Explosion ExplosionPrefab => _explosionPrefab;

        protected void Explode(TerraformingTerrain2dChunk chunk)
        {
            if (_isExploding == false)
            {
                _isExploding = true;
                chunk.TerraformingPresenter.Rebuild(transform.position, _explosionRadius, TerraformingMode.Carve);

                Explosion explosion = Instantiate(ExplosionPrefab);
                explosion.transform.position = transform.position;
                explosion.Play();

                Exploded?.Invoke();

                Destroy(gameObject);
            }
        }

        public abstract void Shoot(Transform spawnPoint, float angle, float energy);
    }
}