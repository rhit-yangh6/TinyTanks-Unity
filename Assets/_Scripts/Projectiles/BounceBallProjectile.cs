using System;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BounceBallProjectile: LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static int _bounceTimeTotal;
        private static float _bounceDamage, _bounceRadius;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private int _bounceTime;

        private void Start() { _bounceTime = _bounceTimeTotal; }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else if (_bounceTime > 0)
            {
                _bounceTime--;
                Bounce();
            }
            else
            {
                Detonate();
            }
        }

        private void Bounce()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleCircularDamage(pos, _bounceRadius, _bounceDamage);

            // TerrainDestroyer.Instance.DestroyTerrain(pos, _bounceRadius);
        
            SpawnExplosionFX();
            DoCameraShake();
        }
        
        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _bounceTimeTotal = (int)Array.Find(extraWeaponTerms, ewt => ewt.term == "bounceTimeTotal").value;
            _bounceDamage = (int)Array.Find(extraWeaponTerms, ewt => ewt.term == "bounceDamage").value;
            _bounceRadius = (int)Array.Find(extraWeaponTerms, ewt => ewt.term == "bounceRadius").value;
        }


    }
}