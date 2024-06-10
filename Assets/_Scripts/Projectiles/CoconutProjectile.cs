using _Scripts.Managers;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class CoconutProjectile: DerivedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks icedMmFeedbacks;
        
        // Extra Fields
        [HideInInspector] public bool isIced;
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();
            DealDamage();

            if (isIced)
            {
                icedMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void DealDamage()
        {
            Vector2 pos = transform.position;
            if (isIced)
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular, 
                    false, GameAssets.i.frozenBuff, 2);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            }
        }
    }
}