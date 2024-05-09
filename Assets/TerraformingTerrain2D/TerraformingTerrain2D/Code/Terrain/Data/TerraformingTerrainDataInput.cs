using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [Serializable]
    public class TerraformingTerrainDataInput
    {
        [SerializeField] private Texture2D _sdfTexture;
        [SerializeField] private Material _material;
        
        public Texture2D SdfTexture => _sdfTexture;
        public Material Material => _material;
        public bool IsInitialized => _material != null && SdfTexture != null;
        
        public bool CheckIfValid()
        {
            if (_material != null && _material.shader.name != "Dunno/Terrain")
            {
                Debug.LogError("Material should be an instance of \"Dunno/Terrain\" shader");
                _material = null;
            }

            if (_sdfTexture != null && _sdfTexture.format != TextureFormat.Alpha8)
            {
                Debug.LogError("SDF texture should be a format of Alpha8");
                _sdfTexture = null;
            }

            return IsInitialized;
        }
    }
}