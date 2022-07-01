using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoulderProjectile : MonoBehaviour, IProjectile
    {
        private static float _radius, _damage, _maxMagnitude;
        private static int _steps;
        public GameObject explosionFX;
        
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
            DamageHandler.i.HandleCircularDamage(pos, _radius, _damage);

            TerrainDestroyer.Instance.DestroyTerrain(pos, _radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
            OnFinish();
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
