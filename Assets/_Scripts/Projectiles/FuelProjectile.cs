using System;
using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FuelProjectile : LaunchedProjectile
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float fuelAmount = 35f;
        
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
                        playerController.Refuel(fuelAmount);
                    }
                }
            }

            // Deal damage to all the other entities
            DamageHandler.i.HandleDamageExcludingEntity(pos, Radius, Damage, DamageHandler.DamageType.Circular, 
                Shooter.GetComponent<Entity>(),false, GameAssets.i.oilyBuff, 1);
        }
    }
}