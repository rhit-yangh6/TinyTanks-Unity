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
                rigidBody2D.velocity = Vector2.down;
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
            
            // First Coconut
            var derivedObject = Instantiate(coconutProjectilePrefab, 
                new Vector2(pos.x - 1.0f, pos.y + 3.1f), 
                Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
            derivedProjectile.SetParameters(Damage, coconutRadius);

            if (Level == 5)
            {
                derivedObject.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                derivedProjectile.isIced = true;
            }

            yield return new WaitForSeconds(0.15f);
            
            // Second Coconut
            derivedObject = Instantiate(coconutProjectilePrefab, 
                new Vector2(pos.x + 0.8f, pos.y + 3.3f), 
                Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
            derivedProjectile.SetParameters(Damage, coconutRadius);
            
            if (Level == 5)
            {
                derivedObject.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                derivedProjectile.isIced = true;
            }
            
            yield return new WaitForSeconds(0.15f);

            // Third Coconut
            if (Level >= 4)
            {
                derivedObject = Instantiate(coconutProjectilePrefab, 
                    new Vector2(pos.x + 0.1f, pos.y + 3.2f), 
                    Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
                derivedProjectile.SetParameters(Damage, coconutRadius);
                
                if (Level == 5)
                {
                    derivedObject.GetComponent<SpriteRenderer>().sprite = coconutIceSprite;
                    derivedProjectile.isIced = true;
                }
            }
            
            yield return new WaitForSeconds(0.15f);
            
            // Fourth Coconut
            if (Level == 6)
            {
                derivedObject = Instantiate(coconutProjectilePrefab, 
                    new Vector2(pos.x - 0.5f, pos.y + 3.5f), 
                    Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<CoconutProjectile>();
                derivedProjectile.SetParameters(Damage, coconutRadius);
            }
            
            // Wait for everything finished
            yield return new WaitForSeconds(1.5f);
            
            Destroy(coconutTree);
            Destroy(gameObject);
        }
    }
}