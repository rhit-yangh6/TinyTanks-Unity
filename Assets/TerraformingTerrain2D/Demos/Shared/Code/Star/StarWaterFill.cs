using System;
using System.Collections.Generic;
using UnityEngine;

namespace DemosShared
{
    public class StarWaterFill : IOnTriggerEnter2D, IRestart
    {
        private static readonly int _fill = Shader.PropertyToID("_Fill");
        private readonly HashSet<Collider2D> _collidedObjects = new();
        private readonly SpriteRenderer _spriteRenderer;
        private readonly int _particlesToCount = 15;
        private readonly Action _collectEvent;
        private bool _isCollected;
        private int _count;

        public StarWaterFill(SpriteRenderer spriteRenderer, Action collectEvent)
        {
            _spriteRenderer = spriteRenderer;
            _collectEvent = collectEvent;
        }

        void IOnTriggerEnter2D.Entered(Collider2D collider2D)
        {
            if (_collidedObjects.Contains(collider2D) == false && collider2D.GetComponent<Ball>() == false)
            {
                _collidedObjects.Add(collider2D);
                _count++;

                if (_count <= _particlesToCount)
                {
                    _spriteRenderer.material.SetFloat(_fill, (float)_count / _particlesToCount);
                }
                else if (_isCollected == false)
                {
                    _isCollected = true;
                    _collectEvent();
                }
            }
        }

        public void ShowStar()
        {
            _spriteRenderer.material.SetFloat(_fill, 0);
        }

        void IRestart.Restart()
        {
            _spriteRenderer.gameObject.SetActive(true);
            ShowStar();
            _count = 0;
            _collidedObjects.Clear();
            _isCollected = false;
        }
    }
}