using System;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomboxProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        
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
        private int ShockTimes
        {
            get
            {
                return Level switch
                {
                    >= 6 => 1,
                    >= 5 => 5,
                    >= 3 => 3,
                    _ => 2
                };
            }
        }
        private float ShockDamage
        {
            get
            {
                return Level switch
                {
                    6 => _shockDamage * 2f,
                    5 => _shockDamage * 0.5f,
                    >= 4 => _shockDamage * 1.2f,
                    >= 2 => _shockDamage * 1.18f,
                    _ => _shockDamage
                };
            }
        }
        private float ShockRadius
        {
            get
            {
                return Level switch
                {
                    6 => _shockRadius * 0.7f,
                    >= 4 => _shockRadius * 1.3f,
                    >= 2 => _shockRadius * 1.2f,
                    _ => _shockRadius
                };
            }
        }
        private float ShockInterval
        {
            get
            {
                return Level switch
                {
                    5 => _shockInterval * 0.4f,
                    _ => _shockInterval
                };
            }
        }
        
        // Other Variables
        private int _shockTimeLeft; 
        private float _shockIntervalLeft;
        
        private void Start()
        {
            _shockTimeLeft = ShockTimes;
        }

        private void Update()
        {
            Spin();
            
            if (_shockIntervalLeft > 0)
            {
                _shockIntervalLeft -= Time.deltaTime;
            }

            if (_shockTimeLeft > 0 && Input.GetMouseButtonDown(0) && _shockIntervalLeft <= 0)
            {
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            Shock();
            _shockTimeLeft--;
            _shockIntervalLeft = ShockInterval;
        }

        private void Shock()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos,
                ShockRadius, ShockDamage, DamageHandler.DamageType.Circular);
            DoCameraShake();
                
            GameObject insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
            insExpl.GetComponent<ShockwaveManager>().CallShockwave(ExplosionDuration, 0.04f * ShockRadius);
            Destroy(insExpl, ExplosionDuration);
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