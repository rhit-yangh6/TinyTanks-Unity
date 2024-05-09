using System;
using TerraformingTerrain2d;
using UnityEngine;

namespace DemosShared
{
    [Serializable]
    public class BrushView
    {
        [SerializeField] [Min(0)] private float _brushRadius = 1;
        [SerializeField] private int _sortingOder;
        [SerializeField] private float _min;
        [SerializeField] private float _max;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        public float Radius
        {
            get => _brushRadius;
            set => SetupMesh(value);
        }

        public void Initialize(MonoBehaviour context)
        {
            _meshRenderer = context.GetComponent<MeshRenderer>();
            _meshFilter = context.GetComponent<MeshFilter>();
            _meshFilter.sharedMesh = MeshExtensions.BuildQuad(1);

            SetupRenderer();
            SetupMesh(_brushRadius);
        }

        private void SetupRenderer()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer.material = new Material(Shader.Find("Dunno/Circle"));
            }

            _meshRenderer.sortingOrder = _sortingOder;
        }

        private void SetupMesh(float brushRadius)
        {
            _brushRadius = Mathf.Clamp(brushRadius, _min, _max);
            _meshRenderer.transform.localScale = Vector3.one * _brushRadius;
        }
    }
}