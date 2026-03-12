using System;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Networked player tank for multiplayer mode.
    /// Extends BuffableEntity for compatibility with existing buff/damage/death systems.
    /// Uses a sibling MultiplayerTankNetwork (NetworkBehaviour) for RPCs and NetworkVariables.
    /// </summary>
    public class MultiplayerTankController : BuffableEntity
    {
        private MultiplayerTankNetwork _net;
        private bool _canMove;
        private float _lastHorizontal;

        protected override void Start()
        {
            base.Start();
            _net = GetComponent<MultiplayerTankNetwork>();
            _net.Controller = this;
        }

        private void Update()
        {
            if (_net == null || !_net.IsOwner || !_net.IsMyTurn.Value || !_canMove)
                return;

            float horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) > 0.01f && HasFuel)
            {
                _net.MoveServerRpc(horizontal);
            }
        }

        private void FixedUpdate()
        {
            if (!_canMove || !HasFuel || _lastHorizontal == 0)
                FreezeOrFall();
            AdjustRotation();
        }

        /// <summary>
        /// Called by MultiplayerTankNetwork when host processes movement.
        /// </summary>
        public void ApplyMovement(float horizontal)
        {
            if (!HasFuel)
            {
                FreezeOrFall();
                return;
            }

            _lastHorizontal = horizontal;

            FlipToMatch(horizontal);
            ConsumeFuel();
            MoveOnSurface(horizontal);
        }

        /// <summary>
        /// Server-side damage application. Updates NetworkVariable for sync.
        /// </summary>
        public override void TakeDamage(float amount, bool isCriticalHit = false)
        {
            if (_net != null && !_net.IsServer) return;

            base.TakeDamage(amount, isCriticalHit);
            if (_net != null)
                _net.NetworkHealth.Value = Health;
        }

        /// <summary>
        /// Visual-only damage display for non-host clients.
        /// </summary>
        public override void TakeDamageVisualOnly(float amount, bool isCriticalHit)
        {
            var roundedAmount = (int)Math.Round(amount);
            DamagePopup.Create(Rigidbody2D.position, roundedAmount, isCriticalHit);
            if (_net != null)
                healthBar.SetHealth(_net.NetworkHealth.Value, MaxHealth);
        }

        public void OnNetworkHealthChanged(float oldValue, float newValue)
        {
            if (_net.IsServer) return;

            float damageAmount = oldValue - newValue;
            if (damageAmount > 0)
            {
                TakeDamageVisualOnly(damageAmount, false);
            }

            if (newValue <= 0 && !IsDead)
            {
                IsDead = true;
                OnDeath();
                Destroy(gameObject);
            }
        }

        public void OnTurnChanged(bool oldValue, bool newValue)
        {
            _canMove = newValue;
            if (newValue)
                ResetFuel();
        }

        public HealthBarBehavior HealthBarUI => healthBar;
    }
}
