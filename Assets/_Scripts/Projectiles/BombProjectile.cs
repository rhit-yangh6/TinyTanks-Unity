using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BombProjectile : MonoBehaviour, IProjectile
    {
        public int Level { get; set; }
        
        private static float _radius, _damage, _maxMagnitude, _detonateTime;
        private static int _steps;
        public GameObject explosionFX;

        SpriteRenderer _sr;
    
        void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            StartCoroutine(TickBomb());
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
        }

        IEnumerator TickBomb()
        {
            Invoke(nameof(Detonate), _detonateTime);
            while (true)
            {
                _sr.color = Color.red;
                yield return new WaitForSeconds(.1f);
                _sr.color = Color.white;
                yield return new WaitForSeconds(.1f);
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
            
            _detonateTime = Array.Find(extraWeaponTerms, ewt => ewt.term == "detonateTime").value; 
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
