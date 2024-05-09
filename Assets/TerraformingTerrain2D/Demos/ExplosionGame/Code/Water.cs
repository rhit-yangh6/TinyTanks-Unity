using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    [ExecuteAlways]
    public class Water : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _minPoint;
        [SerializeField] private Transform _maxPoint;
        [SerializeField] private int _sortingOrder;
        private MaterialBridge _bridge;

        private void OnValidate()
        {
            _meshRenderer.sortingOrder = _sortingOrder;
        }

        private void Update()
        {
            _bridge ??= new MaterialBridge(_meshRenderer);
            
            _bridge.SetFloat("_RotationAngle", transform.eulerAngles.z * Mathf.Deg2Rad);
            _bridge.SetVector("_Scale", transform.lossyScale);
            _bridge.SetVector("_Position", transform.position);
            _bridge.SetVector("_RectangleSize", transform.lossyScale);
            _bridge.SetVector("_MinPoint", _minPoint.position);
            _bridge.SetVector("_MaxPoint", _maxPoint.position);
            _bridge.UpdateRenderer();
        }
    }
}