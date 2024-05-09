using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class YinYangProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private Sprite blackSprite, whiteSprite;
        
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
        
        // ExtraFields
        private static float _whiteDamage, _whiteRadius, _blackDamage, _blackRadius;
        
        // Other Variables
        private Rigidbody2D _rb;
        private int _currentState;
        private ParticleSystem _ps;
        private SpriteRenderer _sr;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _ps = GetComponent<ParticleSystem>();
            _sr = GetComponent<SpriteRenderer>();
            _ps.Stop();
        }
        
        private void Update()
        {
            var velocity = _rb.velocity;
            transform.Rotate(0,0, velocity.x > 0 ? -1 : 1);

            if (Input.GetMouseButtonDown(0) && _currentState != 1)
            {
                StartCoroutine(SwitchMode(1));
            }

            if (Input.GetMouseButtonDown(1) && _currentState != 2)
            {
                StartCoroutine(SwitchMode(2));
            }
        }

        public override void Detonate()
        {
            var pos = transform.position;
            if (_currentState == 0)
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);

                EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                    Radius, 1, DestroyTypes.Circular);
            }
            else if (_currentState == 1) // white
            {
                DamageHandler.i.HandleDamage(pos, _whiteRadius, _whiteDamage, DamageHandler.DamageType.Circular);

                EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                    _whiteRadius, 1, DestroyTypes.Circular);
            }
            else // black
            {
                DamageHandler.i.HandleDamage(pos, _blackRadius, _blackDamage, DamageHandler.DamageType.Circular);
            }
            SpawnExplosionFX();
            DoCameraShake();
            
            Destroy(gameObject);
        }
        
        public override void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(ExplosionFX, transform.position, Quaternion.identity);
            insExpl.transform.localScale *= _currentState == 0 ? Radius : _currentState == 1 ? _whiteRadius : _blackRadius;
            Destroy(insExpl, ExplosionDuration);
        }

        private IEnumerator SwitchMode(int mode)
        {
            _ps.Play();
            yield return new WaitForSeconds(0.05f);
            _currentState = mode;
            _sr.sprite = mode == 1 ? whiteSprite : blackSprite;
            _explosionFX = mode == 1 ? GameAssets.i.gunpowderlessExplosionFX : GameAssets.i.blackExplosionFX;
            yield return new WaitForSeconds(0.05f);
            _ps.Stop();
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
            
            _whiteDamage = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "whiteDamage").value;
            _whiteRadius = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "whiteRadius").value;
            _blackDamage = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "blackDamage").value;
            _blackRadius = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "blackRadius").value;
        }
        
    }
}