using System;
using System.Collections;
using _Scripts.Buffs;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SnowballProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private float maxEnlargement = 3.0f;
        [SerializeField] private float growFactor = 0.27f;
        
        // References
        protected override float Damage => Level >= 2 ? damage * 1.1f : damage;
        protected override int Steps => Level >= 3 ? (int)(steps * 1.75f) : steps;
        
        // Other Variables
        private float _timer;

        private void Start()
        {
            StartCoroutine(Scale());
        }

        private void Update()
        {
            Direct();
        }

        private IEnumerator Scale()
        {
            while(true) // this could also be a condition indicating "alive or dead"
            {
                // we scale all axis, so they will have the same value, 
                // so we can work with a float instead of comparing vectors
                var maxSize = Level >= 4 ? maxEnlargement * 1.17f : maxEnlargement;
                while(maxSize > transform.localScale.x)
                {
                    _timer += Time.deltaTime;
                    transform.localScale +=  Time.deltaTime * growFactor * new Vector3(1, 1, 0);
                    yield return null;
                }
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            var multiplier = GetDamageAndRadiusMultiplier();
            
            if (Level == 5)
            {
                // Apply frozen buff if Level = 5
                DamageHandler.i.HandleDamage(pos, Radius * multiplier, Damage * multiplier, 
                    DamageHandler.DamageType.Circular, false, GameAssets.i.frozenBuff, 2);
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius * multiplier, Damage * multiplier, 
                    DamageHandler.DamageType.Circular);
            }
            
            if (Level == 6)
            {
                EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                    Radius * multiplier * 1.5f, 1, DestroyTypes.Circular);
            }
        }

        private float GetDamageAndRadiusMultiplier()
        {
            var maxSize = Level >= 4 ? maxEnlargement * 1.17f : maxEnlargement;
            return Math.Max(Math.Min(maxSize / 2, _timer / 2), 1f);
        }
    }
}
