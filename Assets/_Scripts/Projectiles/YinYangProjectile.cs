using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class YinYangProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private Sprite blackSprite, whiteSprite, harmonySprite;
        [SerializeField] private MMFeedbacks blackMmFeedbacks;
        [SerializeField] private MMFeedbacks whiteMmFeedbacks;
        [SerializeField] private MMFeedbacks harmonyMmFeedbacks;
        [SerializeField] private MMFeedbacks activateBlackMmFeedbacks;
        [SerializeField] private MMFeedbacks activateWhiteMmFeedbacks;
        [SerializeField] private MMFeedbacks activateHarmonyMmFeedbacks;
        [SerializeField] private GameObject yinYangSplitWhitePrefab, yinYangSplitBlackPrefab;
        [SerializeField] private float splitOrbSpeed = 12.0f;
        [SerializeField] private float splitOrbDeviateSpeed = 3f;
        [SerializeField] private float splitOrbDeviateAngle = 20f;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => Level >= 2 ? (int)(_steps * 1.2f) : _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        private float WhiteDamage
        {
            get
            {
                return Level switch
                {
                    >= 3 => _whiteDamage * 1.2f,
                    _ => _whiteDamage
                };
            }
        }
        private float WhiteRadius
        {
            get
            {
                return Level switch
                {
                    >= 3 => _whiteRadius * 1.2f,
                    _ => _whiteRadius
                };
            }
        }
        private float BlackRadius
        {
            get
            {
                return Level switch
                {
                    >= 4 => _blackRadius * 1.2f,
                    _ => _blackRadius
                };
            }
        }
        
        // ExtraFields
        private static float _whiteDamage, _whiteRadius, _blackDamage, _blackRadius;
        
        // Other Variables
        private int _currentState;
        private bool _isActivated;
        private SpriteRenderer _sr;
        
        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            Spin();
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && Level == 5)
            {
                activateHarmonyMmFeedbacks.PlayFeedbacks();
            } 
            
            if (Input.GetMouseButtonDown(0) && _currentState == 0)
            {
                activateWhiteMmFeedbacks.PlayFeedbacks();
            }

            if (Input.GetMouseButtonDown(1) && _currentState == 0)
            {
                activateBlackMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;
            Disappear();
            
            switch (_currentState)
            {
                case 0:
                    defaultMmFeedbacks.PlayFeedbacks();
                    DealDamage();
                    break;
                case 1:
                    whiteMmFeedbacks.PlayFeedbacks();
                    DealWhiteDamage();
                    break;
                case 2:
                    blackMmFeedbacks.PlayFeedbacks();
                    DealBlackDamage();
                    break;
                case 3:
                    harmonyMmFeedbacks.PlayFeedbacks();
                    DealHarmonyDamage();
                    break;
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                Radius, 1, DestroyTypes.Circular);
        }

        private void DealWhiteDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, WhiteRadius, WhiteDamage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                WhiteRadius, 1, DestroyTypes.Circular);
            
            if (Level != 6) return;
            
            var derivedObject = Instantiate(yinYangSplitBlackPrefab, transform.position + Vector3.up, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(_blackDamage, BlackRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(Vector2.up,
                Random.Range(-splitOrbDeviateAngle, splitOrbDeviateAngle)) *
                                   (splitOrbSpeed + Random.Range(-splitOrbDeviateSpeed, splitOrbDeviateSpeed));
        }

        private void DealBlackDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, BlackRadius, _blackDamage, DamageHandler.DamageType.Circular);
            
            if (Level != 6) return;
            
            var derivedObject = Instantiate(yinYangSplitWhitePrefab, transform.position + Vector3.up, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(WhiteDamage, WhiteRadius, ExplosionDuration, ExplosionFX);
            derivedRb2d.velocity = Geometry.Rotate(Vector2.up,
                Random.Range(-splitOrbDeviateAngle, splitOrbDeviateAngle)) *
                                   (splitOrbSpeed + Random.Range(-splitOrbDeviateSpeed, splitOrbDeviateSpeed));
        }

        private void DealHarmonyDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, BlackRadius / 2, WhiteDamage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                BlackRadius / 2, 1, DestroyTypes.Circular);
        }

        public void ActivateWhiteMode()
        {
            _currentState = 1;
            _sr.sprite = whiteSprite;
        }

        public void ActivateBlackMode()
        {
            _currentState = 2;
            _sr.sprite = blackSprite;
        }
        
        public void ActivateHarmonyMode()
        {
            _currentState = 3;
            _sr.sprite = harmonySprite;
        }
        
        public override void SetParameters(float damage, float radius, 
            float maxMagnitude, int steps, float explosionDuration, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            _explosionDuration = explosionDuration;

            _explosionFX = GameAssets.i.gunpowderlessExplosionFX;
            
            _whiteDamage = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "whiteDamage").value;
            _whiteRadius = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "whiteRadius").value;
            _blackDamage = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "blackDamage").value;
            _blackRadius = Array.Find(extraWeaponTerms, ewt => 
                ewt.term == "blackRadius").value;
        }
        
    }
}