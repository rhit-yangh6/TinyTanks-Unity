using System;
using _Scripts.Managers;
using Destructible2D;
using Destructible2D.Examples;
using UnityEngine;

namespace _Scripts.GameEngine.Map
{
    public class TerrainDeformer : MonoBehaviour
    {
        private const float RadiusMultiplier = 1.25f;

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
            switch (type)
            {
                case DestroyTypes.Circular:
                    DestroyTerrainCircular(pos, radius * RadiusMultiplier, destroyingPower);
                    break;
                case DestroyTypes.Square:
                    DestroyTerrainSquare(pos, radius * RadiusMultiplier, destroyingPower);
                    break;
                case DestroyTypes.Star:
                    DestroyTerrainStar(pos, radius * RadiusMultiplier, destroyingPower);
                    break;
                case DestroyTypes.Oval:
                    DestroyTerrainOval(pos, radius * RadiusMultiplier, destroyingPower);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void DestroyTerrainCircular(Vector3 pos, float radius, int destroyingPower = 1)
        {
            // var collider2Ds = Physics2D.OverlapCircleAll(pos, radius, layerMask);
            //
            // foreach (var c in collider2Ds)
            // {
            //     c.TryGetComponent(out TerraformingTerrain2dChunk chunk);
            //     chunk.TerraformingPresenter.Rebuild(pos, radius, TerraformingMode.Carve);
            // }

            var explosion = Instantiate(GameAssets.i.d2dCircleExplosion, pos, Quaternion.identity)
                .GetComponent<D2dExplosion>();
            explosion.StampSize *= radius;
        }
        
        private void DestroyTerrainSquare(Vector3 pos, float radius, int destroyingPower = 1)
        {
            // DestroyTerrainCircular(pos, radius, destroyingPower);
            //
            // var oneThirdRadius = radius / 3;
            // // Spawn four smaller explosions
            // DestroyTerrainCircular(pos + new Vector3(oneThirdRadius, oneThirdRadius),
            //     oneThirdRadius, destroyingPower);
            // DestroyTerrainCircular(pos + new Vector3(-oneThirdRadius, oneThirdRadius),
            //     oneThirdRadius, destroyingPower);
            // DestroyTerrainCircular(pos + new Vector3(-oneThirdRadius, -oneThirdRadius),
            //     oneThirdRadius, destroyingPower);
            // DestroyTerrainCircular(pos + new Vector3(oneThirdRadius, -oneThirdRadius),
            //     oneThirdRadius, destroyingPower);
            var explosion = Instantiate(GameAssets.i.d2dSquareExplosion, pos, Quaternion.identity)
                .GetComponent<D2dExplosion>();
            explosion.StampSize *= radius;
        }
        
        private void DestroyTerrainStar(Vector3 pos, float radius, int destroyingPower = 1)
        {
            var explosion = Instantiate(GameAssets.i.d2dStarExplosion, pos, Quaternion.identity)
                .GetComponent<D2dExplosion>();
            explosion.StampSize *= radius;
        }
        
        private void DestroyTerrainOval(Vector3 pos, float radius, int destroyingPower = 1)
        {
            var explosion = Instantiate(GameAssets.i.d2dOvalExplosion, pos, Quaternion.identity)
                .GetComponent<D2dExplosion>();
            explosion.StampSize *= radius;
        }
    }

    public enum DestroyTypes
    {
        Circular,
        Square,
        Star,
        Oval
    }
}
