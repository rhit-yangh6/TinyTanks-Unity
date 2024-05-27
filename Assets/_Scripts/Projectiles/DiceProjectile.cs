using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using JetBrains.Annotations;
using TerraformingTerrain2d;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class DiceProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject topDownDisplay;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _unitDiceDamage;
        
        // References
        protected override float Radius => Level >= 2 ? _radius * 1.5f : _radius;
        protected override float Damage => Level >= 3 ? _damage * 1.2f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private int _diceResult;

        private int HandleDiceResult()
        {
            return Level switch
            {
                5 => Random.value > 0.7 ? 6 : Random.Range(1, 5),
                >= 4 => Random.value > 0.78 ? 6 : Random.Range(1, 5),
                _ => Random.Range(1, 6)
            };
        }


        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            _diceResult = HandleDiceResult();
            
            defaultMmFeedbacks.PlayFeedbacks();
        }

        public override void DealDamage()
        {
            var pos = transform.position;

            var finalCalculatedDamage = Damage + _diceResult * _unitDiceDamage;
            var finalCalculatedRadius = Radius;

            if (_diceResult == 3 && Level == 6)
            {
                finalCalculatedDamage *= 1.5f;
                finalCalculatedRadius *= 1.5f;
            }

            DamageHandler.i.HandleDamage(pos, finalCalculatedRadius, finalCalculatedDamage, 
                DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                finalCalculatedRadius, 1, DestroyTypes.Circular);
        }
        
        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration,
            ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _unitDiceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "unitDiceDamage").value;
        }

        public void DisplayTopDownResult()
        {
            Vector3 cameraCenterPos = Camera.main.transform.position;
            GameObject insTopdown = Instantiate(topDownDisplay, 
                new Vector3(cameraCenterPos.x, cameraCenterPos.y, 0), 
                quaternion.identity);
            insTopdown.GetComponent<SpriteRenderer>().sprite = 
                (Level == 6 && _diceResult == 3) ? GameAssets.i.diceNumbers[6] : GameAssets.i.diceNumbers[_diceResult - 1];
            insTopdown.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-18.0f, 18.0f)));
            Destroy(insTopdown, 1.9f);
        }
    }
}
