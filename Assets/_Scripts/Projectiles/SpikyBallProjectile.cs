using System;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SpikyBallProjectile: LaunchedProjectile
    {
         // Set in Inspector
        [SerializeField] private GameObject ballPrefab, spikePrefab;
        [SerializeField] private MMFeedbacks activateFeedbacks;
        
        // Shared Fields
        private static float _radius, _damage, _maxMagnitude, _explosionDuration;
        private static int _steps;
        private static GameObject _explosionFX;

        // ExtraFields
        private static float _spikeDamage, _spikeRadius;
        
        // References
        protected override float Radius => _radius;
        protected override float Damage => Level >= 2 ? _damage * 1.2f : _damage;
        protected override float MaxMagnitude => _maxMagnitude;
        protected override int Steps => _steps;
        protected override float ExplosionDuration => _explosionDuration;
        protected override GameObject ExplosionFX => _explosionFX;
        
        // Other Variables
        private bool _isActivated;
        private readonly int[,] _fourDirections = 
            { { 1, 0 }, { 0, -1 }, { -1, 0 }, { 0, 1 } };
        private readonly int[,] _eightDirections = 
            { { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 }, {-2, 0}, {-1, 1}, {0, 2}, {1, 1} };
        private readonly int[,] _twelveDirections = 
            { { 3, 0 }, { 2, -1 }, { 1, -2 }, { 0, -3 }, { -1, -2 }, { -2, -1 }, { -3, 0 }, {-2, 1}, {-1, 2}, {0, 3}, {1, 2}, {2, 1} };
        

        private void Update()
        {
            Spin();

            if (Input.GetMouseButtonDown(0) && !_isActivated)
            {
                _isActivated = true;
                Disappear();
                activateFeedbacks.PlayFeedbacks();
            }
        }

        public override void Activate()
        {
            GameObject derivedObject;
            DerivedProjectile derivedProjectile;

            if (Level >= 3)
            {
                derivedObject = Instantiate(ballPrefab, transform.position, Quaternion.identity);
                derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                derivedProjectile.SetParameters(Damage, Radius, ExplosionDuration, ExplosionFX);
            }


            switch (Level)
            {
                case 5:
                {
                    for (var i = 0; i < 12; i++)
                    {
                        var direction = Vector3.Normalize(Vector2.right * _twelveDirections[i, 0] +
                                                          Vector2.up * _twelveDirections[i, 1]);
                
                        derivedObject = Instantiate(spikePrefab, transform.position, Quaternion.identity);
                        derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                        var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
        
                        derivedProjectile.SetParameters(_spikeDamage, _spikeRadius, ExplosionDuration, ExplosionFX);
                        derivedRb2d.velocity = direction * 25f;
                    }

                    break;
                }
                case >= 4:
                {
                    for (var i = 0; i < 8; i++)
                    {
                        var direction = Vector3.Normalize(Vector2.right * _eightDirections[i, 0] +
                                                          Vector2.up * _eightDirections[i, 1]);
                
                        derivedObject = Instantiate(spikePrefab, transform.position, Quaternion.identity);
                        derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                        var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
        
                        derivedProjectile.SetParameters(Level == 6 ? _spikeDamage * 2 : _spikeDamage, 
                            Level == 6 ? _spikeRadius * 2 : _spikeRadius, ExplosionDuration, ExplosionFX);
                        derivedRb2d.velocity = direction * 25f;
                    }

                    break;
                }
                default:
                {
                    for (var i = 0; i < 4; i++)
                    {
                        var direction = Vector3.Normalize(Vector2.right * _fourDirections[i, 0] +
                                                          Vector2.up * _fourDirections[i, 1]);
                
                        derivedObject = Instantiate(spikePrefab, transform.position, Quaternion.identity);
                        derivedProjectile = derivedObject.GetComponent<DerivedProjectile>();
                        var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
        
                        derivedProjectile.SetParameters(_spikeDamage, _spikeRadius, ExplosionDuration, ExplosionFX);
                        derivedRb2d.velocity = direction * 25f;
                    }

                    break;
                }
            }
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

            _spikeDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "spikeDamage").value;
            _spikeRadius = Array.Find(extraWeaponTerms, ewt => ewt.term == "spikeRadius").value;
        }
    }
}