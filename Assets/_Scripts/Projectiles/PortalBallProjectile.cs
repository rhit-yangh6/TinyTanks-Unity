using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class PortalBallProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject portalBluePrefab, portalOrangePrefab, portalMirror1Prefab, portalMirror2Prefab;
        [SerializeField] private Sprite orangePortalBall, mirrorPortalBall;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _maximumDistance, _mirrorMultiplier, _portalDamage;
        private static int _circleSteps;

        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.25f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
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
        private LineRenderer _circleRenderer;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
            _sr = GetComponent<SpriteRenderer>();
            _circleRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            // Calculate MaximumDistance
            var finalMaximumDistance = Level switch
            {
                >= 2 => _maximumDistance * 1.5f,
                1 => _maximumDistance,
                _ => throw new ArgumentOutOfRangeException(nameof(Level), Level, null)
            };

            if (Level < 6)
            {
                DrawCircle(_circleSteps, finalMaximumDistance);
            }
            else
            {
                _circleRenderer.enabled = false;
            }
            var velocity = _rb.velocity;
            transform.Rotate(0,0, velocity.x > 0 ? -1 : 1);

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                var targetLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var distance = Vector2.Distance(transform.position, targetLocation);
                // Check Distance
                if (distance <= finalMaximumDistance || Level == 6)
                {
                    _isActivated = true;
                    _velocity = velocity;
                    _circleRenderer.enabled = false;
                    _originalLocation = transform.position;
                    _teleportLocation = targetLocation;
                    StartCoroutine(TeleportBall());
                }
            }
        }
        
        private IEnumerator TeleportBall()
        {
            // Spawn Portals
            var portalAngle = Vector2.Angle(Vector2.right, _velocity);
            GameObject bluePortal;
            GameObject orangePortal;

            if (Level != 5)
            {
                bluePortal = Instantiate(portalBluePrefab,
                    _originalLocation, Quaternion.Euler(0, 0, portalAngle));
                orangePortal = Instantiate(portalOrangePrefab,
                    _teleportLocation, Quaternion.Euler(0, 0, portalAngle));
            }
            else
            {
                bluePortal = Instantiate(portalMirror1Prefab,
                    _originalLocation, Quaternion.Euler(0, 0, portalAngle));
                orangePortal = Instantiate(portalMirror2Prefab,
                    _teleportLocation, Quaternion.Euler(0, 0, portalAngle));
            }
            
            // Deal Portal Damage
            if (Level >= 4)
            {
                DamageHandler.i.HandleDamage(_originalLocation, Radius, _portalDamage, DamageHandler.DamageType.Circular);
                DamageHandler.i.HandleDamage(_teleportLocation, Radius, _portalDamage, DamageHandler.DamageType.Circular);
            }
            
            _r.enabled = false;
            _rb.gravityScale = 0;
            _rb.velocity = Vector2.zero;

            yield return new WaitForSeconds(1);

            _sr.sprite = Level == 5 ? mirrorPortalBall : orangePortalBall;
            _r.enabled = true;
            _rb.gravityScale = 1;
            transform.position = _teleportLocation;
            _rb.velocity = Level == 5 ? -_velocity : _velocity;
            
            yield return new WaitForSeconds(0.5f);
            
            Destroy(bluePortal);
            Destroy(orangePortal);
        }
        
        public override void Detonate()
        {
            var pos = transform.position;

            var finalRadius = Level == 5 ? Radius * _mirrorMultiplier : Radius;
            
            DamageHandler.i.HandleDamage(pos, finalRadius, Level == 5 ? Damage * _mirrorMultiplier : Damage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                finalRadius, 1, DestroyTypes.Circular);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        private void DrawCircle(int steps, float radius)
        {
            _circleRenderer.positionCount = steps;
            for (var currentStep = 0; currentStep < steps; currentStep++)
            {
                var circumferenceProgress = (float)currentStep / steps;

                var currentRadian = circumferenceProgress * 2 * Mathf.PI;

                var xScaled = Mathf.Cos(currentRadian);
                var yScaled = Mathf.Sin(currentRadian);

                var x = xScaled * radius;
                var y = yScaled * radius;
                var pos = gameObject.transform.position;

                var currentPosition = new Vector3(pos.x + x, pos.y + y, 0);
                _circleRenderer.SetPosition(currentStep, currentPosition);
            }
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
            
            _maximumDistance = Array.Find(extraWeaponTerms, ewt => ewt.term == "maximumDistance").value;
            _circleSteps = (int)Array.Find(extraWeaponTerms, ewt => ewt.term == "circleSteps").value;
            _mirrorMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "mirrorMultiplier").value;
            _portalDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "portalDamage").value;
        }
        
    }
}