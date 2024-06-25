using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class FireballProjectile: LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private GameObject fireballSmallPrefab;
        [SerializeField] private float summonInterval = 0.3f;
        [SerializeField] private float fireballSmallDamage = 10f;
        [SerializeField] private float fireballSmallRadius = 1.5f;
        
        // References
        protected override float Radius => Level >= 3 ? radius * 1.3f : radius;
        protected override float Damage => Level >= 2 ? damage * 1.1f : damage;

        private int BurningBuffLevel
        {
            get
            {
                if (Level == 5) return 4; // Level 5: Eternal Fire
                return Level >= 4 ? 3 : 2; // LEVEL 4+
            }
        }
        
        private void Start()
        {
            if (Level == 6) StartCoroutine(SummonSmallFireballs());
        }

        private IEnumerator SummonSmallFireballs()
        {
            while (!isDetonated)
            {
                yield return new WaitForSeconds(summonInterval);
                var derivedObject = Instantiate(fireballSmallPrefab, gameObject.transform.position, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
                
                derivedProjectile.SetParameters(fireballSmallDamage, fireballSmallRadius);
                derivedRb2d.velocity = Vector2.zero;
                
                var spawnTimes = SteamManager.IncrementStat(Constants.StatSmallFireballSpawned);
                if (spawnTimes >= 100)
                {
                    SteamManager.UnlockAchievement(Constants.AchievementSpreadTheFire);
                    WeaponManager.UnlockWeapon(31); // Molotov 31
                }
            }
        }

        private void Update()
        {
            Direct();
        }

        public override void DealDamage()
        {
            var pos = transform.position;

            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular,
                false, GameAssets.i.burningBuff, BurningBuffLevel);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
        }
    }
}