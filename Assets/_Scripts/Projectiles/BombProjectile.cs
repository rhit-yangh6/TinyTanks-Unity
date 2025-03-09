using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class BombProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks secondaryExplosionFeedbacks;
        [SerializeField] private MMFeedbacks clusterExplosionFeedbacks;
        [SerializeField] private MMFeedbacks longerFeedbacks;
        [SerializeField] private MMFeedbacks clusterSoundFeedbacks;
        [SerializeField] private float clusterExplosionRadius = 1.5f;
        [SerializeField] private float clusterExplosionDamage = 10f;
        [SerializeField] private float clusterExplosionDuration = 0.2f;
        [SerializeField] private float secondaryExplosionRadius = 2.6f;
        [SerializeField] private float secondaryExplosionDamage = 20f;

        // References
        protected override float Damage => Level >= 4 ? damage * 1.25f : damage;
        protected override float MaxMagnitude => Level >= 3 ? maxMagnitude * 1.1f : maxMagnitude;

        // Other Variables
        private readonly int[,] _clusterDirections = 
        { { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 }, {-2, 0}, {-1, 1}, {0, 2}, {1, 1} };
        
        protected override void Start()
        {
            base.Start();
            switch (Level)
            {
                case 6:
                    clusterExplosionFeedbacks.PlayFeedbacks();
                    break;
                case 5:
                    if (Random.value < 0.35) secondaryExplosionFeedbacks.PlayFeedbacks();
                    else longerFeedbacks.PlayFeedbacks();
                    break;
                case >= 2:
                    longerFeedbacks.PlayFeedbacks();
                    break;
                default:
                    defaultMmFeedbacks.PlayFeedbacks();
                    break;
            }
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone")) Destroy(gameObject);
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            DealDamage();
        }

        public void SpawnSecondaryExplosion()
        {
            var origin = transform.position;
            
            DamageHandler.i.HandleDamage(origin, secondaryExplosionRadius, secondaryExplosionDamage, 
                DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, origin,
                secondaryExplosionRadius, 1, DestroyTypes.Circular);
        }

        private IEnumerator SpawnClusterExplosion(Vector3 origin)
        {
            for (var i = 0; i < 8; i++)
            {
                var direction = Vector3.Normalize(Vector2.right * _clusterDirections[i, 0] +
                                                  Vector2.up * _clusterDirections[i, 1]);
                var pos = origin + direction * (Radius + clusterExplosionRadius) / 2;
                
                DamageHandler.i.HandleDamage(pos, clusterExplosionRadius, clusterExplosionDamage,
                    DamageHandler.DamageType.Circular);
                EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                    clusterExplosionRadius, 1, DestroyTypes.Circular);

                var insExpl = Instantiate(GameAssets.i.explosionFX, pos, Quaternion.identity);
                insExpl.transform.localScale *= clusterExplosionRadius;
                clusterSoundFeedbacks.PlayFeedbacks();
                Destroy(insExpl, clusterExplosionDuration);  
                
                yield return new WaitForSeconds(.07f);
            }

            yield return 0;
        }

        public void TriggerClusterExplosion()
        {
            StartCoroutine(SpawnClusterExplosion(transform.position));
        }
    }
}
