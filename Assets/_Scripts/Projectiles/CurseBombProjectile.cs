using _Scripts.Managers;

namespace _Scripts.Projectiles
{
    public class CurseBombProjectile : LaunchedProjectile
    {
        // References
        protected override float Radius => Level >= 4 ? radius * 1.4f : radius;
        protected override float MaxMagnitude => Level >= 3 ? maxMagnitude * 1.4f : maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(steps * 1.4f) : steps;
        private int FinalBuffLevel
        {
            get
            {
                return Level switch
                {
                    5 => 2,
                    6 => 3,
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
                false, GameAssets.i.cursedBuff, FinalBuffLevel);
        }
    }
}