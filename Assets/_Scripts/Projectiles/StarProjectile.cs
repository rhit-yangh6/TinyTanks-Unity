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
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _damageMultiplier, _radiusMultiplier, _drawStarSpeed, _drawStarInterval, _shockwaveRadius, _shockwaveDamage;
        
        // References
        protected override float Radius => Level >= 2 ? _radius * 1.3f : _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.3f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;

        private float FinalRadiusMultiplier
        {
            get
            {
                return Level switch
                {
                    5 => _radiusMultiplier * 1.3f,
                    >= 4 => _radiusMultiplier * 1.2f,
                    _ => _radiusMultiplier
                };
            }
        }
        
        private float FinalDamageMultiplier
        {
            get
            {
                return Level switch
                {
                    5 => _damageMultiplier * 1.3f,
                    >= 4 => _damageMultiplier * 1.2f,
                    _ => _damageMultiplier
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
            var initialVelocity = Rigidbody2D.velocity;
            Rigidbody2D.gravityScale = 0;
            Rigidbody2D.velocity = Vector2.zero;
            _tr.emitting = true;

            Rigidbody2D.velocity = (Vector2.left + Vector2.down) * _drawStarSpeed;
            
            yield return new WaitForSeconds(Level >= 3 ? _drawStarInterval * 0.75f : _drawStarInterval);

            Vector2 pos;
            GameObject insExpl;
            if (Level == 6)
            {
                pos = gameObject.transform.position;
                insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
                insExpl.GetComponent<ShockwaveManager>().CallShockwave(ExplosionDuration, 0.09f);
                Destroy(insExpl, ExplosionDuration);
                DamageHandler.i.HandleDamage(pos, _shockwaveRadius, _shockwaveDamage, DamageHandler.DamageType.Circular);
            }

            for (var i = 0; i < 4; i++)
            {
                Rigidbody2D.velocity = Geometry.Rotate(Rigidbody2D.velocity, RotateDegree);
                yield return new WaitForSeconds(Level >= 3 ? _drawStarInterval * 0.75f : _drawStarInterval);
                if (Level != 6) continue;
                pos = gameObject.transform.position;
                insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
                insExpl.GetComponent<ShockwaveManager>().CallShockwave(ExplosionDuration, 0.09f);
                Destroy(insExpl, ExplosionDuration);
                DamageHandler.i.HandleDamage(pos, _shockwaveRadius, _shockwaveDamage, DamageHandler.DamageType.Circular);
            }
            
            Rigidbody2D.velocity = initialVelocity;
            Rigidbody2D.gravityScale = 1;
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
                _isStarComplete ? Radius * FinalRadiusMultiplier : Radius, 1, DestroyTypes.Circular);
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

            _damageMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "damageMultiplier").value;
            _radiusMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "radiusMultiplier").value;
            
            _drawStarSpeed = Array.Find(extraWeaponTerms, ewt => ewt.term == "drawStarSpeed").value;
            
            _drawStarInterval = Array.Find(extraWeaponTerms, ewt => ewt.term == "drawStarInterval").value;
            
            _shockwaveRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "shockwaveRadius").value;
            _shockwaveDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "shockwaveDamage").value;
        }
    }
}