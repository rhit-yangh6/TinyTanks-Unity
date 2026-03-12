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
            Vector2 pos = transform.position;
            SpawnDerived(splitterSmallPrefab, pos, Damage, Radius, (Vector2.left * 2 + Vector2.up) * spawnVelocity);
            SpawnDerived(splitterSmallPrefab, pos, Damage, Radius, (Vector2.right * 2 + Vector2.up) * spawnVelocity);

            if (Random.value < thirdProjectileChance)
            {
                SpawnDerived(splitterSmallPrefab, pos, Damage, Radius, Vector2.up * spawnVelocity);
            }
        }

        public void SetExtraFields(bool canSplitAgain)
        {
            _canSplitAgain = canSplitAgain;
        }
    }
}