using System.Collections.Generic;
using _Scripts.Entities;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class ElectricOrbProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private MMFeedbacks dischargedMmFeedbacks;

        [SerializeField] private float stunChance = 0.2f;
        // References
        protected override float Radius
        {
            get
            {
                return Level switch
                {
                    >= 2 => radius * 1.2f,
                    _ => radius
                };
            }
        }

        protected override float Damage => damage;
        protected override float MaxMagnitude => maxMagnitude;
        protected override int Steps => steps;
        private int ChainingNumber {
            get
            {
                return Level switch
                {
                    6 => 4,
                    >= 3 => 3,
                    _ => 2
                };
            }
        }

        private bool _isActivated;
        
        private void Update()
        {
            Spin(1.3f);
            if (Input.GetMouseButtonDown(0) && !_isActivated && Level >= 4)
            {
                _isActivated = true;
                activateMmFeedbacks.PlayFeedbacks();
            }
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();

            if (!_isActivated)
            {
                DealDamage();
                defaultMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                dischargedMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            var excludingEntities = new List<Entity>(){};
            var previousPosition = pos;

            for (var i = 0; i < ChainingNumber; i++)
            {
                var closestEntity = DamageHandler.i.DetectNearestTarget(previousPosition, Radius, excludingEntities);

                if (closestEntity == null)
                {
                    break;
                }

                // Add the found entity in the list of excludingEntities to avoid it being damaged twice
                excludingEntities.Add(closestEntity);
            
                // Chain, won't deal damage if there's no entity in the radius
                var electricLine = Instantiate(GameAssets.i.electricLineFX).GetComponent<LineController>();
                var entityPos = closestEntity.transform.position;
                electricLine.AssignPositions(previousPosition, entityPos);
                previousPosition = entityPos;
                closestEntity.TakeDamage(Damage);
                
                var be = closestEntity.GetComponent<BuffableEntity>();
                // Apply Buff
                if (be != null && Level == 5 && Random.value < stunChance)
                {
                    be.AddBuff(GameAssets.i.stunnedBuff.InitializeBuff(be.gameObject, 1));
                }
            }

            if (Level == 6)
            {
                previousPosition = pos;
                for (var i = 0; i < ChainingNumber; i++)
                {
                    var closestEntity = DamageHandler.i.DetectNearestTarget(previousPosition, Radius, excludingEntities);

                    if (closestEntity == null)
                    {
                        break;
                    }

                    // Add the found entity in the list of excludingEntities to avoid it being damaged twice
                    excludingEntities.Add(closestEntity);
            
                    // Chain, won't deal damage if there's no entity in the radius
                    var electricLine = Instantiate(GameAssets.i.electricLineFX).GetComponent<LineController>();
                    var entityPos = closestEntity.transform.position;
                    electricLine.AssignPositions(previousPosition, entityPos);
                    previousPosition = entityPos;
                    closestEntity.TakeDamage(Damage);
                }
            }
        }
    }
}