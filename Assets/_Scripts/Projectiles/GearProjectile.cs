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
        
        private static readonly int SpinIndex = Animator.StringToHash("Spin");

        // References
        protected override float Radius => radius;
        protected override float Damage => damage;
        protected override float MaxMagnitude => maxMagnitude;
        protected override int Steps => steps;
        
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
        
        public override void Disappear()
        {
            // Stop rigidBody from moving/rotating
            rigidBody2D.gravityScale = 0;
            rigidBody2D.freezeRotation = true;
            rigidBody2D.velocity = Vector2.zero;

            // Disable collider
            collider2D.enabled = false;
        }
    }
}