using UnityEngine;

namespace _Scripts.Projectiles
{
    public class LaunchedProjectile : Projectile
    {
        public int Level { get; set; }

        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            renderer = GetComponent<Renderer>();
            StartCoroutine(TemporarilyDisableCollider());
        }

        public float GetMaxMagnitude()
        {
            return MaxMagnitude;
        }

        public int GetSteps()
        {
            return Steps;
        }

        public virtual float GetFixedMagnitude()
        {
            return -1f;
        }
    }
}
