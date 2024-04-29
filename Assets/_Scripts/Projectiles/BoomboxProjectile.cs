using System;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomboxProjectile: LaunchedProjectile
    {
        // Set In Inspector
        [SerializeField] private GameObject shockFX;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _shockDamage, _shockRadius, _shockInterval;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private int _shockTimeLeft; 
        private float _shockIntervalLeft;
        private Rigidbody2D _rb;
        
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();

            _shockTimeLeft = 5;
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate(0, 0, velocity.x > 0 ? -0.5f : 0.5f);
            
            if (_shockIntervalLeft > 0)
            {
                _shockIntervalLeft -= Time.deltaTime;
            }

            if (_shockTimeLeft > 0 && Input.GetMouseButtonDown(0) && _shockIntervalLeft <= 0)
            {
                var pos = transform.position;
                DamageHandler.i.HandleDamage(pos,
                    _shockRadius, _shockDamage, DamageHandler.DamageType.Circular);
                DoCameraShake();
                
                GameObject insExpl = Instantiate(shockFX, pos, Quaternion.identity);
                insExpl.GetComponent<ShockwaveManager>().CallShockwave(ExplosionDuration, 0.1f);
                insExpl.transform.localScale *= _shockRadius;
                Destroy(insExpl, ExplosionDuration);
                
                _shockTimeLeft--;
                _shockIntervalLeft = _shockInterval;
            }
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            _shockDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "shockDamage").value;
            _shockRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "shockRadius").value;
            _shockInterval = Array.Find(extraWeaponTerms, ewt => ewt.term == "shockInterval").value;
        }
    }
}