using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class GearProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject gearBackgroundPrefab;
        [SerializeField] private float extraUnitDamageMultiplier = 0.1f;
        [SerializeField] private MMFeedbacks spinMmFeedbacks;
        [SerializeField] private float gearSpinDamage = 10f;
        [SerializeField] private float gearExplosionDamage = 25f;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        
        // References
        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    5 => damage * 1.3f,
                    _ => damage
                };
            }
        }

        protected override float MaxMagnitude
        {
            get
            {
                return Level switch
                {
                    >= 2 => maxMagnitude * 1.2f,
                    _ => maxMagnitude
                };
            }
        }

        private int MaximumGears
        {
            get
            {
                return Level switch
                {
                    5 => 12,
                    >= 3 => 7,
                    _ => 5
                };
            }
        }
        
        // Other Variables
        private int _gearNumber;
        private bool _isActivated;
        private GearExtraData _gearExtraData;
        private static readonly int OverclockIndex = Animator.StringToHash("Overclock");
        private static readonly int SpinIndex = Animator.StringToHash("Spin");

        private void Start()
        {
            _gearExtraData = (GearExtraData) WeaponExtraData;
            _gearExtraData.cap = MaximumGears;
        }

        private void Update()
        {
            Spin(1.3f);
            if (!Input.GetMouseButtonDown(0) || _isActivated || Level != 6) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
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
                gearObject.GetComponent<Animator>().SetTrigger(_isActivated ? OverclockIndex : SpinIndex);
                if (Level >= 4)
                {
                    DamageHandler.i.HandleDamage(gearObject.transform.position, Radius,
                        gearSpinDamage, DamageHandler.DamageType.Circular);
                }
            }
            
            _gearNumber = Math.Min(gearObjects.Length, MaximumGears);
            if (_gearNumber == 0)
            {
                defaultMmFeedbacks.PlayFeedbacks();
                Instantiate(gearBackgroundPrefab, pos, Quaternion.identity);
                
                // Update External Display
                _gearExtraData.AddOneGear();
                EventBus.Broadcast(EventTypes.ExternalDisplayChange, _gearExtraData);
                
                DealDamage();
                yield break;
            }
            
            spinMmFeedbacks.PlayFeedbacks();
            
            yield return new WaitForSeconds(2.0f);
            DealDamage();

            if (_isActivated)
            {
                foreach (var gearObject in gearObjects)
                {
                    var gearPos = gearObject.transform.position;
                    Instantiate(GameAssets.i.explosionFX, gearPos, Quaternion.identity, gearObject.transform);
                    EventBus.Broadcast(EventTypes.DestroyTerrain, gearPos, Radius, 1, 
                        DestroyTypes.Circular);
                    DamageHandler.i.HandleDamage(gearPos, Radius,
                        gearExplosionDamage, DamageHandler.DamageType.Circular);
                    Destroy(gearObject, 0.3f);
                }
                
                // Update External Display
                _gearExtraData.ClearAllGears();
                EventBus.Broadcast(EventTypes.ExternalDisplayChange, _gearExtraData);
            }
            
            Instantiate(gearBackgroundPrefab, pos, Quaternion.identity);
            
            // Update External Display
            _gearExtraData.AddOneGear();
            EventBus.Broadcast(EventTypes.ExternalDisplayChange, _gearExtraData);
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