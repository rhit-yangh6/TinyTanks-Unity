using System;
using System.Collections;
using _Scripts.Buffs;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FireballProjectile: LaunchedProjectile
    {
        // TODO: Different ExplosionFX?
        
        // Set in Inspector
        [SerializeField] private GameObject fireballSmallPrefab;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _summonInterval, _fireballSmallDamage, _fireballSmallRadius;
        
        // References
        protected override float Radius => Level >= 3 ? _radius * 1.3f : _radius;
        protected override float Damage => Level >= 2 ? _damage * 1.1f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Vars
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (Level == 6) StartCoroutine(SummonSmallFireballs());
        }

        private IEnumerator SummonSmallFireballs()
        {
            while (true)
            {
                yield return new WaitForSeconds(_summonInterval);
                var derivedObject = Instantiate(fireballSmallPrefab, gameObject.transform.position, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
                
                derivedProjectile.SetParameters(_fireballSmallDamage, _fireballSmallRadius, ExplosionDuration, ExplosionFX);
                derivedRb2d.velocity = Vector2.down;
            }
        }

        private void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public override void Detonate()
        {
            var pos = transform.position;

            var burningBuffLevel = 2;
            
            if (Level >= 4) burningBuffLevel = 3;
            
            // Level 5: Eternal Fire
            if (Level == 5) burningBuffLevel = 4;

            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                false, GameAssets.i.burningBuff, burningBuffLevel);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
        
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

            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _summonInterval = Array.Find(extraWeaponTerms, ewt => ewt.term == "summonInterval").value;
            _fireballSmallDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "fireballSmallDamage").value;
            _fireballSmallRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "fireballSmallRadius").value;
        }
    }
}