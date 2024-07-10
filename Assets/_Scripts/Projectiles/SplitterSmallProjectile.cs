using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class SplitterSmallProjectile :DerivedProjectile
    {
        [SerializeField] private float spawnVelocity = 3f;
        [SerializeField] private GameObject splitterSmallPrefab;
        [SerializeField] private float thirdProjectileChance = 0.35f;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        
        // Other Variables
        private bool _isActivated;
        private bool _canSplitAgain;

        private void Update()
        {
            Spin();
            if (!Input.GetMouseButtonDown(0) || _isActivated || !_canSplitAgain) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
            var pos = transform.position;
            // First Piece
            var derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = (Vector2.left * 2 + Vector2.up) * spawnVelocity;
            
            // Second Piece
            derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
            derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(Damage, Radius);
            derivedRb2d.velocity = (Vector2.right * 2 + Vector2.up) * spawnVelocity;
            
            if (Random.value < thirdProjectileChance)
            {
                // Third Piece
                derivedObject = Instantiate(splitterSmallPrefab, pos, Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(Damage, Radius);
                derivedRb2d.velocity = Vector2.up * spawnVelocity;
            }
        }

        public void SetExtraFields(bool canSplitAgain)
        {
            _canSplitAgain = canSplitAgain;
        }
    }
}