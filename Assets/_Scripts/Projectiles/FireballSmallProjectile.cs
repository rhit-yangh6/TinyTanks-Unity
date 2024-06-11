using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FireballSmallProjectile: DerivedProjectile
    {
        private void Update()
        {
            Direct();
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                false, GameAssets.i.burningBuff);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
        }
    }
}