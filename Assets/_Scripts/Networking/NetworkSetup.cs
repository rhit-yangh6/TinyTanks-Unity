using Steamworks;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Configures NetworkManager with FacepunchTransport.
    /// Attach to a prefab in Resources/ and mark DontDestroyOnLoad.
    /// </summary>
    public class NetworkSetup : MonoBehaviour
    {
        private static NetworkSetup _instance;
        public static NetworkSetup Instance => _instance;

        private FacepunchTransport _transport;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            var nm = GetComponent<NetworkManager>();
            _transport = GetComponent<FacepunchTransport>();

            if (nm == null || _transport == null)
            {
                Debug.LogError("[NetworkSetup] Missing NetworkManager or FacepunchTransport component.");
            }
        }

        /// <summary>
        /// Start as host (the lobby creator).
        /// </summary>
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("[NetworkSetup] Started as Host.");
        }

        /// <summary>
        /// Start as client, connecting to the host via their SteamId.
        /// </summary>
        public void StartClient(SteamId hostSteamId)
        {
            _transport.targetSteamId = hostSteamId;
            NetworkManager.Singleton.StartClient();
            Debug.Log($"[NetworkSetup] Started as Client, connecting to {hostSteamId}.");
        }

        /// <summary>
        /// Shutdown networking and clean up.
        /// </summary>
        public void Shutdown()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
            }

            Debug.Log("[NetworkSetup] Network shutdown.");
        }
    }
}
