using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class DiceProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject topDownDisplay;
        [SerializeField] private float unitDiceDamage = 3.0f;
        
        // References
        protected override float Radius => Level >= 2 ? radius * 1.5f : radius;
        protected override float Damage => Level >= 3 ? damage * 1.2f : damage;
        
        // Other Variables
        private int _diceResult;

        private int HandleDiceResult()
        {
            return Level switch
            {
                5 => Random.value > 0.7 ? 6 : Random.Range(1, 6),
                >= 4 => Random.value > 0.78 ? 6 : Random.Range(1, 6),
                _ => Random.Range(1, 7)
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

            var finalCalculatedDamage = Damage + _diceResult * unitDiceDamage;
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

        public void DisplayTopDownResult()
        {
            var cameraCenterPos = Camera.main.transform.position;
            var insTopdown = Instantiate(topDownDisplay, 
                new Vector3(cameraCenterPos.x, cameraCenterPos.y, 0), 
                quaternion.identity);
            insTopdown.GetComponent<SpriteRenderer>().sprite = 
                (Level == 6 && _diceResult == 3) ? GameAssets.i.diceNumbers[6] : GameAssets.i.diceNumbers[_diceResult - 1];
            insTopdown.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-18.0f, 18.0f)));
            Destroy(insTopdown, 1.9f);
        }
    }
}
