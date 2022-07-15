using UnityEngine;

namespace _Scripts.Projectiles
{
    public class ShotgunProjectile : LaunchedProjectile
    {
        // Set In Inspector        
        [SerializeField] private GameObject shotgunSecondaryPrefab;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        
        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            var velocity = _rb.velocity;
            
            // Make clones
            GameObject derivedObject = Instantiate(shotgunSecondaryPrefab, transform.position, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Rotate(velocity, 10f);
            
            derivedObject = Instantiate(shotgunSecondaryPrefab, transform.position, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Rotate(velocity, -10f);

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
        }
        
        private Vector2 Rotate(Vector2 v, float delta)
        {
            float deltaRad = delta * Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(deltaRad) - v.y * Mathf.Sin(deltaRad),
                v.x * Mathf.Sin(deltaRad) + v.y * Mathf.Cos(deltaRad)
            );
        }
        
    }
}
