using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomerangProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _travelBackTime, _proximityDetectRange;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated;
        private Rigidbody2D _rb;

        // TODO: Save Shooter (For All Projectiles) ?

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            
            // TODO: UPGRADE ADDS STEP & REMOTE & NO-SELF-DAMAGE
            /*
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                
                Vector2 startPos = transform.position, endPos = _playerTransform.position;
            
                float t = _travelBackTime;
q
                float vx = (endPos.x - startPos.x) / t;
                float vy = (endPos.y - startPos.y + 0.5f * -Physics2D.gravity.magnitude * t * t) / t;
                
                _rb.gravityScale = -1;
                _rb.velocity = new Vector2(vx, vy);
            }


            if (_isActivated)
            {
                Vector2 startPos = transform.position, endPos = _playerTransform.position;

                if (Vector2.Distance(startPos, endPos) <= _proximityDetectRange)
                {
                    Destroy(gameObject);
                }
            }
            */
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
