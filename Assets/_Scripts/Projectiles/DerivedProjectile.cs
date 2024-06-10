using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

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