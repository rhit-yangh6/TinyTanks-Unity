using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ShotgunSecondaryProjectile : DerivedProjectile
    {
        private void Start()
        {
            StartCoroutine(TemporarilyDisableCollider());
        }
    }
}
