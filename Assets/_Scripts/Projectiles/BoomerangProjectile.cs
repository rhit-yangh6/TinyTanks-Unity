using System;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomerangProjectile : MonoBehaviour, IProjectile
    {
        
        public GameObject explosionFX;
        
        private static float _radius, _maxMagnitude, _damage;
        private static int _steps;

        private static float _travelBackTime, _proximityDetectRange;

        private bool _isActivated;
        private Rigidbody2D _rb;

        private Transform _playerTransform;
        
        
        // TODO: Save Shooter (For All Projectiles)
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            
            _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                
                Vector2 startPos = transform.position, endPos = _playerTransform.position;
            
                float t = _travelBackTime;

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
            Destroy(insExpl, .15f);
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

            _travelBackTime = Array.Find(extraWeaponTerms, ewt => ewt.term == "travelBackTime").value; 
            _proximityDetectRange = Array.Find(extraWeaponTerms, ewt => ewt.term == "proximityDetectRange").value;
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
