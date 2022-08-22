using System.Collections;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class PortalBallProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject portalBluePrefab, portalOrangePrefab;
        [SerializeField] private Sprite orangePortalBall;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => Level >= 2 ? _maxMagnitude * 1.2f : _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private bool _isActivated;
        private Vector2 _velocity;
        private Vector2 _originalLocation, _teleportLocation;
        private Renderer _r;
        private SpriteRenderer _sr;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate (0,0, velocity.x > 0 ? -1 : 1);

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                _velocity = velocity;
                _originalLocation = transform.position;
                _teleportLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                StartCoroutine(TeleportBall());
            }
        }
        
        private IEnumerator TeleportBall()
        {
            // Spawn Portals
            var portalAngle = Vector2.Angle(Vector2.right, _velocity);
            var bluePortal = Instantiate(portalBluePrefab,
                _originalLocation, Quaternion.Euler(0, 0, portalAngle));
            var orangePortal = Instantiate(portalOrangePrefab,
                _teleportLocation, Quaternion.Euler(0, 0, portalAngle));
            
            _r.enabled = false;
            _rb.gravityScale = 0;
            _rb.velocity = Vector2.zero;

            yield return new WaitForSeconds(1);

            _sr.sprite = orangePortalBall;
            _r.enabled = true;
            _rb.gravityScale = 1;
            transform.position = _teleportLocation;
            _rb.velocity = _velocity;
            
            yield return new WaitForSeconds(0.5f);
            
            Destroy(bluePortal);
            Destroy(orangePortal);
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

            //_boulderPieceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceDamage").value;
            //_boulderPieceRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "boulderPieceRadius").value;
        }
        
    }
}