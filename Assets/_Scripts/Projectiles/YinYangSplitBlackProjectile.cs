using _Scripts.Managers;

namespace _Scripts.Projectiles
{
    public class YinYangSplitBlackProjectile : DerivedProjectile
    {
        private void Update()
        {
            Spin();
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
        }
    }
}
