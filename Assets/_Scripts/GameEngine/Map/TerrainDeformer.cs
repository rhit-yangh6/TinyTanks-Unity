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
                DestroyTerrainCircular(pos, radius * RadiusMultiplier, 1);
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
    }

    public enum DestroyTypes
    {
        Circular,
        
        Square,
    }
}
