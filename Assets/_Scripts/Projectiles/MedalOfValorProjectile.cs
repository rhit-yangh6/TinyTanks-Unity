using System;
using _Scripts.Entities;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class MedalOfValorProjectile: LaunchedProjectile
    {
        [SerializeField] private ParticleSystem _ps;
        protected override float MaxMagnitude => Level >= 2 ? maxMagnitude * 1.2f : maxMagnitude;
        protected override float Damage => Level >= 3 ? damage * 1.2f : damage;
        protected override float Radius => Level >= 3 ? radius * 0.9f : radius;
        private float HighestDamageMultiplier
        {
            get
            {
                return Level switch
                {
                    >= 4 => 2.3f,
                    _ => 2f
                };
            }
        }

        private const float Level4Threshold = 0.3f;
        private const float Level5HealAmount = 30f;
        private const float Level6DamageMultiplier = 3f;
        private const float Level6RadiusMultiplier = 1.3f;
        private const float Level6Threshold = 0.1f;

        private bool _isCritical;
        private float _finalDamage;
        private float _finalRadius;

        protected override void Start()
        {
            base.Start();
            _ps.Stop();
            var currentHealthPercentage = Shooter.GetComponent<Entity>().GetHealthPercentage();
            Debug.Log(Damage);
            _finalDamage = Damage;
            _finalRadius = Radius;

            switch (Level)
            {
                case 6 when currentHealthPercentage < Level6Threshold:
                    _finalDamage = Damage * Level6DamageMultiplier;
                    _finalRadius = Radius * Level6RadiusMultiplier;
                    _isCritical = true;
                    _ps.Play();
                    break;
                case >=4 when currentHealthPercentage < Level4Threshold:
                    _finalDamage = Damage * HighestDamageMultiplier;
                    _isCritical = true;
                    _ps.Play();
                    break;
                default:
                    _isCritical = false;
                    _finalDamage = Mathf.Lerp(HighestDamageMultiplier, 1, currentHealthPercentage) * Damage;
                    break;
            }
        }

        private void Update()
        {
            Spin();
        }
        
        public override void DealDamage()
        {
            var pos = transform.position;
            var targetHits = DamageHandler.i.HandleDamage(pos, _finalRadius, _finalDamage, DamageHandler.DamageType.Circular, _isCritical);
            if (Level == 5 && targetHits >= 2)
            {
                Shooter.GetComponent<Entity>().TakeDamage(-Level5HealAmount);
            }
        }
    }
}