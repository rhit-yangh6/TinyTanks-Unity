using _Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class MedKitProjectile : LaunchedProjectile
    {
        // References
        protected override float Radius => Level >= 2 ? radius * 1.3f : radius;
        protected override float Damage => Level >= 3 ? damage * 1.35f : damage;
        private bool IsCompleteHeal
        {
            get
            {
                return Level switch
                {
                    5 => Random.value > 0.75,
                    >= 4 => Random.value > 0.95,
                    _ => false
                };
            }
        }

        public override void DealDamage()
        {
            Vector2 pos = transform.position;

            if (IsCompleteHeal)
            {
                DamageHandler.i.HandleCompleteHeals(pos, Radius, Level == 6 ? GameAssets.i.healingBuff : null);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                    false, Level == 6 ? GameAssets.i.healingBuff : null);
            }
        }

        private void Update()
        {
            Spin();
        }
    }
}