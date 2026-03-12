using System.Collections;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CoconutSeedProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject coconutProjectilePrefab;
        [SerializeField] private Sprite coconutIceSprite;
        [SerializeField] private float coconutRadius = 1.2f;
        
        // References
        protected override float Damage => Level >= 3 ? damage * 1.2f : damage;
        protected override float MaxMagnitude => Level >= 2 ? maxMagnitude * 1.2f : maxMagnitude;
        
        // Other Variables
        private bool _isCollided;

        public override void Detonate()
        {
            if (!_isCollided)
            {
                rigidBody2D.linearVelocity = Vector2.down;
                _isCollided = true;
                return;
            }

            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            
            defaultMmFeedbacks.PlayFeedbacks();
        }
        
        private void Update()
        {
            Spin();
        }

        public override void Activate()
        {
            StartCoroutine(GrowCoconutTree());
        }
        
        private IEnumerator GrowCoconutTree()
        {
            var pos = transform.position;
            var coconutTree =
                Instantiate(GameAssets.i.coconutTreeFX, new Vector2(pos.x, pos.y - 1.5f),
                    Quaternion.identity);

            yield return new WaitForSeconds(2.2f);

            SpawnCoconut(new Vector2(pos.x - 1.0f, pos.y + 3.1f));
            yield return new WaitForSeconds(0.15f);

            SpawnCoconut(new Vector2(pos.x + 0.8f, pos.y + 3.3f));
            yield return new WaitForSeconds(0.15f);

            if (Level >= 4)
            {
                SpawnCoconut(new Vector2(pos.x + 0.1f, pos.y + 3.2f));
            }
            yield return new WaitForSeconds(0.15f);

            if (Level == 6)
            {
                SpawnCoconut(new Vector2(pos.x - 0.5f, pos.y + 3.5f));
            }

            yield return new WaitForSeconds(1.5f);

            Destroy(coconutTree);
            Destroy(gameObject);
        }

        private void SpawnCoconut(Vector2 spawnPos)
        {
            var obj = Instantiate(coconutProjectilePrefab, spawnPos, Quaternion.identity);
            var coconut = obj.GetComponent<CoconutProjectile>();
            coconut.SetParameters(Damage, coconutRadius);

            if (Level == 5)
            {
                obj.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                coconut.isIced = true;
            }
        }
    }
}