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
        [SerializeField] private float whiteDamage = 33f;
        [SerializeField] private float whiteRadius = 2f;
        [SerializeField] private float blackDamage = 19f;
        [SerializeField] private float blackRadius = 8f;
        
        // References
        protected override int Steps => Level >= 2 ? (int)(steps * 1.2f) : steps;
        private float WhiteDamage
        {
            get
            {
                return Level switch
                {
                    >= 3 => whiteDamage * 1.2f,
                    _ => whiteDamage
                };
            }
        }
        private float WhiteRadius
        {
            get
            {
                return Level switch
                {
                    >= 3 => whiteRadius * 1.2f,
                    _ => whiteRadius
                };
            }
        }
        private float BlackRadius
        {
            get
            {
                return Level switch
                {
                    >= 4 => blackRadius * 1.2f,
                    _ => blackRadius
                };
            }
        }
        
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
            
            derivedProjectile.SetParameters(blackDamage, BlackRadius);
            derivedRb2d.velocity = Geometry.Rotate(Vector2.up,
                Random.Range(-splitOrbDeviateAngle, splitOrbDeviateAngle)) *
                                   (splitOrbSpeed + Random.Range(-splitOrbDeviateSpeed, splitOrbDeviateSpeed));
        }

        private void DealBlackDamage()
        {
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, BlackRadius, blackDamage, DamageHandler.DamageType.Circular);
            
            if (Level != 6) return;
            
            var derivedObject = Instantiate(yinYangSplitWhitePrefab, transform.position + Vector3.up, Quaternion.identity);
            var derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
            var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
            derivedProjectile.SetParameters(WhiteDamage, WhiteRadius);
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
    }
}