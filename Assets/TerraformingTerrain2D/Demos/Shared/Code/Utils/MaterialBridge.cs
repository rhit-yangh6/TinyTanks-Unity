using System.Collections.Generic;
using UnityEngine;

namespace DemosShared
{
    public class MaterialBridge
    {
        private readonly Renderer _renderer;
        private readonly MaterialPropertyBlock _propertyBlock;
        private readonly Dictionary<string, int> _properties = new();

        public MaterialBridge(Renderer renderer)
        {
            _renderer = renderer;
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void UpdateRenderer()
        {
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetColor(string name, Color color)
        {
            _propertyBlock.SetColor(GetPropertyId(name), color);
        }

        public void SetVector(string name, Vector4 value)
        {
            _propertyBlock.SetVector(GetPropertyId(name), value);
        }

        public void SetTexture(string name, Texture texture)
        {
            _propertyBlock.SetTexture(GetPropertyId(name), texture);
        }

        public void SetFloat(string name, float value)
        {
            _propertyBlock.SetFloat(GetPropertyId(name), value);
        }

        public void SetFloatArray(string name, float[] value)
        {
            _propertyBlock.SetFloatArray(GetPropertyId(name), value);
        }

        public void SetInt(string name, int value)
        {
            _propertyBlock.SetInt(GetPropertyId(name), value);
        }

        public float GetFloat(string name)
        {
            return _propertyBlock.GetFloat(GetPropertyId(name));
        }

        private int GetPropertyId(string name)
        {
            if (_properties.ContainsKey(name) == false)
            {
                int id = Shader.PropertyToID(name);
                _properties[name] = id;
            }

            return _properties[name];
        }
    }
}