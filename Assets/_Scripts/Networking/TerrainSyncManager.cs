using _Scripts.GameEngine.Map;
using _Scripts.Managers;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Syncs terrain deformation from host to all clients.
    /// Host listens for DestroyTerrain events and broadcasts via ClientRpc.
    /// Clients replay the same deformation locally (Destructible2D is deterministic).
    /// </summary>
    public class TerrainSyncManager : NetworkBehaviour
    {
        private TerrainDeformer _terrainDeformer;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _terrainDeformer = FindAnyObjectByType<TerrainDeformer>();

            if (IsServer)
            {
                EventBus.AddListener<Vector3, float, int, DestroyTypes>(
                    EventTypes.DestroyTerrain, OnTerrainDestroyedOnHost);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                EventBus.RemoveListener<Vector3, float, int, DestroyTypes>(
                    EventTypes.DestroyTerrain, OnTerrainDestroyedOnHost);
            }

            base.OnNetworkDespawn();
        }

        /// <summary>
        /// Host intercepted a terrain destroy event. Broadcast to all clients.
        /// The host's own TerrainDeformer already handles the event via EventBus,
        /// so we only need to send it to non-host clients.
        /// </summary>
        private void OnTerrainDestroyedOnHost(Vector3 pos, float radius, int power, DestroyTypes type)
        {
            DestroyTerrainClientRpc(pos, radius, power, (int)type);
        }

        [ClientRpc]
        private void DestroyTerrainClientRpc(Vector3 pos, float radius, int power, int type)
        {
            // Host already handled this via EventBus, skip
            if (IsServer) return;

            if (_terrainDeformer != null)
            {
                _terrainDeformer.DestroyTerrainDirect(pos, radius, power, (DestroyTypes)type);
            }
        }
    }
}
