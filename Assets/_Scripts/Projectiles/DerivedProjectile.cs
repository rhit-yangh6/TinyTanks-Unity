namespace _Scripts.Projectiles
{
    public class DerivedProjectile : Projectile
    {
        public void SetParameters(float setDamage, float setRadius)
        {
            damage = setDamage;
            radius = setRadius;
        }
    }
}
