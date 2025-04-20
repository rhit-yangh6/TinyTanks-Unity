using System;
using _Scripts.Buffs;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CannonProjectile: LaunchedProjectile
    {
        // References
        protected override float Damage => Level == 6 ? 99f : damage;

        private void Update()
        {
            Spin();
        }
    }
}