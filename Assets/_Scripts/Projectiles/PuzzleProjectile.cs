using _Scripts.GameEngine.Map;
using _Scripts.GameEngine.WeaponExtraData;
using _Scripts.Managers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class PuzzleProjectile : LaunchedProjectile
    {
        [SerializeField] private GameObject topDownDisplay;
        
        private int _result;
        private PuzzleExtraData _puzzleExtraData;

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
            _result = Random.Range(0, 3);
        }
        
        public override void Detonate()
        {
            if (isDetonated) return;
            isDetonated = true;

            Disappear();
            HandleResult();
            defaultMmFeedbacks.PlayFeedbacks();
        }

        public override void DealDamage()
        {
            var isComplete = _puzzleExtraData.AddStatus(_result);
            EventBus.Broadcast(EventTypes.ExternalDisplayChange, _puzzleExtraData);
            var pos = transform.position;
            DamageHandler.i.HandleDamage(pos, Radius, Damage, DamageHandler.DamageType.Square);
            EventBus.Broadcast(EventTypes.DestroyTerrain, pos, Radius, 1, DestroyTypes.Puzzle);
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