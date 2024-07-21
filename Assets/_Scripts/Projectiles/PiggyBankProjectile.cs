using _Scripts.GameEngine;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class PiggyBankProjectile : LaunchedProjectile
    {
        [SerializeField] private GameObject coinPickupPrefab;
        [SerializeField] private float minimalDamage = 8f;
        [SerializeField] private float minimalRadius = 1.2f;
        [SerializeField] private float refundChance = 0.2f;
        [SerializeField] private float refundPortion = 0.4f;
        [SerializeField] private float spawnCoinRange = 2f;

        protected override float Radius
        {
            get
            {
                return Level switch
                {
                    >= 3 => radius * 1.15f,
                    _ => radius
                };
            }
        }
        
        private int ActivateCost
        {
            get
            {
                return Level switch
                {
                    >= 2 => 20,
                    _ => 30
                };
            }
        }

        private float MinimalDamage
        {
            get
            {
                return Level switch
                {
                    _ => minimalDamage,
                };
            }
        }
        
        private float MinimalRadius
        {
            get
            {
                return Level switch
                {
                    _ => minimalRadius,
                };
            }
        }
        
        private void Update()
        {
            Spin(1.2f);
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();
            
            if (PlayerData.Instance.SpendCoins(ActivateCost))
            {
                DealDamage();
                // Activate feedback?
                if (Level == 6)
                {
                    SpawnCoins();
                }
            }
            else
            {
                DealMinimalDamage();
            }
            defaultMmFeedbacks.PlayFeedbacks();
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            var targetHits = DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Circular);
            switch (Level)
            {
                case 5 when targetHits >= 3:
                    PlayerData.Instance.GainMoney(ActivateCost);
                    break;
                case >= 4 when Random.value < refundChance:
                    PlayerData.Instance.GainMoney((int)(ActivateCost * refundPortion));
                    break;
            }
        }

        private void DealMinimalDamage()
        {
            var pos = transform.position;
            var targetHits = 
                DamageHandler.i.HandleDamage(pos, MinimalRadius, MinimalDamage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, MinimalRadius, 1, DestroyTypes.Circular);
        }

        private void SpawnCoins()
        {
            var pos = transform.position;
            // Spawn coins
            for (var i = 0; i < 3; i++)
            {
                Instantiate(coinPickupPrefab, new Vector2(
                    pos.x + Random.Range(-spawnCoinRange, spawnCoinRange), 
                    pos.y + Random.Range(-spawnCoinRange, spawnCoinRange)), Quaternion.identity);
            }
        }
    }
}