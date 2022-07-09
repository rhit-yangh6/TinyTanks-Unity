using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SniperProjectile : MonoBehaviour, IProjectile
    {
        
        [SerializeField] private GameObject explosionFX;
        
        private static float _radius, _damage, _maxMagnitude;
        private static int _steps;

        private static float _fixedMagnitude;

        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
            
            DamageHandler.i.HandleCircularDamage(pos, _radius, _damage);
            
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
        }

        public void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(explosionFX, transform.position, quaternion.identity);
            insExpl.transform.localScale *= _radius;
            Destroy(insExpl, .1f);
        }

        public void DoCameraShake()
        {
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.1f;
        }

        public void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            
            _fixedMagnitude = Array.Find(extraWeaponTerms, ewt => ewt.term == "fixedMagnitude").value;
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
            return _fixedMagnitude;
        }
    }
}
