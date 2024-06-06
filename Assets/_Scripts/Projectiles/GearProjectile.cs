using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class GearProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject gearBackgroundPrefab;
        [SerializeField] private int maximumGears = 5;
        [SerializeField] private float extraUnitDamageMultiplier = 0.1f;
        [SerializeField] private MMFeedbacks spinMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        private static readonly int SpinIndex = Animator.StringToHash("Spin");

        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private int _gearNumber;

        private void Update()
        {
            Spin();
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();
            
            StartCoroutine(SpinGearsAndSpawnNewOne());
        }

        private IEnumerator SpinGearsAndSpawnNewOne()
        {
            var pos = transform.position;
            var gearObjects = GameObject.FindGameObjectsWithTag("Prop-Gear");
            foreach (var gearObject in gearObjects)
            {
                gearObject.GetComponent<Animator>().SetTrigger(SpinIndex);
            }
            
            _gearNumber = Math.Min(gearObjects.Length, maximumGears);
            if (_gearNumber == 0)
            {
                defaultMmFeedbacks.PlayFeedbacks();
                Instantiate(gearBackgroundPrefab, pos, Quaternion.identity);
                DealDamage();
                yield break;
            }
            
            spinMmFeedbacks.PlayFeedbacks();
            
            yield return new WaitForSeconds(2.0f);
            DealDamage();
            
            Instantiate(gearBackgroundPrefab, pos, Quaternion.identity);
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage * (1 + _gearNumber * extraUnitDamageMultiplier),
                DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Circular);
        }
        
        protected override void Disappear()
        {
            // Stop rigidBody from moving/rotating
            Rigidbody2D.gravityScale = 0;
            Rigidbody2D.freezeRotation = true;
            Rigidbody2D.velocity = Vector2.zero;

            // Disable collider
            Collider2D.enabled = false;
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
        }
    }
}