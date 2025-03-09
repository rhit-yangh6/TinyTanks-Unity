using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SniperProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private float fixedMagnitude = 13.0f;
        [SerializeField] private float extraForceXMultiplier = 0.5f;
        [SerializeField] private float extraForceYMultiplier = 0.5f;

        // References
        protected override float Radius => Level >= 3 ? radius * 1.5f : radius;
        protected override int Steps {
            get
            {
                if (Level == 4) return (int)(steps * 3f); // LEVEl 5
                return Level >= 2 ? (int)(steps * 1.5f) : steps; // LEVEL 2+
            }
        }
        protected override void Start()
        {
            base.Start();
            var velocity = rigidBody2D.velocity;
            rigidBody2D.GetComponent<ConstantForce2D>().force = new Vector3(velocity.x * extraForceXMultiplier,
                velocity.y * extraForceYMultiplier, 0);
        }

        private void Update() { Direct(); }

        public override void DealDamage()
        {
            var pos = transform.position;

            var isCritical = false;
            if (Level >= 4) isCritical = Random.value > 0.75;
            if (Level == 6) isCritical = true;

            DamageHandler.i.HandleDamage(pos, Radius, isCritical ? Damage * 1.5f : Damage, 
                DamageHandler.DamageType.Circular, isCritical);

            if (Level >= 3) EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
        }

        public override float GetFixedMagnitude()
        {
            return fixedMagnitude;
        }

        public override void Disappear()
        {
            base.Disappear();
            rigidBody2D.GetComponent<ConstantForce2D>().force = Vector2.zero;
        }
    }
}
