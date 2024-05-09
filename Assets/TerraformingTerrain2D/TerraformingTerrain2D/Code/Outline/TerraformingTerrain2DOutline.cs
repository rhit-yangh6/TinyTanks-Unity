using UnityEngine;

namespace TerraformingTerrain2d
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class TerraformingTerrain2DOutline : MonoBehaviour
    {
        [SerializeField] private bool _useOutline = true;
        [SerializeField] private int _sortingOrder;
        [SerializeField] private float _outlineTextureResolution = 1f;
        [SerializeField] private float _size = 0.1f;
        [SerializeField] private Color _color = new(0.67f, 0.23f, 0.05f);
        [SerializeField, HideInInspector] private Vector2Int _originalTextureSize;
        [SerializeField, HideInInspector] private MeshRenderer _meshRenderer;
        [SerializeField, HideInInspector] private Vector2 _rectangleSize;
        [SerializeField, HideInInspector] private Material _outlineView;
        private TerrainOutlineShader _shader;
        private Vector2Int _textureSize;

        public void Setup()
        {
            Compose();
            SetupTransform();
            SetupMeshRenderer();
        }

        private void SetupTransform()
        {
            gameObject.name = "Outline";
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
        }

        private void SetupMeshRenderer()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = _outlineView;
            OnValidate();
        }

        private void Compose()
        {
            _textureSize = Vector2Int.FloorToInt((Vector2)_originalTextureSize * _outlineTextureResolution);
            _outlineView = new Material(Shader.Find("OutlineView"));

            AccumulateTextures accumulateTextures = new(_textureSize);
            _shader = new TerrainOutlineShader(accumulateTextures);
            _shader.Initialize();
        }

        public void Initialize(Vector2Int textureSize)
        {
            _originalTextureSize = textureSize;
        }

        private void OnValidate()
        {
            if (_meshRenderer != null)
            {
                _meshRenderer.sortingOrder = _sortingOrder;
                if (_outlineView != null)
                {
                    _outlineView.SetColor("_MainColor", _color);
                }
            }
        }

        public void RebuildRectangle(Vector2 rectangleSize)
        {
            _rectangleSize = rectangleSize;
            GetComponent<MeshFilter>().sharedMesh = MeshExtensions.BuildQuad(_rectangleSize.x, _rectangleSize.y);
        }

        public void SetOutline(MinMax minMax, Vector2 position, float radius, TerraformingMode mode)
        {
            if (_useOutline)
            {
                int carveSign = mode == TerraformingMode.Carve ? 1 : -1;
                radius = mode == TerraformingMode.Carve ? radius + _size : radius;

                _shader.Update(transform, minMax, _textureSize, position, radius, _rectangleSize, carveSign);
                _shader.PassToView(_outlineView);
            }
        }

        public void Clear()
        {
            _shader.Clear();
        }
    }
}