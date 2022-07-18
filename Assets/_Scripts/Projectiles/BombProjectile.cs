using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BombProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _detonateTime;

        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 4 ? _damage * 1.25f : _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.1f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        // Other Variables
        private SpriteRenderer _sr;
    
        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            StartCoroutine(TickBomb());
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone")) Destroy(gameObject);
        }

        private IEnumerator TickBomb()
        {
            var finalCalculatedDetonateTime = _detonateTime;
            if (Level >= 2) finalCalculatedDetonateTime += 1f;

            Invoke(nameof(Detonate), finalCalculatedDetonateTime);
            while (true)
            {
                _sr.color = Color.red;
                yield return new WaitForSeconds(.1f);
                _sr.color = Color.white;
                yield return new WaitForSeconds(.1f);
            }
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude,
            int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.regularExplosionFX;

            _detonateTime = Array.Find(extraWeaponTerms, ewt => ewt.term == "detonateTime").value;
        }
    }
}
