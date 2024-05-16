using System;
using _Scripts.Managers;
using UnityEngine;
using System.Collections;
using _Scripts.Entities;
using _Scripts.GameEngine;
using _Scripts.Utils;
using TerraformingTerrain2d;
using Unity.VisualScripting;

namespace _Scripts.Projectiles
{
    public class DoubleEdgedSwordProjectile : LaunchedProjectile
    {
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;
        
        // ExtraFields
        private static float _swordShadowYOffset;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private Rigidbody2D _rb;
        private Renderer _r;
        private bool isDetonated;
    
        private void Start()
        {
            isDetonated = false;
            _rb = gameObject.GetComponent<Rigidbody2D>();
            _r = GetComponent<Renderer>();
        }

        private void Update()
        {
            Vector2 velocity = _rb.velocity;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        
        public override void Detonate()
        {
            if (isDetonated)
            {
                return;
            }

            isDetonated = true;
            
            Vector2 pos = transform.position;

            _rb.gravityScale = 0;
            _rb.velocity = Vector2.zero;
            _r.enabled = false;
            
            StartCoroutine(DoubleEdgedSwordAction(pos));
            // Destroy(gameObject);
        }
        
        private IEnumerator DoubleEdgedSwordAction(Vector2 pos)
        {
            // Spawn FX first
            SpawnExplosionFX();
            DoCameraShake();
            yield return new WaitForSeconds(ExplosionDuration);
            
            // Calculate Damage
            var damagedNumber = DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);
            
            // It doesn't destroy tiles, for now
            // TerrainDestroyer.instance.DestroyTerrainCircular(pos, Radius);

            if (damagedNumber == 0)
            {
                // Spawn another sword at shooter's position
                var shooterPos = Shooter.transform.position;
                GameObject insExpl = Instantiate(ExplosionFX, new Vector2(shooterPos.x, shooterPos.y + _swordShadowYOffset), Quaternion.identity);
            
                // Take damage
                yield return new WaitForSeconds(ExplosionDuration);
                Shooter.GetComponent<BuffableEntity>().TakeDamage(Damage);
                
                // Increment SELF_SACRIFICE counter
                var sacrificeTimes = SteamManager.IncrementStat(Constants.StatDoubleEdgedSwordPunishmentCount);

                if (sacrificeTimes >= 25)
                {
                    SteamManager.UnlockAchievement("AchievementWillOfSacrifice");
                    WeaponManager.Instance.UnlockWeapon(27); // Sacrificial Bond 27
                }
                
                Destroy(insExpl);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public override void SpawnExplosionFX()
        {
            var pos = transform.position;
            GameObject insExpl = Instantiate(ExplosionFX, new Vector2(pos.x, pos.y + _swordShadowYOffset), Quaternion.identity);
            Destroy(insExpl, ExplosionDuration);
        }

        public override void SetParameters(float damage, float radius, float maxMagnitude, int steps, float explosionDuration,
            ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;
            
            _explosionFX = GameAssets.i.swordShadowFX;
            
            _swordShadowYOffset = Array.Find(extraWeaponTerms, ewt => ewt.term == "swordShadowYOffset").value;
        } 
    }
    
}