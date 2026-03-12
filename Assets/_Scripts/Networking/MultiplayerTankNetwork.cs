using System;
using Unity.Collections;
using Unity.Netcode;

namespace _Scripts.Networking
{
    /// <summary>
    /// NetworkBehaviour companion for MultiplayerTankController.
    /// Handles all RPCs and NetworkVariables since the tank extends BuffableEntity, not NetworkBehaviour.
    /// </summary>
    public class MultiplayerTankNetwork : NetworkBehaviour
    {
        [UnityEngine.HideInInspector] public MultiplayerTankController Controller;

        public NetworkVariable<int> PlayerIndex = new(
            writePerm: NetworkVariableWritePermission.Server);

        public NetworkVariable<float> NetworkHealth = new(
            writePerm: NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> IsMyTurn = new(
            writePerm: NetworkVariableWritePermission.Server);

        public NetworkVariable<NetworkString64> DisplayName = new(
            writePerm: NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkHealth.OnValueChanged += OnHealthChanged;
            IsMyTurn.OnValueChanged += OnTurnChanged;

            if (IsServer)
            {
                Controller = GetComponent<MultiplayerTankController>();
                if (Controller != null)
                    NetworkHealth.Value = Controller.Health;
            }
        }

        public override void OnNetworkDespawn()
        {
            NetworkHealth.OnValueChanged -= OnHealthChanged;
            IsMyTurn.OnValueChanged -= OnTurnChanged;
            base.OnNetworkDespawn();
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            if (Controller != null)
                Controller.OnNetworkHealthChanged(oldValue, newValue);
        }

        private void OnTurnChanged(bool oldValue, bool newValue)
        {
            if (Controller != null)
                Controller.OnTurnChanged(oldValue, newValue);
        }

        [ServerRpc]
        public void MoveServerRpc(float horizontal)
        {
            if (!IsMyTurn.Value) return;
            if (Controller == null) return;

            Controller.ApplyMovement(horizontal);
            FlipSyncClientRpc(Controller.FacingDirection);
        }

        [ClientRpc]
        private void FlipSyncClientRpc(int facingDirection)
        {
            if (IsServer) return;
            if (Controller == null) return;

            while (Controller.FacingDirection != facingDirection)
                Controller.Flip();
        }

        [ServerRpc]
        public void SetCannonAngleServerRpc(float angle)
        {
            if (Controller != null)
                Controller.SetCannonAngle(angle);
            SetCannonAngleClientRpc(angle);
        }

        [ClientRpc]
        private void SetCannonAngleClientRpc(float angle)
        {
            if (IsServer) return;
            if (Controller != null)
                Controller.SetCannonAngle(angle);
        }

        /// <summary>
        /// Struct for 64-char network strings (Steam names).
        /// </summary>
        public struct NetworkString64 : INetworkSerializable, IEquatable<NetworkString64>
        {
            private FixedString64Bytes _value;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _value);
            }

            public override string ToString() => _value.ToString();

            public static implicit operator NetworkString64(string s) =>
                new() { _value = new FixedString64Bytes(s) };

            public bool Equals(NetworkString64 other) => _value.Equals(other._value);
            public override int GetHashCode() => _value.GetHashCode();
        }
    }
}
