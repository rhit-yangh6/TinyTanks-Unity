using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SteelCubeSmallProjectile : DerivedProjectile
    {
        private void Update()
        {
            Spin();
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Square);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Square);
        }
    }
}