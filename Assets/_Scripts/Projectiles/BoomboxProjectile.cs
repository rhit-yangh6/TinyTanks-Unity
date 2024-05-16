using System;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomboxProjectile: LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _shockDamage, _shockRadius, _shockInterval;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => Level >= 4 ? _maxMagnitude * 1.3f : _maxMagnitude;
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

            _shockTimeLeft = Level switch
            {
                >= 6 => 1,
                >= 5 => 5,
                >= 3 => 3,
                _ => 2
            };

            switch (Level)
            {
                case 6:
                    _shockDamage *= 2;
                    _shockRadius *= 0.7f;
                    break;
                case 5:
                    _shockDamage *= 0.5f;
                    _shockInterval *= 0.4f;
                    break;
                case >= 4:
                    _shockDamage *= 1.2f;
                    _shockRadius *= 1.3f;
                    break;
                case >= 2:
                    _shockDamage *= 1.2f;
                    _shockRadius *= 1.2f;
                    break;
            }
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
                
                GameObject insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
                insExpl.GetComponent<ShockwaveManager>().CallShockwave(ExplosionDuration, 0.04f * _shockRadius);
                // insExpl.transform.localScale *= _shockRadius;
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