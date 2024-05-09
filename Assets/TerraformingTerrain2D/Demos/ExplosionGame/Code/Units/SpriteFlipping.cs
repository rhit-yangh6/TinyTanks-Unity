using DemosShared;
using UnityEngine;

namespace ExplosionGame
{
    public class SpriteFlipping : IUpdate
    {
        private readonly SpriteRenderer _spriteRenderer;
        private readonly MovementState _movementState;
        private readonly WeaponHolder _weaponHolder;

        public SpriteFlipping(WeaponHolder weaponHolder, MovementState movementState, SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
            _movementState = movementState;
            _weaponHolder = weaponHolder;
        }

        void IUpdate.Update()
        {
            Vector3 right = _weaponHolder.Current.transform.right;
            float cross = GetCross(right, Vector2.up);
            bool isRight = cross >= 0;
            
            _spriteRenderer.flipX = isRight;
            
            Vector3 scale = _weaponHolder.Current.transform.localScale;
            scale.y = -Mathf.Abs(scale.y) * Mathf.Sign(cross);
            _weaponHolder.Current.transform.localScale = scale;
        }
        
        float GetCross(Vector2 A, Vector2 B)
        {
            return -A.x * B.y + A.y * B.x;
        }
    }
}