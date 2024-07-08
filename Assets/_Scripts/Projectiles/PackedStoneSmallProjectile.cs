using Unity.VisualScripting;

namespace _Scripts.Projectiles
{
    public class PackedStoneSmallProjectile :DerivedProjectile
    {
        private void Update()
        {
            Spin(1.2f);
        }
    }
}