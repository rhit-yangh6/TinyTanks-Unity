using UnityEngine;

namespace ExplosionGame
{
    public class ClusterGrenade : MonoBehaviour
    {
        [SerializeField] private PhysicsProjectile _projectile;
        [SerializeField] private Transform _fragmentsSpawnPoint;
        [SerializeField] private GrenadeFragmentsSpawner _fragmentsSpawner;

        private void OnEnable()
        {
            _projectile.Exploded += OnExploded;
        }

        private void OnDisable()
        {
            _projectile.Exploded -= OnExploded;
        }

        private void OnExploded()
        {
            _fragmentsSpawner.Spawn(_fragmentsSpawnPoint);
        }
    }
}