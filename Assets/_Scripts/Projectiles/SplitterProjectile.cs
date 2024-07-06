using System;
using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace _Scripts.Projectiles
{
    public class SplitterProjectile : LaunchedProjectile
    {
        // Set in Inspector
        [SerializeField] private MMFeedbacks activateMmFeedbacks;
        
        // References
        protected override float Damage {
            get
            {
                if (Level >= 1) return damage * 1.38f;
                return Level >= 3 ? damage * 1.15f : damage;
            }
        }
        protected override int Steps => steps; // LEVEL 2
        
        // Other Variables
        private bool _isActivated;
    
        private void Start()
        {
        }

        private void Update()
        {
            Direct();
            if (!Input.GetMouseButtonDown(0) || _isActivated) return;
            
            _isActivated = true;
            activateMmFeedbacks.PlayFeedbacks();
        }

        public override void Activate()
        {
        }
    }
}
