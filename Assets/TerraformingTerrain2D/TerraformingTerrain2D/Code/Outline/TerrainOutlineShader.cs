using UnityEngine;

namespace TerraformingTerrain2d
{
    public class TerrainOutlineShader
    {
        private readonly AccumulateTextures _accumulateTextures;
        private readonly Material _outlineMaterial;

        public TerrainOutlineShader(AccumulateTextures accumulateTextures)
        {
            _accumulateTextures = accumulateTextures;
            _outlineMaterial = Resources.Load<Material>("OutlineAccumulation");
        }

        public void Initialize()
        {
            _accumulateTextures.Initialize();
        }

        public void Update(Transform transform, MinMax minMax, Vector2Int textureSize, Vector2 position, float radius,
            Vector2 rectangleSize, int carveSign)
        {
            _outlineMaterial.SetVector("_TextureSize", new Vector4(textureSize.x, textureSize.y, 0, 0));
            _outlineMaterial.SetVector("_MouseData", new Vector4(position.x, position.y, radius, 0));
            _outlineMaterial.SetVector("_MinPoint", minMax.Min);
            _outlineMaterial.SetVector("_MaxPoint", minMax.Max);
            _outlineMaterial.SetFloat("_RotationAngle", transform.eulerAngles.z * Mathf.Deg2Rad);
            _outlineMaterial.SetVector("_Scale", transform.lossyScale);
            _outlineMaterial.SetVector("_Position", transform.position);
            _outlineMaterial.SetVector("_RectangleSize", rectangleSize);
            _outlineMaterial.SetInt("_CarveSign", carveSign);

            _accumulateTextures.Accumulate(_outlineMaterial);
        }

        public void PassToView(Material viewMaterial)
        {
            viewMaterial.SetTexture("_Mask", _accumulateTextures.Value);
        }

        public void Clear()
        {
            _accumulateTextures.Clear();
        }
    }
}