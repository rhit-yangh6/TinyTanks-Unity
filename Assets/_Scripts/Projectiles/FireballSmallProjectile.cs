using _Scripts.Buffs;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FireballSmallProjectile: DerivedProjectile
    {
        [SerializeField] private ScriptableBuff burningBuff;
        
        // Shared Fields
        private static float _radius, _damage,_explosionDuration;
        private static GameObject _explosionFX;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public override void SetParameters(float damage, float radius, float explosionDuration, GameObject explosionFX)
        {
            _radius = radius;
            _damage = damage;
            _explosionDuration = explosionDuration;
            _explosionFX = explosionFX;
        }
        
        public override void Detonate()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                false, burningBuff);

            TerrainDestroyer.instance.DestroyTerrainCircular(pos, Radius);
        
            SpawnExplosionFX();
            DoCameraShake();
            
            Destroy(gameObject);
        }
    }
}