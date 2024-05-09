using UnityEngine;

namespace TerraformingTerrain2d
{
    public interface IScalarFieldInitializer
    {
        float Proceed(int x, int y, Vector2Int density);
    }
}