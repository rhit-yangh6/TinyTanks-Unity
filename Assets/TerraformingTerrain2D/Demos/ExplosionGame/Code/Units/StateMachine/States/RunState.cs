using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class RunState : State
    {
        private readonly MovementState _movementState;
        private readonly Rigidbody2D _rigidbody;
        private readonly float _movementSpeed;

        public RunState(AnimationState animationState, Rigidbody2D rigidbody, MovementState movementState, float speed)
            : base(animationState)
        {
            _rigidbody = rigidbody;
            _movementState = movementState;
            _movementSpeed = speed;
        }

        public override void FixedUpdate()
        {
            Vector2 slopeNormal = GetSlopeNormal();
            Vector3 localInputDirection = GetLocalInputDirection();
            _rigidbody.velocity = EvaluateVelocity(slopeNormal, localInputDirection);
        }

        private Vector2 GetSlopeNormal()
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(_rigidbody.position, 0.2f, -Vector2.up, 3);

            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.GetComponent<Ground>())
                {
                    return hits[i].normal;
                }
            }

            return Vector2.zero;
        }

        private Vector3 GetLocalInputDirection()
        {
            return _rigidbody.transform.TransformDirection(new Vector3(_movementState.Direction.x,
                _movementState.Direction.y, 0));
        }

        private Vector3 EvaluateVelocity(Vector3 slopeNormal, Vector3 localDirection)
        {
            return Vector3.Cross(Vector3.Cross(slopeNormal, localDirection), slopeNormal).normalized * _movementSpeed;
        }
    }
}