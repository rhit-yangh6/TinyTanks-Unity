using System;
using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class MedalOfValorProjectile: LaunchedProjectile
    {
        protected override float MaxMagnitude => Level >= 2 ? maxMagnitude * 1.2f : maxMagnitude;
        protected override float Damage => Level >= 3 ? damage * 1.2f : damage;
        protected override float Radius => Level >= 3 ? radius * 0.9f : radius;
        private float HighestDamageMultiplier
        {
            get
            {
                return Level switch
                {
                    >= 4 => 2.3f,
                    _ => 2f
                };
            }
        }

        private const float Level4Threshold = 0.3f;

        private void Update()
        {
            Spin();
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, CalculateFinalDamage(), DamageHandler.DamageType.Circular);
            
            // TODO: Level 5 and 6
        }

        private float CalculateFinalDamage()
        {
            var currentHealthPercentage = Shooter.GetComponent<Entity>().GetHealthPercentage();
            if (Level >= 4 && currentHealthPercentage < Level4Threshold)
            {
                return Damage * HighestDamageMultiplier;
            }
            // No damage increase when current percentage is over 75%
            return Mathf.Lerp(HighestDamageMultiplier, 1, currentHealthPercentage) * Damage;
        }
    }
}