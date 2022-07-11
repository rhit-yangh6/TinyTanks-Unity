using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoulderProjectile : MonoBehaviour, IProjectile
    {
        public int Level { get; set; }

        private static float _radius, _damage, _maxMagnitude;
        private static int _steps;
        public GameObject explosionFX;
        
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


            
            // Gaining from Higher Levels
            float finalCalculatedRadius = _radius;
            float finalCalculatedDamage = _damage;

            if (Level >= 2)
            {
                finalCalculatedRadius *= 1.15f;
            }

            if (Level >= 3)
            {
                finalCalculatedDamage *= 1.20f;
            }
            
            

            DamageHandler.i.HandleCircularDamage(pos, finalCalculatedRadius, finalCalculatedDamage);

            TerrainDestroyer.Instance.DestroyTerrain(pos, finalCalculatedRadius);
        
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
