using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class RocketProjectile : MonoBehaviour, IProjectile
    {
        private static float _radius, _maxMagnitude, _damage, _velocityMultiplier;
        private static int _steps;
        public GameObject explosionFX;

        private bool _isActivated;
        private Rigidbody2D _rb;
        private ParticleSystem _ps;
    
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _ps = gameObject.GetComponentInChildren<ParticleSystem>();
            _ps.Stop();
        }

        private void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                velocity *= _velocityMultiplier;
                _rb.velocity = velocity;
                _ps.Play();
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
            
            DamageHandler.i.HandleCircularDamage(pos, _radius, _damage);

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
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.2f;
        }

        public void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            
            _velocityMultiplier = Array.Find(extraWeaponTerms, ewt => ewt.term == "velocityMultiplier").value; 
        }

        public float getMaxMagnitude()
        {
            return _maxMagnitude;
        }

        public int getSteps()
        {
            return _steps;
        }
    }
}
