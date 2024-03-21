using System;
using _Scripts.Buffs;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CurseBombProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private ScriptableBuff cursedBuff;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _boulderPieceRadius, _boulderPieceDamage;
        
        // References
        protected override float Radius => Level >= 4 ? _radius * 1.4f : _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => Level >= 3 ? _maxMagnitude * 1.4f : _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.4f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate(0, 0, velocity.x > 0 ? -1 : 1);
        }

        public override void Detonate()
        {
            Vector2 pos = transform.position;

            var cursedBuffLevel = Level switch
            {
                5 => 2,
                6 => 3,
                _ => 1
            };

            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                false, cursedBuff, cursedBuffLevel);

            // TerrainDestroyer.Instance.DestroyTerrain(pos, Radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            // TODO: new fx
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;

            // _boulderPieceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceDamage").value;
            // _boulderPieceRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceRadius").value;
        }
    }
}