using UnityEngine;

namespace DemosShared
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsRestart : MonoBehaviour, IRestart
    {
        private Rigidbody2D _rigidbody;
        private Vector3 _startPosition;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _startPosition = transform.position;
        }

        public void Restart()
        {
            _rigidbody.velocity = Vector2.zero;
            transform.position = _startPosition;
        }
    }
}