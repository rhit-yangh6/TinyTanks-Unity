using System.Collections;
using _Scripts.Utils;
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
            _initialVelocity = rigidBody2D.linearVelocity;
            var newVelocity = Geometry.Rotate(_initialVelocity, CalculateBulletDispersion());
            rigidBody2D.linearVelocity = newVelocity * velocityMultiplier;
            _initialPosition = transform.position;
            StartCoroutine(SpawnSecondaryProjectiles());
        }

        private IEnumerator SpawnSecondaryProjectiles()
        {
            var oldInfo = Disappear();
            // Minigun wind-up time
            yield return new WaitForSeconds(windUpSpeed);
            Reappear(oldInfo);
            Instantiate(bulletShellPrefab, _initialPosition, Quaternion.identity);
            
            for (var i = 0; i < Bullets - 1; i++)
            {
                if (Level >= 4) _bulletDamage += damageIncrease;
                yield return new WaitForSeconds(0.2f);
                SpawnBullet();
            }

            if (Level == 6)
            {
                yield return new WaitForSeconds(1f);
                for (var i = 0; i < Bullets; i++)
                {
                    _bulletDamage += damageIncrease;
                    yield return new WaitForSeconds(0.2f);
                    SpawnBullet();
                }
            }
        }

        private void SpawnBullet()
        {
            var velocity = Geometry.Rotate(_initialVelocity, CalculateBulletDispersion()) * velocityMultiplier;
            SpawnDerived(secondaryBulletPrefab, _initialPosition, _bulletDamage, Radius, velocity);
            Instantiate(bulletShellPrefab, _initialPosition, Quaternion.identity);
        }

        // Main minigun projectile doesn't deal damage — only spawned bullets do
        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
        }

        private void Update() { Direct(); }

        private float CalculateBulletDispersion()
        {
            return Random.Range(-BulletDispersion, BulletDispersion);
        }
    }
}
