using System;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AirstrikeSmallMissileProjectile : DerivedProjectile
    {
        private void Start()
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }

        private void Update()
        {
            Direct();
        }
    }
}