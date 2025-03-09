using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ShotgunSecondaryProjectile : DerivedProjectile
    {
        protected override void Start()
        {
            base.Start();
            StartCoroutine(TemporarilyDisableCollider());
        }
    }
}
