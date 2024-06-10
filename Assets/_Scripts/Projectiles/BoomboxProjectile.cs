using System;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class BoomboxProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private float shockDamage = 20f;
        [SerializeField] private float shockRadius = 5f;
        [SerializeField] private float shockInterval = 1f;
        [SerializeField] private float waveDuration = 0.3f;
        
        // References
        protected override float MaxMagnitude => Level >= 4 ? maxMagnitude * 1.3f : maxMagnitude;
        private int ShockTimes
        {
            get
            {
                return Level switch
                {
                    >= 6 => 1,
                    >= 5 => 5,
                    >= 3 => 3,
                    _ => 2
                };
            }
        }
        private float ShockDamage
        {
            get
            {
                return Level switch
                {
                    6 => shockDamage * 2f,
                    5 => shockDamage * 0.5f,
                    >= 4 => shockDamage * 1.2f,
                    >= 2 => shockDamage * 1.18f,
                    _ => shockDamage
                };
            }
        }
        private float ShockRadius
        {
            get
            {
                return Level switch
                {
                    6 => shockRadius * 0.7f,
                    >= 4 => shockRadius * 1.3f,
                    >= 2 => shockRadius * 1.2f,
                    _ => shockRadius
                };
            }
        }
        private float ShockInterval
        {
            get
            {
                return Level switch
                {
                    5 => shockInterval * 0.4f,
                    _ => shockInterval
                };
            }
        }
        
        // Other Variables
        private int _shockTimeLeft; 
        private float _shockIntervalLeft;
        
        private void Start()
        {
            _shockTimeLeft = ShockTimes;
        }

        private void Update()
        {
            Spin();
            
            if (_shockIntervalLeft > 0)
            {
                _shockIntervalLeft -= Time.deltaTime;
            }

            if (_shockTimeLeft > 0 && Input.GetMouseButtonDown(0) && _shockIntervalLeft <= 0)
            {
                activateMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            Shock();
            _shockTimeLeft--;
            _shockIntervalLeft = ShockInterval;
        }

        private void Shock()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos,
                ShockRadius, ShockDamage, DamageHandler.DamageType.Circular);
                
            GameObject insExpl = Instantiate(GameAssets.i.shockwaveFX, pos, Quaternion.identity);
            insExpl.GetComponent<ShockwaveManager>().CallShockwave(waveDuration, 0.04f * ShockRadius);
            Destroy(insExpl, waveDuration);
        }
    }
}