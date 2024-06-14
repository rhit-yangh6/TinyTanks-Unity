using _Scripts.Managers;

namespace _Scripts.Projectiles
{
    public class IceCubeProjectile: LaunchedProjectile
    {
        // References
        protected override float Damage => Level >= 2 ? damage * 1.1f : damage;

        private int BuffLevel
        {
            get
            {
                return Level switch
                {
                    5 => 4,
                    6 => 5,
                    4 => 3,
                    3 => 2,
                    _ => 1
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
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, 
                false, GameAssets.i.frozenBuff, BuffLevel);
        }
    }
}