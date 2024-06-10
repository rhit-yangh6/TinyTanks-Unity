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
        [SerializeField] private float spikeDamage = 8f;
        [SerializeField] private float spikeRadius = 1f;
        
        // References
        protected override float Damage => Level >= 2 ? damage * 1.2f : damage;
        
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
                derivedProjectile.SetParameters(Damage, Radius);
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
        
                        derivedProjectile.SetParameters(spikeDamage, spikeRadius);
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
        
                        derivedProjectile.SetParameters(Level == 6 ? spikeDamage * 2 : spikeDamage, 
                            Level == 6 ? spikeRadius * 2 : spikeRadius);
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
        
                        derivedProjectile.SetParameters(spikeDamage, spikeRadius);
                        derivedRb2d.velocity = direction * 25f;
                    }

                    break;
                }
            }
        }
    }
}