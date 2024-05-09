using UnityEngine;

namespace ExplosionGame
{
    public class GrenadeFragmentsSpawner : MonoBehaviour
    {
        [SerializeField] private ParabolaProjectile[] _fragments;
        [SerializeField] private float _energy = 5;

        public void Spawn(Transform spawnPoint)
        {
            foreach (ParabolaProjectile fragmentPrefab in _fragments)
            {
                ParabolaProjectile fragment = Instantiate(fragmentPrefab);

                float angle = Random.Range(45, 135) * Mathf.Deg2Rad;
                fragment.Shoot(spawnPoint, angle, _energy);
            }
        }
    }
}