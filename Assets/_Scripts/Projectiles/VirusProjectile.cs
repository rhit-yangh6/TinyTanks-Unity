using _Scripts.Managers;

namespace _Scripts.Projectiles
{
    public class VirusProjectile : LaunchedProjectile
    {
        // References
        protected override float Radius => Level >= 3 ? radius * 1.3f : radius;
        protected override float MaxMagnitude => Level >= 3 ? maxMagnitude * 1.3f : maxMagnitude;
        private int BuffLevel
        {
            get
            {
                return Level switch
                {
                    6 => 5,
                    5 => 4,
                    4 => 3,
                    >= 2 => 2,
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
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, false,
                GameAssets.i.infectedBuff, BuffLevel);
        }
    }
}