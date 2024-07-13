using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.GameEngine.Map;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class PuzzleProjectile : LaunchedProjectile
    {
        [SerializeField] private GameObject topDownDisplay;
        [SerializeField] private float completionRadiusMultiplier;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float completionDamageMultiplier;
        [SerializeField] private float newPieceChance = 0.35f;
        [SerializeField] private GameObject fallPiecePrefab;
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        [SerializeField] private float skyFallAngleDeviation = 10f;
        
        private const float YOffset = -2f;
        private int _result;
        private PuzzleExtraData _puzzleExtraData;
        
        protected override float Radius => Level >= 2 ? radius * 1.25f : radius;

        private void Start()
        {
            _puzzleExtraData = (PuzzleExtraData) WeaponExtraData;
        }

        private void Update()
        {
            Spin(1.8f);
        }

        private void HandleResult()
        {
            var possibleResults = new HashSet<int>() {0, 1, 2, 3};
            var currentStatus = _puzzleExtraData.GetStatus();
            possibleResults.ExceptWith(currentStatus);
            var possibleResultsList = possibleResults.ToList();
            _result = Level switch
            {
                5 => possibleResultsList[Random.Range(0, possibleResultsList.Count - 1)],
                >= 3 => Random.value < newPieceChance
                    ? possibleResultsList[Random.Range(0, possibleResultsList.Count - 1)]
                    : Random.Range(0, 3),
                _ => Random.Range(0, 3)
            };
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();

            if (_puzzleExtraData.IsComplete())
            {
                activateMmFeedbacks.PlayFeedbacks();
            }
            else
            {
                HandleResult();
                defaultMmFeedbacks.PlayFeedbacks();
            }
        }

        public override void DealDamage()
        {
            var pos = transform.position;
            _puzzleExtraData.AddStatus(_result);
            EventBus.Broadcast(EventTypes.ExternalDisplayChangePuzzle, _puzzleExtraData);
            
            if (Level >= 4 && _puzzleExtraData.IsComplete())
            {
                DealCompletionDamage();
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Square);
                EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Puzzle);
            }
        }

        public void DealCompletionDamage()
        {
            var pos = transform.position;
            _puzzleExtraData.ClearStatus();
            EventBus.Broadcast(EventTypes.ExternalDisplayChangePuzzle, _puzzleExtraData);
            
            // Sky fall?
            if (Level == 6)
            {
                StartCoroutine(SkyFall());
            }
            else
            {
                DamageHandler.i.HandleDamage(pos, Radius * completionRadiusMultiplier, 
                    Damage * completionDamageMultiplier, DamageHandler.DamageType.Square);
                EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius * completionRadiusMultiplier,
                    1, DestroyTypes.Puzzle);
            }
        }

        private IEnumerator SkyFall()
        {
            var pos = transform.position;
            // RayCast to sky
            var hit = Physics2D.Raycast(pos, Vector2.up, 1000, layerMask);
            
            // Calculate missile spawn point
            var spawnPos = new Vector2(hit.point.x, hit.point.y + YOffset);
            for (var i = 0; i < 4; i++)
            {
                var derivedObject = Instantiate(fallPiecePrefab, spawnPos, Quaternion.identity);
                var derivedProjectile = derivedObject.GetComponent<PuzzleFallProjectile>();
                var derivedRb2d = derivedObject.GetComponent<Rigidbody2D>();
            
                derivedProjectile.SetParameters(Damage, Radius);
                derivedProjectile.ChangeSprite(i);
                derivedRb2d.velocity = Geometry.Rotate(Vector2.down, 
                    Random.Range(-skyFallAngleDeviation, skyFallAngleDeviation));

                yield return new WaitForSeconds(0.1f);
            }
        }
        
        public void DisplayTopDownResult()
        {
            var cameraCenterPos = Camera.main.transform.position;
            var insTopdown = Instantiate(topDownDisplay, 
                new Vector3(cameraCenterPos.x, cameraCenterPos.y, 0), 
                quaternion.identity);
            insTopdown.GetComponent<SpriteRenderer>().sprite = GameAssets.i.puzzlePieces[_result];
            insTopdown.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-90f, 90f)));
            Destroy(insTopdown, 1.9f);
        }
    }
}