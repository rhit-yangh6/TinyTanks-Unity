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
        [SerializeField] private float velocityMultiplier = 4.0f;
        
        // References
        protected override float Damage {
            get
            {
                if (Level >= 4) return damage * 1.38f; // LEVEl 4
                return Level >= 3 ? damage * 1.15f : damage; // LEVEL 3
            }
        }
        protected override int Steps => Level >= 2 ? (int)(steps * 1.24f) : steps; // LEVEL 2
        private float VelocityMultiplier
        {
            get
            {
                return Level switch
                {
                    >= 4 => velocityMultiplier * 1.24f,
                    _ => velocityMultiplier
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
            var velocity = rigidBody2D.velocity;
            velocity *= VelocityMultiplier;
            rigidBody2D.velocity = velocity;
            if (Level == 5) // LEVEL 5
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var guideDirection = (mousePosition - (Vector2)transform.position).normalized;
                rigidBody2D.velocity += guideDirection * 5f;
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, Level == 6 ? 2 : 1, DestroyTypes.Circular);
        }
    }
}
