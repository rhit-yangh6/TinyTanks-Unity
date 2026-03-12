using _Scripts.GameEngine.WeaponExtraData;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class LaunchedProjectile : Projectile
    {
        public WeaponExtraData WeaponExtraData;
        public int Level { get; set; }

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(TemporarilyDisableCollider());
        }

        public float GetMaxMagnitude()
        {
            return MaxMagnitude;
        }

        public int GetSteps()
        {
            return Steps;
        }

        public virtual float GetFixedMagnitude()
        {
            return -1f;
        }
    }
}
