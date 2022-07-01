using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SnowballProjectile : MonoBehaviour, IProjectile
    {
        private static float _radius, _maxMagnitude, _damage, _maxSize, _growFactor;
        private static int _steps;
        public GameObject explosionFX;

        private float _timer;
        private Rigidbody2D _rb;
        
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
            StartCoroutine(Scale());
        }

        void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        IEnumerator Scale()
        {
            while(true) // this could also be a condition indicating "alive or dead"
            {
                // we scale all axis, so they will have the same value, 
                // so we can work with a float instead of comparing vectors
                while(_maxSize > transform.localScale.x)
                {
                    _timer += Time.deltaTime;
                    transform.localScale +=  Time.deltaTime * _growFactor * new Vector3(1, 1, 0);
                    yield return null;
                }
            }
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
                OnFinish();
            }
            else
            {
                Detonate();
            }
        }

        public void Detonate()
        {
            Vector2 pos = transform.position;
            
            // TODO: 1 + ??
            float multiplier = Math.Max(Math.Min(_maxSize / 2, _timer / 2), 1f);
            
            DamageHandler.i.HandleCircularDamage(pos, _radius * multiplier, _damage * multiplier);
            
            // TODO: SNOWBALL - DESTROY TERRAIN IF UPGRADED?
            
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
            OnFinish();
        }

        public void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(explosionFX, transform.position, quaternion.identity);
            insExpl.transform.localScale *= _radius * _timer / 2;
            Destroy(insExpl, .2f);
        }

        public void DoCameraShake()
        {
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.1f;
        }

        public void OnFinish()
        {
            GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
            gameController.ChangeTurn();
        }

        public void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            
            _maxSize = Array.Find(extraWeaponTerms, ewt => ewt.term == "maxSize").value;
            _growFactor = Array.Find(extraWeaponTerms, ewt => ewt.term == "growFactor").value;
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
