using System;
using System.Collections;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class YinYangProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private Sprite blackSprite, whiteSprite;
        [SerializeField] private MMFeedbacks blackMmFeedbacks;
        [SerializeField] private MMFeedbacks whiteMmFeedbacks;
        [SerializeField] private MMFeedbacks activateBlackMmFeedbacks;
        [SerializeField] private MMFeedbacks activateWhiteMmFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // References
        protected override float Radius => _radius;
        protected override float Damage => _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
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

        public void DealWhiteDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, _whiteRadius, _whiteDamage, DamageHandler.DamageType.Circular);

            EventBus.Broadcast(EventTypes.DestroyTerrain, pos,
                _whiteRadius, 1, DestroyTypes.Circular);
        }

        public void DealBlackDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, _blackRadius, _blackDamage, DamageHandler.DamageType.Circular);
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

        private IEnumerator SwitchMode(int mode)
        {
            yield return new WaitForSeconds(0.05f);
            _currentState = mode;
            _sr.sprite = mode == 1 ? whiteSprite : blackSprite;
            _explosionFX = mode == 1 ? GameAssets.i.gunpowderlessExplosionFX : GameAssets.i.blackExplosionFX;
            yield return new WaitForSeconds(0.05f);
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