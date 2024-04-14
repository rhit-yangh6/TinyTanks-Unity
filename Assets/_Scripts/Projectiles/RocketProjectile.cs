using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class RocketProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _velocityMultiplier;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage {
            get
            {
                if (Level >= 4) return _damage * 1.38f; // LEVEl 4
                return Level >= 3 ? _damage * 1.15f : _damage; // LEVEL 3
            }
        }
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.24f) : _steps; // LEVEL 2
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated;
        private Rigidbody2D _rb;
        private ParticleSystem _ps;
    
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _ps = gameObject.GetComponentInChildren<ParticleSystem>();
            _ps.Stop();
        }

        private void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                velocity *= Level >= 4 ? _velocityMultiplier * 1.24f :_velocityMultiplier; // LEVEL 4
                _rb.velocity = velocity;
                if (Level == 5) // LEVEL 5
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var guideDirection = (mousePosition - (Vector2)transform.position).normalized;
                    _rb.velocity = _rb.velocity + guideDirection * 5f;
                }
                _ps.Play();
            }
        }
        
        public override void Detonate()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);

            TerrainDestroyer.instance.DestroyTerrainCircular(pos, Radius, Level == 6 ? 2 : 1); // LEVEL 6
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.regularExplosionFX;
  
            _velocityMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "velocityMultiplier").value;
        }
    }
}
