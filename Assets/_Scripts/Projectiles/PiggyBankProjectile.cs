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

        private int ActivateCost
        {
            get
            {
                return Level switch
                {
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
            }
            else
            {
                DealMinimalDamage();
                
            }
            defaultMmFeedbacks.PlayFeedbacks();
        }

        private void DealMinimalDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, MinimalRadius, MinimalDamage, DamageHandler.DamageType.Circular);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, MinimalRadius, 1, DestroyTypes.Circular);
        }
    }
}