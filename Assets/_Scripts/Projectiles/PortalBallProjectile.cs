using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class PortalBallProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject portalBluePrefab, portalOrangePrefab, portalMirror1Prefab, portalMirror2Prefab;
        [SerializeField] private Sprite orangePortalBall, mirrorPortalBall;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;

        [SerializeField] private float maximumDistance = 10.0f;
        [SerializeField] private float mirrorMultiplier = 1.6f;
        [SerializeField] private float portalDamage = 10f;
        [SerializeField] private int circleSteps = 100;

        // References
        protected override float Damage => Level >= 3 ? damage * 1.25f : damage;
        private float MaximumDistance
        {
            get
            {
                return Level switch
                {
                    >= 2 => maximumDistance * 1.5f,
                    _ => maximumDistance,
                };
            }
        }
        
        // Other Variables
        private bool _isActivated;
        private Vector2 _velocity;
        private Vector2 _originalLocation, _teleportLocation;
        private SpriteRenderer _sr;
        private LineRenderer _circleRenderer;

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            _circleRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            Spin();

            if (Level < 6)
            {
                DrawCircle(circleSteps, MaximumDistance);
            }
            else
            {
                _circleRenderer.enabled = false;
            }

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                var targetLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var distance = Vector2.Distance(transform.position, targetLocation);
                // Check Distance
                if (distance <= MaximumDistance || Level == 6)
                {
                    _isActivated = true;
                    _velocity = GetComponent<Rigidbody2D>().velocity;
                    _circleRenderer.enabled = false;
                    _originalLocation = transform.position;
                    _teleportLocation = targetLocation;
                    activateMmFeedbacks.PlayFeedbacks();
                }
            }
        }

        public override void Activate()
        {
            StartCoroutine(TeleportBall());
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
                DamageHandler.i.HandleDamage(_originalLocation, Radius,
                    portalDamage, DamageHandler.DamageType.Circular);
                DamageHandler.i.HandleDamage(_teleportLocation, Radius,
                    portalDamage, DamageHandler.DamageType.Circular);
            }
            
            renderer.enabled = false;
            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;

            yield return new WaitForSeconds(1);

            _sr.sprite = Level == 5 ? mirrorPortalBall : orangePortalBall;
            renderer.enabled = true;
            rigidBody2D.gravityScale = 1;
            transform.position = _teleportLocation;
            rigidBody2D.velocity = Level == 5 ? -_velocity : _velocity;
            
            yield return new WaitForSeconds(0.5f);
            
            Destroy(bluePortal);
            Destroy(orangePortal);
        }

        public override void DealDamage()
        {
            var pos = transform.position;

            var finalRadius = Level == 5 ? Radius * mirrorMultiplier : Radius;
            
            DamageHandler.i.HandleDamage(pos, finalRadius, Level == 5 ? Damage * mirrorMultiplier : Damage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                finalRadius, 1, DestroyTypes.Circular);
        }

        private void DrawCircle(int circleStepCount, float circleRadius)
        {
            _circleRenderer.positionCount = circleStepCount;
            for (var currentStep = 0; currentStep < circleStepCount; currentStep++)
            {
                var circumferenceProgress = (float)currentStep / circleStepCount;

                var currentRadian = circumferenceProgress * 2 * Mathf.PI;

                var xScaled = Mathf.Cos(currentRadian);
                var yScaled = Mathf.Sin(currentRadian);

                var x = xScaled * circleRadius;
                var y = yScaled * circleRadius;
                var pos = gameObject.transform.position;

                var currentPosition = new Vector3(pos.x + x, pos.y + y, 0);
                _circleRenderer.SetPosition(currentStep, currentPosition);
            }
        }
        
    }
}