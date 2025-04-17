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
        [SerializeField] private float windUpSpeed = 2f;
        [SerializeField] private float damageIncrease = 1f;
        [SerializeField] private GameObject secondaryBulletPrefab;
        [SerializeField] private GameObject bulletShellPrefab;
        // References

        private float BulletDispersion
        {
            get
            {
                return Level switch
                {
                    5 => 10,
                    >= 2 => 5,
                    _ => 7
                };
            }
        }
        private int Bullets
        {
            get
            {
                return Level switch
                {
                    6 => 6,
                    5 => 10,
                    >= 3 => 7,
                    _ => 6
                };
            }
        }
        
        // Other Variables
        private Vector2 _initialVelocity;
        private Vector2 _initialPosition;
        private float _bulletDamage;
        protected override void Start()
        {
            base.Start();
            _bulletDamage = Damage;
            _initialVelocity = rigidBody2D.velocity;
            var newVelocity = Geometry.Rotate(_initialVelocity, CalculateBulletDispersion());
            rigidBody2D.velocity = newVelocity * velocityMultiplier;
            _initialPosition = transform.position;
            StartCoroutine(SpawnSecondaryProjectiles());
        }

        private IEnumerator SpawnSecondaryProjectiles()
        {
            var oldInfo = Disappear();
            // Minigun wind-up time
            yield return new WaitForSeconds(windUpSpeed);
            Reappear(oldInfo);
            SpawnBulletShell();
            
            for (var i = 0; i < Bullets - 1; i++)
            {
                if (Level >= 4)
                {
                    _bulletDamage += damageIncrease;
                }
                yield return new WaitForSeconds(0.2f);
                var derivedObject = Instantiate(secondaryBulletPrefab, _initialPosition, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                var derivedRigidBody2D = derivedObject.GetComponent<Rigidbody2D>();
                
                derivedProjectile.SetParameters(_bulletDamage, Radius);
                derivedRigidBody2D.velocity = Geometry.Rotate(_initialVelocity, 
                                                  CalculateBulletDispersion()) * velocityMultiplier;
                SpawnBulletShell();
            }

            if (Level == 6)
            {
                yield return new WaitForSeconds(1f);
                for (var i = 0; i < Bullets; i++)
                {
                    _bulletDamage += damageIncrease;
                    yield return new WaitForSeconds(0.2f);
                    var derivedObject = Instantiate(secondaryBulletPrefab, _initialPosition, Quaternion.identity);
                    var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                    var derivedRigidBody2D = derivedObject.GetComponent<Rigidbody2D>();
                
                    derivedProjectile.SetParameters(_bulletDamage, Radius);
                    derivedRigidBody2D.velocity = Geometry.Rotate(_initialVelocity, 
                        CalculateBulletDispersion()) * velocityMultiplier;
                    SpawnBulletShell();
                }
            }
        }

        private void SpawnBulletShell()
        {
            Instantiate(bulletShellPrefab, _initialPosition, Quaternion.identity);
        }

        private void Update() { Direct(); }

        private float CalculateBulletDispersion()
        {
            return Random.Range(-BulletDispersion, BulletDispersion);
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
