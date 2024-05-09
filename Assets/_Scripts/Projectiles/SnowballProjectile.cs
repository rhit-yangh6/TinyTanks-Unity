using System;
using System.Collections;
using _Scripts.Buffs;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SnowballProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _maxSize, _growFactor;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 2 ? _damage * 1.1f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => Level >= 3 ? (int)(_steps * 1.75f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private float _timer;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            StartCoroutine(Scale());
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private IEnumerator Scale()
        {
            while(true) // this could also be a condition indicating "alive or dead"
            {
                // we scale all axis, so they will have the same value, 
                // so we can work with a float instead of comparing vectors
                var maxSize = Level >= 4 ? _maxSize * 1.17f : _maxSize;
                while(maxSize > transform.localScale.x)
                {
                    _timer += Time.deltaTime;
                    transform.localScale +=  Time.deltaTime * _growFactor * new Vector3(1, 1, 0);
                    yield return null;
                }
            }
        }

        public override void Detonate()
        {
            var pos = transform.position;
            
            var multiplier = GetDamageAndRadiusMultiplier();

            if (Level == 5)
            {
                // Apply frozen buff if Level = 5
                DamageHandler.i.HandleDamage(pos, Radius * multiplier, Damage * multiplier, 
                    DamageHandler.DamageType.Circular, false, GameAssets.i.frozenBuff, 2);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius * multiplier, Damage * multiplier, 
                    DamageHandler.DamageType.Circular);
            }
            
            if (Level == 6)
            {
                EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                    Radius * multiplier * 1.5f, 1, DestroyTypes.Circular);
            }
            
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }
        
        public override void SpawnExplosionFX()
        {
            var insExpl = Instantiate(ExplosionFX, transform.position, Quaternion.identity);
            insExpl.transform.localScale *= Radius * GetDamageAndRadiusMultiplier();
            Destroy(insExpl, ExplosionDuration);
        }

        private float GetDamageAndRadiusMultiplier()
        {
            var maxSize = Level >= 4 ? _maxSize * 1.17f : _maxSize;
            return Math.Max(Math.Min(maxSize / 2, _timer / 2), 1f);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration,
            ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _maxSize = Array.Find(extraWeaponTerms, ewt => ewt.term == "maxSize").value;
            _growFactor = Array.Find(extraWeaponTerms, ewt => ewt.term == "growFactor").value;
        }
    }
}
