using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class RocketProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        
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
        private float VelocityMultiplier
        {
            get
            {
                return Level switch
                {
                    >= 4 => _velocityMultiplier * 1.24f,
                    _ => _velocityMultiplier
                };
            }
        }
        
        // Other Variables
        private bool _isActivated;
        private ParticleSystem _ps;
    
        private void Start()
        {
            _ps = gameObject.GetComponentInChildren<ParticleSystem>();
            _ps.Stop();
        }

        private void Update()
        {
            Direct();
            if (!Input.GetMouseButtonDown(0) || _isActivated) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
            var velocity = Rigidbody2D.velocity;
            velocity *= VelocityMultiplier;
            Rigidbody2D.velocity = velocity;
            if (Level == 5) // LEVEL 5
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var guideDirection = (mousePosition - (Vector2)transform.position).normalized;
                Rigidbody2D.velocity += guideDirection * 5f;
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, Level == 6 ? 2 : 1, DestroyTypes.Circular);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.explosionFX;
  
            _velocityMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "velocityMultiplier").value;
        }
    }
}
