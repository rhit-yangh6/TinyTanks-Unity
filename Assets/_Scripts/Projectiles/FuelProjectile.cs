using System;
using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FuelProjectile : LaunchedProjectile
    {
        [SerializeField] private LayerMask layerMask;

        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    >= 3 => damage * 1.15f,
                    _ => damage
                };
            }
        }
        protected override float MaxMagnitude
        {
            get
            {
                return Level switch
                {
                    >= 3 => maxMagnitude * 1.2f,
                    _ => maxMagnitude
                };
            }
        }
        private float FuelAmount
        {
            get
            {
                return Level switch
                {
                    >= 4 => 200f,
                    >= 2 => 60f,
                    _ => 35f
                };
            }
        }
        
        private void Update()
        {
            Spin();
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;
            
            var hitColliders = Physics2D.OverlapCircleAll(pos, Radius, layerMask);
            
            foreach(var col in hitColliders)
            {
                var rb = col.GetComponent<Rigidbody2D>();
                if (rb == null) continue;
                
                if (ReferenceEquals(Shooter, rb.gameObject))
                {
                    var playerController = Shooter.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.Refuel(FuelAmount);
                        if (Level == 6)
                        {
                            playerController.AddBuff(GameAssets.i.superchargedBuff.InitializeBuff(col.gameObject, 1));
                        }
                    }
                }
            }

            // Deal damage to all the other entities
            DamageHandler.i.HandleDamageExcludingEntity(pos, Radius, Damage, DamageHandler.DamageType.Circular, 
                Shooter.GetComponent<Entity>(),false, GameAssets.i.oilyBuff, 1);
            
            // Also apply burning buff (Level 5)
            if (Level == 5)
            {
                DamageHandler.i.HandleDamageExcludingEntity(pos, Radius, 0, DamageHandler.DamageType.Circular, 
                    Shooter.GetComponent<Entity>(),false, GameAssets.i.burningBuff, 1);
            }
        }
    }
}