using UnityEngine;

namespace TerraformingTerrain2d
{
    public class SDFTextureToTerrainConverter : IScalarFieldInitializer
    {
        private readonly Texture2D _sdfTexture;
        private readonly float _sdfFactor;

        public SDFTextureToTerrainConverter(float sdfFactor, Texture2D sdfTexture)
        {
            _sdfTexture = sdfTexture;
            _sdfFactor = sdfFactor;
        }

        public float Proceed(int x, int y, Vector2Int density)
        {
            Vector2 uv = new Vector2(x + 0.5f, y + 0.5f) / density;

            Color color = _sdfTexture.GetPixelBilinear(uv.x, uv.y);

            return (1 - color.a) * _sdfFactor;
        }
    }
}