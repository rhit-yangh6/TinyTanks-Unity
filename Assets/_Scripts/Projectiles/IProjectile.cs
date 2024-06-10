using System.Collections;
using TerraformingTerrain2d;

namespace _Scripts.Projectiles
{
    public interface IProjectile
    {
        void Detonate();
        void Activate();
        void DealDamage();
        void Spin();
        void Direct();
        void Disappear();
        IEnumerator TemporarilyDisableCollider();
    }
}
