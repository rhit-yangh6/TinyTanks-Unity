using System.Collections;
using _Scripts.GameEngine;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Predefined quick chat messages for multiplayer.
    /// Sends message via ServerRpc, displayed as floating text above sender's tank.
    /// </summary>
    public class QuickChat : NetworkBehaviour
    {
        public static readonly string[] Messages =
        {
            "Nice Shot!",
            "Oops!",
            "Hurry Up!",
            "GG"
        };

        [SerializeField] private GameObject chatBubblePrefab;
        [SerializeField] private float displayDuration = 2.5f;

        private MultiplayerGameController _gameController;

        private void Start()
        {
            _gameController = FindAnyObjectByType<MultiplayerGameController>();
        }

        /// <summary>
        /// Send a quick chat message. Called from UI buttons.
        /// </summary>
        public void SendMessage(int messageIndex)
        {
            if (messageIndex < 0 || messageIndex >= Messages.Length) return;
            SendMessageServerRpc(messageIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendMessageServerRpc(int messageIndex, ServerRpcParams rpcParams = default)
        {
            // Find which player sent this
            ulong senderId = rpcParams.Receive.SenderClientId;
            int playerIndex = -1;

            if (_gameController != null)
            {
                var tanks = _gameController.Tanks;
                for (int i = 0; i < tanks.Count; i++)
                {
                    var tankNet = tanks[i] != null ? tanks[i].GetComponent<MultiplayerTankNetwork>() : null;
                    if (tankNet != null && tankNet.OwnerClientId == senderId)
                    {
                        playerIndex = i;
                        break;
                    }
                }
            }

            if (playerIndex >= 0)
            {
                DisplayMessageClientRpc(playerIndex, messageIndex);
            }
        }

        [ClientRpc]
        private void DisplayMessageClientRpc(int playerIndex, int messageIndex)
        {
            if (_gameController == null) return;
            if (messageIndex < 0 || messageIndex >= Messages.Length) return;

            var tanks = _gameController.Tanks;
            if (playerIndex < 0 || playerIndex >= tanks.Count) return;
            if (tanks[playerIndex] == null) return;

            StartCoroutine(ShowBubble(tanks[playerIndex].transform, Messages[messageIndex]));
        }

        private IEnumerator ShowBubble(Transform tankTransform, string message)
        {
            if (chatBubblePrefab == null) yield break;

            Vector3 offset = new Vector3(0, 2f, 0);
            GameObject bubble = Instantiate(chatBubblePrefab,
                tankTransform.position + offset, Quaternion.identity, tankTransform);

            var text = bubble.GetComponentInChildren<TMPro.TextMeshPro>();
            if (text != null) text.text = message;

            var textUI = bubble.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textUI != null) textUI.text = message;

            yield return new WaitForSeconds(displayDuration);

            if (bubble != null) Destroy(bubble);
        }
    }
}
