using UnityEngine;

namespace DemosShared
{
    public class TransformRestart : IInitializable, IRestart
    {
        private readonly TrailRenderer _trailRenderer;
        private readonly Transform _transform;
        private Quaternion _startRotation;
        private Vector3 _startPosition;
        private Vector3 _startScale;
        private Transform _parent;

        public TransformRestart(Transform transform, TrailRenderer trailRenderer)
        {
            _trailRenderer = trailRenderer;
            _transform = transform;
        }

        void IInitializable.Initialize()
        {
            _startPosition = _transform.position;
            _startRotation = _transform.rotation;
            _startScale = _transform.localScale;
            _parent = _transform.parent;
        }

        void IRestart.Restart()
        {
            _transform.parent = _parent;
            _transform.position = _startPosition;
            _transform.rotation = _startRotation;
            _transform.localScale = _startScale;
            _trailRenderer.Clear();
        }
    }
}