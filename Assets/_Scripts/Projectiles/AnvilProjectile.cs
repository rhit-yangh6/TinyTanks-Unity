using System;
using _Scripts.Buffs;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AnvilProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private ScriptableBuff stunnedBuff;
        
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
        private Rigidbody2D _rb;
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (!_isActivated)
            {
                Vector2 velocity = _rb.velocity;
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            if (Input.GetMouseButtonDown(0) && _isActivated && Level == 5 && !_isSecondPhaseActivated)
            {
                _rb.AddForce(Vector2.down * 30.0f);
                _isSecondPhaseActivated = true;
            }
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                _rb.velocity = Vector2.zero;
                _rb.gravityScale *= _gravityScaleMultiplier;
                transform.rotation = Quaternion.identity;
            }
        }

        public override void Detonate()
        {
            Vector2 pos = transform.position;

            float damageDealt = _isActivated ? (_isSecondPhaseActivated ? 
                Damage * _secondPhaseFallDamageMultiplier * _fallDamageMultiplier :
                Damage * _fallDamageMultiplier) : Damage;

            if (Level == 6 && _isActivated)
            {
                DamageHandler.i.HandleDamage(pos, Radius, damageDealt, DamageHandler.DamageType.Circular, 
                    false, stunnedBuff);    
            }
            else DamageHandler.i.HandleDamage(pos, Radius, damageDealt, DamageHandler.DamageType.Circular);

            TerrainDestroyer.instance.DestroyTerrainCircular(pos,
                (Level >= 4 && _isActivated) ? Radius * 1.5f : Radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
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
