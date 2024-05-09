using UnityEngine;

namespace ExplosionGame
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        protected Transform SpawnPoint => _spawnPoint;

        public void Equip()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public abstract void Shoot();
    }
}