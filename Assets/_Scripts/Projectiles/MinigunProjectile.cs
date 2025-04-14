using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class MinigunProjectile : LaunchedProjectile
    {
        [SerializeField] private float velocityMultiplier = 1.5f;
        [SerializeField] private float bulletDispersion = 7f;
        [SerializeField] private GameObject secondaryBulletPrefab;
        // References
        
        // Other Variables
        private Vector2 _initialVelocity;
        protected override void Start()
        {
            base.Start();
            _initialVelocity = rigidBody2D.velocity;
            var newVelocity = Geometry.Rotate(_initialVelocity, CalculateBulletDispersion());
            rigidBody2D.velocity = newVelocity * velocityMultiplier;
            StartCoroutine(SpawnSecondaryProjectiles());
        }

        private IEnumerator SpawnSecondaryProjectiles()
        {
            var velocity = rigidBody2D.velocity;
            var pos = transform.position;

            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(0.2f);
                var derivedObject = Instantiate(secondaryBulletPrefab, pos, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                var derivedRigidBody2D = derivedObject.GetComponent<Rigidbody2D>();
                
                derivedProjectile.SetParameters(Damage, Radius);
                derivedRigidBody2D.velocity = Geometry.Rotate(_initialVelocity, 
                                                  CalculateBulletDispersion()) * velocityMultiplier;
            }
        }

        private void Update() { Direct(); }

        private float CalculateBulletDispersion()
        {
            return Random.Range(-bulletDispersion, bulletDispersion);
        }

        // public override void DealDamage()
        // {
        //     var pos = transform.position;
        //
        //     var isCritical = false;
        //     if (Level >= 4) isCritical = Random.value > 0.75;
        //     if (Level == 6) isCritical = true;
        //
        //     DamageHandler.i.HandleDamage(pos, Radius, isCritical ? Damage * 1.5f : Damage, 
        //         DamageHandler.DamageType.Circular, isCritical);
        //
        //     if (Level >= 3) EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
        //         Radius, 1, DestroyTypes.Circular);
        // }
    }
}
