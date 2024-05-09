using System;
using UnityEngine;

namespace DemosShared
{
    public class StarBallCollect : IOnTriggerEnter2D, IRestart
    {
        private readonly Action _collectInvoke;
        private bool _isCollected;

        public StarBallCollect(Action collectInvoke)
        {
            _collectInvoke = collectInvoke;
        }

        public void Entered(Collider2D collider2D)
        {
            if (collider2D.GetComponent<Ball>() && _isCollected == false)
            {
                _isCollected = true;
                _collectInvoke();
            }
        }

        public void Restart()
        {
            _isCollected = false;
        }
    }
}