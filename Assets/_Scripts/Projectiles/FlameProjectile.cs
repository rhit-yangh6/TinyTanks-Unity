using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FlameProjectile: DerivedProjectile
    {
        [SerializeField] private float terrainDestroyInterval = 1f;
        [SerializeField] private float burnInterval = 1f;
        [SerializeField] private float radiusFluctuation = 0.3f;
        
        private float _burnIntervalLeft;
        private float _terrainDestroyIntervalLeft;
        private float _lifeSpan = 3f;
        private float _lifeTime;
        
        private void Update()
        {
            Debug.Log(isDetonated);
            if (!isDetonated) return;
            _lifeTime += Time.deltaTime;
            if (_lifeTime >= _lifeSpan)
            {
                Destroy(gameObject);
            }
            CheckBurn();
            CheckTerrainDestroy();
        }

        private void CheckBurn()
        {
            if (_burnIntervalLeft > 0)
            {
                _burnIntervalLeft -= Time.deltaTime;
            }
            if (_burnIntervalLeft > 0) return;
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, false,
                GameAssets.i.burningBuff);
            _burnIntervalLeft = burnInterval;
        }

        private void CheckTerrainDestroy()
        {
            if (_terrainDestroyIntervalLeft > 0)
            {
                _terrainDestroyIntervalLeft -= Time.deltaTime;
            }
            if (_terrainDestroyIntervalLeft > 0) return;
            var pos = transform.position;
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius + Random.Range(-radiusFluctuation, radiusFluctuation), 1, DestroyTypes.Circular);
            _terrainDestroyIntervalLeft = terrainDestroyInterval;
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            
            _burnIntervalLeft = burnInterval;
            _terrainDestroyIntervalLeft = terrainDestroyInterval;
        }
        
        protected override void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log("aa");
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else
            {
                Detonate();
            }
        }

        public void SetLifeSpan(float lifeSpan)
        {
            _lifeSpan = lifeSpan;
        }
    }
}