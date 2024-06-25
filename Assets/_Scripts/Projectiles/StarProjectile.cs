using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    // TODO: Change camera
    public class StarProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private Sprite novaStar;
        [SerializeField] private Material glowMaterial;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private float damageMultiplier = 1.7f;
        [SerializeField] private float radiusMultiplier = 1.5f;
        [SerializeField] private float drawStarSpeed = 16f;
        [SerializeField] private float drawStarInterval = 0.4f;
        [SerializeField] private float shockwaveRadius = 1.5f;
        [SerializeField] private float shockwaveDamage = 15f;
        [SerializeField] private float shockDuration = 0.25f;
        
        // References
        protected override float Radius => Level >= 2 ? radius * 1.3f : radius;
        protected override int Steps => Level >= 2 ? (int)(steps * 1.3f) : steps;

        private float FinalRadiusMultiplier
        {
            get
            {
                return Level switch
                {
                    5 => radiusMultiplier * 1.3f,
                    >= 4 => radiusMultiplier * 1.2f,
                    _ => radiusMultiplier
                };
            }
        }
        
        private float FinalDamageMultiplier
        {
            get
            {
                return Level switch
                {
                    5 => damageMultiplier * 1.3f,
                    >= 4 => damageMultiplier * 1.2f,
                    _ => damageMultiplier
                };
            }
        }
        
        // Other Variables
        private TrailRenderer _tr;
        private SpriteRenderer _sr;
        private ParticleSystem _ps;
        private bool _isActivated, _isStarComplete;
        private const float RotateDegree = -144f;
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

        private void Start()
        {
            _tr = GetComponent<TrailRenderer>();
            _sr = GetComponent<SpriteRenderer>();
            _ps = gameObject.GetComponentInChildren<ParticleSystem>();
            _ps.Stop();
        }
        
        private void Update()
        {
            if (!_isActivated) Spin();
            else Direct();
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            StartCoroutine(DrawStar());
        }

        private IEnumerator DrawStar()
        {
            var initialVelocity = rigidBody2D.velocity;
            rigidBody2D.gravityScale = 0;
            rigidBody2D.velocity = Vector2.zero;
            _tr.emitting = true;

            rigidBody2D.velocity = (Vector2.left + Vector2.down) * drawStarSpeed;
            
            yield return new WaitForSeconds(Level >= 3 ? drawStarInterval * 0.75f : drawStarInterval);

            Vector2 pos;
            GameObject insExpl;
            if (Level == 6)
            {
                pos = gameObject.transform.position;
                insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
                insExpl.GetComponent<ShockwaveManager>().CallShockwave(shockDuration, 0.09f);
                Destroy(insExpl, shockDuration);
                DamageHandler.i.HandleDamage(pos, shockwaveRadius, shockwaveDamage, DamageHandler.DamageType.Circular);
            }

            for (var i = 0; i < 4; i++)
            {
                rigidBody2D.velocity = Geometry.Rotate(rigidBody2D.velocity, RotateDegree);
                yield return new WaitForSeconds(Level >= 3 ? drawStarInterval * 0.75f : drawStarInterval);
                if (Level != 6) continue;
                pos = gameObject.transform.position;
                insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
                insExpl.GetComponent<ShockwaveManager>().CallShockwave(shockDuration, 0.09f);
                Destroy(insExpl, shockDuration);
                DamageHandler.i.HandleDamage(pos, shockwaveRadius, shockwaveDamage, DamageHandler.DamageType.Circular);
            }
            
            rigidBody2D.velocity = initialVelocity;
            rigidBody2D.gravityScale = 1;
            _tr.emitting = false;
            _ps.Play();
            _isStarComplete = true;

            if (Level == 5)
            {
                _sr.sprite = novaStar;
                _sr.material = glowMaterial;
                
                var elapsedTime = 0f;
                var lerpTime = 2f;
                var color = Color.white;
                while (elapsedTime < lerpTime)
                {
                    elapsedTime += Time.deltaTime;
                    
                    color *= 1.01f;
                    _sr.material.SetVector(GlowColor, color);
                }
            }
            yield return null;
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            
            DamageHandler.i.HandleDamage(
                pos, 
                _isStarComplete ? Radius * FinalRadiusMultiplier : Radius,
                _isStarComplete ? Damage * FinalDamageMultiplier : Damage, 
                DamageHandler.DamageType.Circular);
            
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                _isStarComplete ? Radius * FinalRadiusMultiplier : Radius, 1,  _isStarComplete ?
                    DestroyTypes.Star : DestroyTypes.Circular);
        }
    }
}