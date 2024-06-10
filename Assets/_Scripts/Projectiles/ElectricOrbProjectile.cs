using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ElectricOrbProjectile: LaunchedProjectile
    {
        // References
        protected override float Radius => radius;
        protected override float Damage => damage;
        protected override float MaxMagnitude => maxMagnitude;
        protected override int Steps => steps;
        
        private void Update()
        {
            Spin();
        }

        public override void DealDamage()
        {
            var pos = transform.position;

            var closestEntity = DamageHandler.i.DetectNearestTarget(pos, Radius, null);
            Vector2 previousPosition = default;

            if (closestEntity == null)
            {
                return;
            }

            // First chain, won't deal damage if there's no entity in the radius
            var electricLine = Instantiate(GameAssets.i.electricLineFX).GetComponent<LineController>();
            var entityPos = closestEntity.transform.position;
            electricLine.AssignPositions(pos, entityPos);
            previousPosition = entityPos;
            closestEntity.TakeDamage(Damage);
            
            // Second chain, comes with weapon basic level
            closestEntity = DamageHandler.i.DetectNearestTarget(previousPosition, Radius, closestEntity);
            
            if (closestEntity == null)
            {
                return;
            }
            electricLine = Instantiate(GameAssets.i.electricLineFX).GetComponent<LineController>();
            entityPos = closestEntity.transform.position;
            electricLine.AssignPositions(previousPosition, entityPos);
            closestEntity.TakeDamage(Damage);
        }
    }
}