using System.Collections.Generic;
using System.Linq;
using _Scripts.GameEngine;
using _Scripts.Networking;
using _Scripts.Utils;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI.Multiplayer
{
    /// <summary>
    /// Lobby screen UI for multiplayer.
    /// Handles lobby creation, friend invites, player slots, map selection, and game start.
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        [Header("Lobby Controls")]
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button inviteFriendButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button leaveLobbyButton;

        [Header("Player Slots")]
        [SerializeField] private GameObject[] playerSlots;
        [SerializeField] private TextMeshProUGUI[] playerNameTexts;
        [SerializeField] private Image[] playerAvatarImages;
        [SerializeField] private GameObject waitingForPlayersText;

        [Header("Map Selection")]
        [SerializeField] private TMP_Dropdown mapDropdown;

        [Header("Friend Picker (fallback when overlay unavailable)")]
        [SerializeField] private GameObject friendPickerPanel;
        [SerializeField] private Transform friendListContent;
        [SerializeField] private GameObject friendEntryPrefab;

        [Header("Debug")]
        [SerializeField] private bool allowSoloStart;
        [SerializeField] private bool debugSimulateGuest;

        [Header("Status")]
        [SerializeField] private TextMeshProUGUI statusText;

        private SteamLobbyManager _lobbyManager;

        private void Start()
        {
            _lobbyManager = SteamLobbyManager.Instance;

            createLobbyButton.onClick.AddListener(OnCreateLobby);
            inviteFriendButton.onClick.AddListener(OnInviteFriend);
            startGameButton.onClick.AddListener(OnStartGame);
            leaveLobbyButton.onClick.AddListener(OnLeaveLobby);


            _lobbyManager.OnLobbyCreated += RefreshLobbyDisplay;
            _lobbyManager.OnLobbyJoined += OnLobbyJoinedHandler;
            _lobbyManager.OnLobbyMemberJoined += RefreshLobbyDisplay;
            _lobbyManager.OnLobbyMemberLeft += RefreshLobbyDisplay;
            _lobbyManager.OnLobbyLeft += OnLobbyLeftHandler;
            _lobbyManager.OnFriendListRequested += ShowFriendPicker;

            // Initially hide lobby controls
            SetLobbyUIActive(false);
            startGameButton.interactable = false;
            inviteFriendButton.interactable = false;
            if (friendPickerPanel != null)
                friendPickerPanel.SetActive(false);

            // If we're already in a lobby (e.g. joined via invite before panel activated),
            // immediately show the correct UI state.
            if (_lobbyManager.CurrentLobby.HasValue || debugSimulateGuest)
            {
                OnLobbyJoinedHandler();
            }
        }

        private void OnDestroy()
        {
            if (_lobbyManager != null)
            {
                _lobbyManager.OnLobbyCreated -= RefreshLobbyDisplay;
                _lobbyManager.OnLobbyJoined -= OnLobbyJoinedHandler;
                _lobbyManager.OnLobbyMemberJoined -= RefreshLobbyDisplay;
                _lobbyManager.OnLobbyMemberLeft -= RefreshLobbyDisplay;
                _lobbyManager.OnLobbyLeft -= OnLobbyLeftHandler;
                _lobbyManager.OnFriendListRequested -= ShowFriendPicker;
            }
        }

        private async void OnCreateLobby()
        {
            statusText.text = "Creating lobby...";
            bool success = await _lobbyManager.CreateLobby(4);
            if (success)
            {
                SetLobbyUIActive(true);
                createLobbyButton.gameObject.SetActive(false);
                inviteFriendButton.interactable = true;

                // Store local player's weapon loadout
                StoreLocalWeaponLoadout();

                statusText.text = "Lobby created! Invite friends to join.";
            }
            else
            {
                statusText.text = "Failed to create lobby.";
            }
        }

        private void OnInviteFriend()
        {
            _lobbyManager.InviteFriend();
        }

        private void OnStartGame()
        {
            if (!_lobbyManager.IsHost) return;
            if (!_lobbyManager.CurrentLobby.HasValue) return;

            var lobby = _lobbyManager.CurrentLobby.Value;
            var members = lobby.Members.ToArray();

            bool soloWithBot = members.Length < 2 && allowSoloStart;

            if (members.Length < 2 && !allowSoloStart)
            {
                statusText.text = "Need at least 2 players to start!";
                return;
            }

            // Lock lobby
            _lobbyManager.LockLobby();

            // Populate session data
            MultiplayerSessionData.Clear();
            MultiplayerSessionData.IsMultiplayer = true;
            MultiplayerSessionData.IsHost = true;
            MultiplayerSessionData.IsBotMode = soloWithBot;
            MultiplayerSessionData.PlayerCount = soloWithBot ? 2 : members.Length;
            MultiplayerSessionData.LocalPlayerIndex = 0;

            // Player 0 is always the local host
            MultiplayerSessionData.PlayerSteamIds[0] = SteamClient.SteamId;
            MultiplayerSessionData.PlayerNames[0] = SteamClient.Name;
            StoreLocalWeaponLoadoutToSession(0);

            if (soloWithBot)
            {
                // Add bot as player 1
                MultiplayerSessionData.PlayerNames[1] = "Bot";
                MultiplayerSessionData.PlayerWeapons[1] = new[] { (-1, 1) };
            }
            else
            {
                // Fill in real players (host is already at index 0)
                int idx = 1;
                for (int i = 0; i < members.Length; i++)
                {
                    if (members[i].Id == SteamClient.SteamId) continue;

                    MultiplayerSessionData.PlayerSteamIds[idx] = members[i].Id;
                    MultiplayerSessionData.PlayerNames[idx] = members[i].Name;

                    string weaponData = _lobbyManager.GetMemberWeaponLoadout(members[i]);
                    MultiplayerSessionData.PlayerWeapons[idx] =
                        MultiplayerSessionData.ParseWeaponLoadout(weaponData);
                    idx++;
                }
            }

            // Start networking
            NetworkSetup.Instance.StartHost();

            // Load multiplayer scene via NetworkManager
            Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene(
                "Multiplayer", LoadSceneMode.Single);
        }

        private void OnLeaveLobby()
        {
            _lobbyManager.LeaveLobby();
            NetworkSetup.Instance.Shutdown();
        }

        private void StoreLocalWeaponLoadoutToSession(int playerIndex)
        {
            var selectedWeapons = PlayerData.Instance.selectedWeapons;
            var weapons = new (int weaponId, int level)[5];
            for (int i = 0; i < 5; i++)
            {
                if (selectedWeapons[i] != null)
                    weapons[i] = (selectedWeapons[i].weaponId, selectedWeapons[i].level);
            }
            MultiplayerSessionData.PlayerWeapons[playerIndex] = weapons;
        }

        private void OnLobbyJoinedHandler()
        {
            // Set up UI state for non-host (joined via invite)
            if (!_lobbyManager.IsHost || debugSimulateGuest)
            {
                SetLobbyUIActive(true);
                createLobbyButton.gameObject.SetActive(false);
                startGameButton.gameObject.SetActive(false);
                inviteFriendButton.interactable = false;
                if (mapDropdown != null)
                    mapDropdown.interactable = false;
                statusText.text = "Joined lobby! Waiting for host to start...";
            }

            // Skip refresh if no real lobby (debug mode)
            if (_lobbyManager.CurrentLobby.HasValue)
                RefreshLobbyDisplay();
        }

        private void OnLobbyLeftHandler()
        {
            SetLobbyUIActive(false);
            createLobbyButton.gameObject.SetActive(true);
            statusText.text = "Left lobby.";
            ClearPlayerSlots();
        }

        private void RefreshLobbyDisplay()
        {
            if (!_lobbyManager.CurrentLobby.HasValue) return;

            var lobby = _lobbyManager.CurrentLobby.Value;
            var members = lobby.Members.ToArray();

            ClearPlayerSlots();

            for (int i = 0; i < members.Length && i < playerSlots.Length; i++)
            {
                playerSlots[i].SetActive(true);
                if (playerNameTexts[i] != null)
                    playerNameTexts[i].text = members[i].Name;
                if (i < playerAvatarImages.Length && playerAvatarImages[i] != null)
                    LoadAvatarAsync(members[i].Id, playerAvatarImages[i]);
            }

            // Host-only controls
            bool isHost = _lobbyManager.IsHost;
            startGameButton.gameObject.SetActive(isHost);
            startGameButton.interactable = isHost && (members.Length >= 2 || allowSoloStart);
            if (mapDropdown != null)
                mapDropdown.interactable = isHost;

            if (waitingForPlayersText != null)
                waitingForPlayersText.SetActive(members.Length < 2);

            // Store loadout on join
            StoreLocalWeaponLoadout();
        }

        private void ClearPlayerSlots()
        {
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].SetActive(false);
                if (playerNameTexts[i] != null)
                    playerNameTexts[i].text = "";
                if (i < playerAvatarImages.Length && playerAvatarImages[i] != null)
                    playerAvatarImages[i].sprite = null;
            }
        }

        private void SetLobbyUIActive(bool active)
        {
            inviteFriendButton.gameObject.SetActive(active);
            startGameButton.gameObject.SetActive(active);
            leaveLobbyButton.gameObject.SetActive(active);
            if (mapDropdown != null)
                mapDropdown.gameObject.SetActive(active);
        }

        private void StoreLocalWeaponLoadout()
        {
            var selectedWeapons = PlayerData.Instance.selectedWeapons;
            var weapons = new (int weaponId, int level)[5];
            for (int i = 0; i < 5; i++)
            {
                if (selectedWeapons[i] != null)
                    weapons[i] = (selectedWeapons[i].weaponId, selectedWeapons[i].level);
            }

            string data = MultiplayerSessionData.SerializeWeaponLoadout(weapons);
            _lobbyManager.SetWeaponLoadout(data);
        }

        private async void LoadAvatarAsync(SteamId steamId, UnityEngine.UI.Image targetImage)
        {
            var avatarImage = await SteamFriends.GetMediumAvatarAsync(steamId);
            if (avatarImage.HasValue && targetImage != null)
            {
                var tex = ImageUtils.Covert(avatarImage.Value);
                targetImage.sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f));
            }
        }

        #region Friend Picker (overlay fallback)

        private void ShowFriendPicker(List<Friend> friends)
        {
            if (friendPickerPanel == null || friendListContent == null || friendEntryPrefab == null)
            {
                statusText.text = "Steam overlay unavailable. Add game to Steam to use invite overlay.";
                return;
            }

            // Clear previous entries
            foreach (Transform child in friendListContent)
                Destroy(child.gameObject);

            if (friends.Count == 0)
            {
                statusText.text = "No online friends found.";
                return;
            }

            friendPickerPanel.SetActive(true);

            foreach (var friend in friends)
            {
                var entry = Instantiate(friendEntryPrefab, friendListContent);
                var text = entry.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    string label = friend.IsPlayingThisGame
                        ? $"{friend.Name} (In Game)"
                        : friend.Name;
                    text.text = label;
                }

                var button = entry.GetComponent<Button>();
                if (button != null)
                {
                    var friendId = friend.Id;
                    button.onClick.AddListener(() =>
                    {
                        bool sent = _lobbyManager.InviteFriendDirect(friendId);
                        statusText.text = sent
                            ? $"Invite sent to {friend.Name}!"
                            : $"Failed to invite {friend.Name}.";
                        friendPickerPanel.SetActive(false);
                    });
                }
            }
        }

        #endregion
    }
}
