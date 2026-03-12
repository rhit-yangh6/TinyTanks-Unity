using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    // TODO: Unlock this weapon
    public class SteelCubeProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject smallCubePrefab;
        [SerializeField] private float smallCubeDamage = 8.0f;
        [SerializeField] private float smallCubeRadius = 1.28f;
        [SerializeField] private float smallCubeAngleDelta = 15.0f;
        
        // References
        protected override float Radius => Level >= 2 ? radius * 1.2f : radius;
        protected override float Damage
        {
            get
            {
                if (Level == 5) return damage * 1.9f; // Level 5
                return Level >= 4 ? damage * 1.25f : damage; // Level 4
            }
        }

        protected override float MaxMagnitude => Level == 5 ? maxMagnitude * 0.6f : maxMagnitude; // Level 5

        private void Update()
        {
            Spin();
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            
            DealDamage();
            
            defaultMmFeedbacks.PlayFeedbacks();
            
            if (Level == 6)
            {
                SplitCube();
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            
            var isCritical = false;
            if (Level >= 3) isCritical = Random.value > 0.70; // Level 3

            DamageHandler.i.HandleDamage(pos, Radius, isCritical ? Damage * 1.5f : Damage,
                DamageHandler.DamageType.Square, isCritical);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, Level == 5 ? 2 : 1, DestroyTypes.Square);
        }

        private void SplitCube()
        {
            Vector2 pos = transform.position;
            for (var i = 0; i < 4; i++)
            {
                var rotateDegree = Random.Range(-smallCubeAngleDelta, smallCubeAngleDelta);
                var speed = Random.Range(5.5f, 9f);
                SpawnDerived(smallCubePrefab, pos, smallCubeDamage, smallCubeRadius,
                    Geometry.Rotate(Vector2.up, rotateDegree) * speed);
            }
        }
    }
}