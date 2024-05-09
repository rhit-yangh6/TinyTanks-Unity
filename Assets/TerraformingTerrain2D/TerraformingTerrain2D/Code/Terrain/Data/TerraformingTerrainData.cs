using System;
using UnityEngine;

namespace TerraformingTerrain2d
{
    [Serializable]
    public class TerraformingTerrainData
    {
        [SerializeField] [Min(1)] private int _chunkCountX = 1;
        [SerializeField] [Min(1)] private int _chunkCountY = 1;
        [SerializeField] [Min(2)] private int _density = 30;
        [SerializeField] [Min(2)] private float _scale = 5;
        [SerializeField] private GridConfigurationPresenter _configuration;
        [SerializeField] private TerraformingTerrainDataInput _input;
        [SerializeField] private MultithreadingUsage _multithreading;
        [SerializeField] private Vector2Int _generatedChunksCount;
        [SerializeField] private TerrainGizmoData _gizmoData;
        [SerializeField] private float _sdfFactor = 1.3f;
        [SerializeField] private Transform _transform;
        [SerializeField] private IsoValue _isoValue;
        [SerializeField] private int _sortingOrder;

        public bool RequiredToRegenerate => _generatedChunksCount == Vector2Int.zero;
        public bool UseMultiThreading => _multithreading.UseMultithreading;
        public GridConfigurationPresenter Configuration => _configuration;
        public Vector2Int ChunkCount => new(_chunkCountX, _chunkCountY);
        public Vector2Int GeneratedChunksCount => _generatedChunksCount;
        public TerraformingTerrainDataInput Input => _input;
        public Texture2D SdfTexture { get; private set; }
        public TerrainGizmoData GizmoData => _gizmoData;
        public Transform Transform => _transform;
        public int SortingOrder => _sortingOrder;
        public IsoValue IsoValue => _isoValue;
        public float SdfFactor => _sdfFactor;
        public int Density => _density;
        public float Scale => _scale;

        public void PassTransform(Transform transform)
        {
            _transform = transform;
        }

        public void SaveGeneratedChunksCount()
        {
            _generatedChunksCount = ChunkCount;
        }

        public void OnValidate()
        {
            _multithreading.CheckCompability(_transform);
            
            _density = Mathf.Max(_density, Mathf.Min(_generatedChunksCount.x, _generatedChunksCount.y));
            SdfTexture = _input.SdfTexture;

            if (_configuration.IsInUse && _configuration.CheckIfValid(_generatedChunksCount))
            {
                _density = _configuration.Preset.Density;
                _sdfFactor = _configuration.Preset.SDFFactor;
                SdfTexture = _configuration.Preset.SDFTexture;
            }

            GridData gridData = new(_density, SdfTexture.GetRatio(), _scale);
            _sdfFactor = Mathf.Max(_sdfFactor, _isoValue.Value + 0.001f);
            _chunkCountX = Mathf.Min(_chunkCountX, gridData.XDensity - 1);
            _chunkCountY = Mathf.Min(_chunkCountY, gridData.YDensity - 1);
        }
    }
}