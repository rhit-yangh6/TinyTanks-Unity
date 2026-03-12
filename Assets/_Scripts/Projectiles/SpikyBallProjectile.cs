using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SpikyBallProjectile: LaunchedProjectile
    {
         // Set in Inspector
        [SerializeField] private GameObject ballPrefab, spikePrefab;
        [SerializeField] private MMFeedbacks activateFeedbacks;
        [SerializeField] private float spikeDamage = 8f;
        [SerializeField] private float spikeRadius = 1f;
        
        // References
        protected override float Damage => Level >= 2 ? damage * 1.2f : damage;
        
        // Other Variables
        private bool _isActivated;
        private readonly int[,] _fourDirections = 
            { { 1, 0 }, { 0, -1 }, { -1, 0 }, { 0, 1 } };
        private readonly int[,] _eightDirections = 
            { { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 }, {-2, 0}, {-1, 1}, {0, 2}, {1, 1} };
        private readonly int[,] _twelveDirections = 
            { { 3, 0 }, { 2, -1 }, { 1, -2 }, { 0, -3 }, { -1, -2 }, { -2, -1 }, { -3, 0 }, {-2, 1}, {-1, 2}, {0, 3}, {1, 2}, {2, 1} };
        

        private void Update()
        {
            Spin();

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                Disappear();
                activateFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            Vector2 pos = transform.position;

            if (Level >= 3)
            {
                var ballObj = Instantiate(ballPrefab, pos, Quaternion.identity);
                ballObj.GetComponent<DerivedProjectile>().SetParameters(Damage, Radius);
            }

            var (directions, count) = Level switch
            {
                5 => (_twelveDirections, 12),
                >= 4 => (_eightDirections, 8),
                _ => (_fourDirections, 4)
            };

            var finalDamage = Level == 6 ? spikeDamage * 2 : spikeDamage;
            var finalRadius = Level == 6 ? spikeRadius * 2 : spikeRadius;

            for (var i = 0; i < count; i++)
            {
                var direction = Vector3.Normalize(
                    Vector2.right * directions[i, 0] + Vector2.up * directions[i, 1]);
                SpawnDerived(spikePrefab, pos, finalDamage, finalRadius, direction * 25f);
            }
        }
    }
}