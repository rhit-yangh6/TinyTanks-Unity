using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class AnvilProjectile : MonoBehaviour, IProjectile
    {
        
        private static float _radius, _maxMagnitude, _damage, _gravityScaleMultiplier, _fallDamageMultiplier;
        private static int _steps;
        
        public GameObject explosionFX;

        private bool _isActivated = false;
        private Rigidbody2D _rb;
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (!_isActivated)
            {
                Vector2 velocity = _rb.velocity;
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                _rb.velocity = Vector2.zero;
                _rb.gravityScale *= _gravityScaleMultiplier;
                transform.rotation = Quaternion.identity;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else
            {
                Detonate();
            }
        }

        public void Detonate()
        {
            Vector2 pos = transform.position;

            float damageDealt = _isActivated ? _damage * _fallDamageMultiplier : _damage;
            
            DamageHandler.i.HandleCircularDamage(pos, _radius, damageDealt);
            
            TerrainDestroyer.Instance.DestroyTerrain(pos, _radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        public void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(explosionFX, transform.position, quaternion.identity);
            insExpl.transform.localScale *= _radius;
            Destroy(insExpl, .2f);
        }

        public void DoCameraShake()
        {
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.15f;
        }

        public void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            
            _gravityScaleMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "gravityScaleMultiplier").value;
            _fallDamageMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "fallDamageMultiplier").value;
        }

        public float GetMaxMagnitude()
        {
            return _maxMagnitude;
        }

        public int GetSteps()
        {
            return _steps;
        }

        public float GetFixedMagnitude()
        {
            return -1f;
        }
    }
}
