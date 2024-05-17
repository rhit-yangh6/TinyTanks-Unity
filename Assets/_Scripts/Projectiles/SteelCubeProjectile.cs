using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SteelCubeProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject smallCubePrefab;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _smallCubeDamage, _smallCubeRadius, _smallCubeAngleDelta;
        
        // References
        protected override float Radius => Level >= 2 ? _radius * 1.2f : _radius;
        protected override float Damage
        {
            get
            {
                if (Level == 5) return _damage * 1.9f; // Level 5
                return Level >= 4 ? _damage * 1.25f : _damage; // Level 4
            }
        }

        protected override float MaxMagnitude => Level == 5 ? _maxMagnitude * 0.6f : _maxMagnitude; // Level 5
        protected override int Steps => _steps;
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
            transform.Rotate(0,0, _rb.velocity.x > 0 ? -1 : 1);
        }

        public override void Detonate()
        {
            var pos = transform.position;
            
            var isCritical = false;
            if (Level >= 3) isCritical = Random.value > 0.70; // Level 3

            DamageHandler.i.HandleDamage(pos, Radius, isCritical ? Damage * 1.5f : Damage,
                DamageHandler.DamageType.Square, isCritical);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, Level == 5 ? 2 : 1, DestroyTypes.Square);
        
            SpawnExplosionFX();
            DoCameraShake();

            if (Level == 6) // Level 6
            {
                for (var i = 0; i < 4; i++)
                {
                    var derivedObject = Instantiate(smallCubePrefab, pos, Quaternion.identity);
                    var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                    var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
                    
                    derivedProjectile.SetParameters(_smallCubeDamage, _smallCubeRadius, ExplosionDuration, ExplosionFX);
                    
                    var rotateDegree = Random.Range(-_smallCubeAngleDelta, _smallCubeAngleDelta);
                    var speed = Random.Range(5.5f, 9f);
                    derivedRb2d.velocity = Geometry.Rotate(Vector2.up, rotateDegree) * speed;
                }
            }

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
            
            _explosionFX = GameAssets.i.squareExplosionFX;

            _smallCubeDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "smallCubeDamage").value;
            _smallCubeRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "smallCubeRadius").value;
            _smallCubeAngleDelta = Array.Find(extraWeaponTerms, ewt => ewt.term == "smallCubeAngleDelta").value;
        }
        
    }
}