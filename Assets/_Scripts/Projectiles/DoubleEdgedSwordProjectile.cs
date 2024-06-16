using System;
using _Scripts.Managers;
using UnityEngine;
using System.Collections;
using _Scripts.Entities;
using _Scripts.GameEngine;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class DoubleEdgedSwordProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private Sprite macheteSprite;
        [SerializeField] private MMFeedbacks activateMmFeedbacks, punishmentMmFeedbacks;
        [SerializeField] private float swordShadowYOffset = 4.6f;
        [SerializeField] private float animationDuration = 0.8f;
        [SerializeField] private float activationMultiplier = 0.77f;
        [SerializeField] private float punishmentMultiplier = 0.6f;
        [SerializeField] private float dodgeChance = 0.92f;
        
        protected override float Damage
        {
            get
            {
                return Level switch
                {
                    6 => damage * 1.27f,
                    >= 2 => damage * 1.06f,
                    _ => damage
                };
            }
        }

        protected override float MaxMagnitude
        {
            get
            {
                return Level switch
                {
                    6 => maxMagnitude * 0.7f,
                    >= 3 => maxMagnitude * 1.2f,
                    _ => maxMagnitude
                };
            }
        }

        protected override int Steps
        {
            get
            {
                return Level switch
                {
                    6 => (int)(steps * 0.5f),
                    >= 3 => (int)(steps * 1.2f),
                    _ => steps
                };
            }
        }
    
        // Other Variables
        private SpriteRenderer _sr;
        private bool _isActivated;
        
        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (Level == 6)
            {
                _sr.sprite = macheteSprite;
            }
        }

        private void Update()
        {
            if (_isActivated)
            {
                Spin();
            }
            else
            {
                Direct();
            }
            
            if (!Input.GetMouseButtonDown(0) || _isActivated || Level != 5) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            
            Disappear();
            
            defaultMmFeedbacks.PlayFeedbacks();
        }
        
        public void DoubleEdgedSwordAction()
        {
            Vector2 pos = transform.position;
            
            // Calculate Damage
            var damagedNumber = DamageHandler.i.HandleDamage(pos,
                _isActivated ? Radius * activationMultiplier : Radius,
                _isActivated ? Damage * activationMultiplier : Damage,
                DamageHandler.DamageType.Circular);

            if (damagedNumber == 0)
            {
                punishmentMmFeedbacks.PlayFeedbacks();
            }
        }

        public void HandlePunishment()
        {
            StartCoroutine(Punish());
        }

        private IEnumerator Punish()
        {
            // Spawn another sword at shooter's position
            var shooterPos = Shooter.transform.position;
            var insExpl = Instantiate(GameAssets.i.swordShadowFX, 
                new Vector2(shooterPos.x, shooterPos.y + swordShadowYOffset), Quaternion.identity);

            yield return new WaitForSeconds(animationDuration);
            // Take damage
            if (Level >= 4 && Random.value < dodgeChance)
            {
                Shooter.GetComponent<BuffableEntity>().TakeDamage(0);
            }
            else
            {
                Shooter.GetComponent<BuffableEntity>().TakeDamage(_isActivated ? Damage * activationMultiplier * punishmentMultiplier : Damage * punishmentMultiplier);
                
                // Increment SELF_SACRIFICE counter
                var sacrificeTimes = SteamManager.IncrementStat(Constants.StatDoubleEdgedSwordPunishmentCount);
                if (sacrificeTimes >= 25)
                {
                    SteamManager.UnlockAchievement(Constants.AchievementWillOfSacrifice);
                    WeaponManager.UnlockWeapon(27); // Sacrificial Bond 27
                }
            }
            Destroy(insExpl);
        }
    }
}