using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public interface IProjectile
    {
        void Detonate();
        void Activate();
        void DealDamage();
        void Spin(float spinSpeed);
        void Direct();
        Tuple<Vector2, float> Disappear();
        void Reappear(Tuple<Vector2, float> oldInfo);
        IEnumerator TemporarilyDisableCollider();
    }
}
