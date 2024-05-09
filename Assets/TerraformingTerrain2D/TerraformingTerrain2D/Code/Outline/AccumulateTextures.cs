using UnityEngine;

namespace TerraformingTerrain2d
{
    public class AccumulateTextures
    {
        private readonly RenderTexture _accumulationTexture;
        private readonly RenderTexture _currentFrame;
        private readonly Material _clearMaterial;

        public AccumulateTextures(Vector2Int size)
        {
            _accumulationTexture = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.ARGB32);
            _currentFrame = new RenderTexture(size.x, size.y, 0, RenderTextureFormat.ARGB32);
            _clearMaterial = Resources.Load<Material>("ClearColor");
        }

        public RenderTexture Value => _accumulationTexture;

        public void Initialize()
        {
            _currentFrame.Create();
            _accumulationTexture.Create();
        }

        public void Clear()
        {
            Graphics.Blit(null, _accumulationTexture, _clearMaterial);
        }

        public void Accumulate(Material outlineMaterial)
        {
            outlineMaterial.SetTexture("_Mask", _accumulationTexture);
            Graphics.Blit(null, _currentFrame, outlineMaterial);
            Graphics.Blit(_currentFrame, _accumulationTexture);
        }
    }
}