using System;
using _Scripts.Managers;
using TerraformingTerrain2d;
using UnityEngine;

namespace _Scripts.GameEngine.Map
{
    public class TerrainDeformer : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        private const float RadiusMultiplier = 2.0f;

        private void Start()
        {
            EventBus.AddListener<Vector3, float, int, DestroyTypes>(EventTypes.DestroyTerrain, DestroyTerrainHandler);
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener<Vector3, float, int, DestroyTypes>(EventTypes.DestroyTerrain, DestroyTerrainHandler);
        }

        private void DestroyTerrainHandler(Vector3 pos, float radius, int destroyingPower, DestroyTypes type)
        {
            if (type == DestroyTypes.Circular)
            {
                DestroyTerrainCircular(pos, radius * RadiusMultiplier, destroyingPower);
            }
            else if (type == DestroyTypes.Square)
            {
                DestroyTerrainSquare(pos, radius * RadiusMultiplier, destroyingPower);
            }
            // TODO change type
        }

        private void DestroyTerrainCircular(Vector3 pos, float radius, int destroyingPower = 1)
        {
            var collider2Ds = Physics2D.OverlapCircleAll(pos, radius, layerMask);

            foreach (var c in collider2Ds)
            {
                // TODO: Was exploded?
                c.TryGetComponent(out TerraformingTerrain2dChunk chunk);
                chunk.TerraformingPresenter.Rebuild(pos, radius, TerraformingMode.Carve);
            }
        }
        
        private void DestroyTerrainSquare(Vector3 pos, float radius, int destroyingPower = 1)
        {
            DestroyTerrainCircular(pos, radius, destroyingPower);

            var oneThirdRadius = radius / 3;
            // Spawn four smaller explosions
            DestroyTerrainCircular(pos + new Vector3(oneThirdRadius, oneThirdRadius),
                oneThirdRadius, destroyingPower);
            DestroyTerrainCircular(pos + new Vector3(-oneThirdRadius, oneThirdRadius),
                oneThirdRadius, destroyingPower);
            DestroyTerrainCircular(pos + new Vector3(-oneThirdRadius, -oneThirdRadius),
                oneThirdRadius, destroyingPower);
            DestroyTerrainCircular(pos + new Vector3(oneThirdRadius, -oneThirdRadius),
                oneThirdRadius, destroyingPower);
        }
    }

    public enum DestroyTypes
    {
        Circular,
        Square,
    }
}
