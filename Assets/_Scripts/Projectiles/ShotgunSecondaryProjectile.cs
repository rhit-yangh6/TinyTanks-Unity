using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ShotgunSecondaryProjectile : DerivedProjectile
    {
        // Shared Fields
        private static float _radius, _damage,_explosionDuration;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        private void Start()
        {
            StartCoroutine(TemporarilyDisableCollider());
        }

        public override void SetParameters(float damage, float radius, float explosionDuration, GameObject explosionFX)
        {
            _radius = radius;
            _damage = damage;
            _explosionDuration = explosionDuration;
            _explosionFX = explosionFX;
        }
        
        private IEnumerator TemporarilyDisableCollider()
        {
            Collider2D.enabled = false;
            yield return new WaitForSeconds(0.1f);
            Collider2D.enabled = true;
        }
    }
}
