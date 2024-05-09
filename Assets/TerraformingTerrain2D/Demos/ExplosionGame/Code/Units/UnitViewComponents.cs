using System;
using UnityEngine;

namespace ExplosionGame
{
    [Serializable]
    public class UnitViewComponents
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _animator;

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
        public Rigidbody2D Rigidbody => _rigidbody2D;
        public Animator Animator => _animator;
    }
}