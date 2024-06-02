using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class AirstrikeProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float missileDeviateAngle = 3f;
        [SerializeField] private float carpetBombingMultiplier = 0.7f;
        [SerializeField] private float missilePosDeviation = 1.5f;
        [SerializeField] private MMFeedbacks carpetBombingMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // References
        protected override float Radius => Level >= 3 ? _radius * 1.2f : _radius;
        protected override float Damage => Level >= 4 ? _damage * 1.25f : _damage;
        protected override float MaxMagnitude => Level >= 2 ? _maxMagnitude * 1.1f : _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.1f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        private float MissileDeviateAngle
        {
            get
            {
                return Level switch
                {
                    >= 2 => 0,
                    _ => missileDeviateAngle
                };
            }
        }

        // Other Variables
        private const float XOffset = -10f;
        private const float YOffset = -2f;
        private const float MissileSpeed = 15f;

        public override void Detonate()
        {
            if (isDetonated)
            {
                return;
            }
            isDetonated = true;
            Disappear();
            
            if (Level == 5)
            {
                carpetBombingMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public void SpawnMissile()
        {
            InstantiateMissile(false, 0);
        }

        public void SpawnCarpetBombingMiddleMissile()
        {
            InstantiateMissile(true, 0);
        }
        
        public void SpawnCarpetBombingLeftMissile()
        {
            InstantiateMissile(true, -1);
        }
        
        public void SpawnCarpetBombingRightMissile()
        {
            InstantiateMissile(true, 1);
        }

        private void InstantiateMissile(bool isCarpetBombing, int position)
        {
            var targetPos = transform.position;
            Vector2 pos = position switch
            {
                -1 => new Vector2(targetPos.x - missilePosDeviation, targetPos.y),
                1 => new Vector2(targetPos.x + missilePosDeviation, targetPos.y),
                _ => targetPos
            };

            // RayCast to sky
            var hit = Physics2D.Raycast(pos, Vector2.up, 1000, layerMask);
            
            // Calculate missile spawn point
            var missilePos = new Vector2(hit.point.x + XOffset +
                                        Random.Range(-missilePosDeviation, missilePosDeviation),
                hit.point.y + YOffset);
            
            // Calculate missile Velocity
            var missileVelocity = Geometry.Rotate((pos - missilePos).normalized,
                Random.Range(-MissileDeviateAngle, MissileDeviateAngle)) * MissileSpeed;
            
            // Instantiate Missile
            var derivedObject = Instantiate(missilePrefab, missilePos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<AirstrikeMissileProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();

            if (isCarpetBombing)
            {
                derivedProjectile.SetParameters(Damage * carpetBombingMultiplier,
                    Radius * carpetBombingMultiplier, ExplosionDuration, ExplosionFX);
            }
            else
            {
                derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            }

            if (Level == 6)
            {
                derivedProjectile.SetIsSplitting();
            }
            derivedProjectile.SetTargetPos(pos);
            derivedRb2d.velocity = missileVelocity;
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude,
            int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.regularExplosionFX;
        }
    }
}