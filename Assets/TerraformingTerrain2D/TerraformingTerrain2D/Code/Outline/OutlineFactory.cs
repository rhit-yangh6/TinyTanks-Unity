using UnityEngine;

namespace TerraformingTerrain2d
{
    public class OutlineFactory
    {
        private readonly Transform _parent;
        private readonly Vector2Int _textureSize;

        public OutlineFactory(Transform parent, Vector2Int textureSize)
        {
            _parent = parent;
            _textureSize = textureSize;
        }

        public TerraformingTerrain2DOutline Create()
        {
            TerraformingTerrain2DOutline outline = _parent.GetComponentInChildren<TerraformingTerrain2DOutline>(true);

            if (outline == null)
            {
                GameObject gameObject = new();
                outline = gameObject.AddComponent<TerraformingTerrain2DOutline>();
                outline.transform.parent = _parent;
            }

            outline.Initialize(_textureSize);

            return outline;
        }
    }
}