using System;
using System.Collections;
using Unity.Mathematics;
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
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private float _timer;
        private Rigidbody2D _rb;
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            StartCoroutine(Scale());
        }

        void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        IEnumerator Scale()
        {
            while(true) // this could also be a condition indicating "alive or dead"
            {
                // we scale all axis, so they will have the same value, 
                // so we can work with a float instead of comparing vectors
                while(_maxSize > transform.localScale.x)
                {
                    _timer += Time.deltaTime;
                    transform.localScale +=  Time.deltaTime * _growFactor * new Vector3(1, 1, 0);
                    yield return null;
                }
            }
        }

        public override void Detonate()
        {
            Vector2 pos = transform.position;
            
            // TODO: 1 + ??
            var multiplier = Math.Max(Math.Min(_maxSize / 2, _timer / 2), 1f);
            
            DamageHandler.i.HandleCircularDamage(pos, Radius * multiplier, Damage * multiplier);
            
            // TODO: SNOWBALL - DESTROY TERRAIN IF UPGRADED?
            
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }
        
        public override void SpawnExplosionFX()
        {
            float multiplier = Math.Max(Math.Min(_maxSize / 2, _timer / 2), 1f);
            
            GameObject insExpl = Instantiate(ExplosionFX, transform.position, Quaternion.identity);
            insExpl.transform.localScale *= Radius * multiplier;
            Destroy(insExpl, ExplosionDuration);
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
