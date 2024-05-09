using TerraformingTerrain2d;
using UnityEngine;

namespace RuntimeCarving
{
    public class WaterProjector : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _sortingOrder;

        private void Start()
        {
            Regenerate();
        }

        [ContextMenu("Regenerate")]
        private void Regenerate()
        {
            float height = 2f * _camera.orthographicSize;
            float width = height * _camera.aspect;

            _meshFilter.sharedMesh = MeshExtensions.BuildQuad(width, height);
        }

        private void OnValidate()
        {
            _meshRenderer.sortingOrder = _sortingOrder;
        }
    }
}