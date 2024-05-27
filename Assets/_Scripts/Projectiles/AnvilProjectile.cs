using System;
using _Scripts.Buffs;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AnvilProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks slamMmFeedbacks;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private MMFeedbacks secondActivateFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _gravityScaleMultiplier, _fallDamageMultiplier, _secondPhaseFallDamageMultiplier;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 2 ? _damage * 1.1f : _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.1f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated, _isSecondPhaseActivated;

        private void Start()
        {
            foreach (var ps in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }
        }
        
        private void Update()
        {
            if (!_isActivated)
            {
                Direct();
            }
            
            if (Input.GetMouseButtonDown(0) && _isActivated && Level == 5 && !_isSecondPhaseActivated)
            {
                _isSecondPhaseActivated = true;
                secondActivateFeedbacks.PlayFeedbacks();
            }
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            Rigidbody2D.velocity = Vector2.zero;
            Rigidbody2D.gravityScale *= _gravityScaleMultiplier;
            transform.rotation = Quaternion.identity;
        }

        public void SecondActivate()
        {
            Rigidbody2D.AddForce(Vector2.down * 30.0f);
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            
            if (_isActivated)
            {
                slamMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
            DealDamage();
        }

        public override void DealDamage()
        {
            var pos = transform.position;

            var damageDealt = _isActivated ? (_isSecondPhaseActivated ? 
                Damage * _secondPhaseFallDamageMultiplier * _fallDamageMultiplier :
                Damage * _fallDamageMultiplier) : Damage;

            if (Level == 6 && _isActivated)
            {
                DamageHandler.i.HandleDamage(pos, Radius, damageDealt, DamageHandler.DamageType.Circular, 
                    false, GameAssets.i.stunnedBuff);    
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, damageDealt, DamageHandler.DamageType.Circular);
            }
            
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                (Level >= 4 && _isActivated) ? Radius * 1.5f : Radius, 1, DestroyTypes.Circular);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            _gravityScaleMultiplier = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "gravityScaleMultiplier").value;
            _fallDamageMultiplier = Array.Find(extraWeaponTerms, ewt =>
                ewt.term == "fallDamageMultiplier").value;
            _secondPhaseFallDamageMultiplier = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "secondPhaseFallDamageMultiplier").value;
        }
    }
}
