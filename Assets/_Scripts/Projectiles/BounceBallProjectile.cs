using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BounceBallProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks bounceMmFeedbacks;
        [SerializeField] private float bounceDamage = 15f;
        [SerializeField] private float bounceRadius = 2f;
        [SerializeField] private float unitBounceDamage = 4f;
        
        // References
        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    5 => damage + unitBounceDamage * 5,
                    6 => damage + (bounceDamage + unitBounceDamage) * 3,
                    _ => damage,
                };
            }
        }

        protected override float MaxMagnitude => Level >= 2 ? maxMagnitude * 1.2f : maxMagnitude;
        private int BounceTime
        {
            get
            {
                return Level switch
                {
                    5 => 5,
                    >= 3 => 3,
                    _ => 2
                };
            }
        }

        // Other Variables
        private int _bounceTimeLeft;

        private void Start()
        {
            _bounceTimeLeft = BounceTime;
        }
        
        private void Update()
        {
            Spin();
        }

        protected override void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
            }
            else if (_bounceTimeLeft > 0)
            {
                _bounceTimeLeft--;
                Bounce();
            }
            else
            {
                Detonate();
            }
        }

        private void Bounce()
        {
            bounceMmFeedbacks.PlayFeedbacks();
            BounceDealDamage();
        }

        private void BounceDealDamage()
        {
            var pos = transform.position;

            var finalCalculatedDamage = bounceDamage;
            switch (Level)
            {
                case 5:
                    finalCalculatedDamage += (5 - _bounceTimeLeft) * unitBounceDamage;
                    break;
                case >= 3:
                    finalCalculatedDamage += unitBounceDamage;
                    break;
            }

            DamageHandler.i.HandleDamage(pos, bounceRadius, finalCalculatedDamage, DamageHandler.DamageType.Circular);

            if (Level >= 4) EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                bounceRadius, 1, DestroyTypes.Circular);
        }
    }
}