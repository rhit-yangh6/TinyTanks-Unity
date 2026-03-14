using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Scripts.GameEngine;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace _Scripts.Networking
{
    /// <summary>
    /// Manages Steam lobby lifecycle: create, join, invite, leave.
    /// DontDestroyOnLoad singleton.
    /// </summary>
    public class SteamLobbyManager : MonoBehaviour
    {
        private static SteamLobbyManager _instance;
        public static SteamLobbyManager Instance => _instance;

        public Lobby? CurrentLobby { get; private set; }
        public bool IsHost { get; private set; }

        public event Action OnLobbyCreated;
        public event Action OnLobbyJoined;
        public event Action OnLobbyLeft;
        public event Action OnLobbyMemberJoined;
        public event Action OnLobbyMemberLeft;
        public event Action<Lobby> OnGameLobbyJoinRequested;

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

        private void OnEnable()
        {
            SteamMatchmaking.OnLobbyMemberJoined += HandleLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += HandleLobbyMemberLeft;
            SteamMatchmaking.OnLobbyEntered += HandleLobbyEntered;
            SteamMatchmaking.OnLobbyDataChanged += HandleLobbyDataChanged;
            SteamFriends.OnGameLobbyJoinRequested += HandleGameLobbyJoinRequested;
        }

        private void OnDisable()
        {
            SteamMatchmaking.OnLobbyMemberJoined -= HandleLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= HandleLobbyMemberLeft;
            SteamMatchmaking.OnLobbyEntered -= HandleLobbyEntered;
            SteamMatchmaking.OnLobbyDataChanged -= HandleLobbyDataChanged;
            SteamFriends.OnGameLobbyJoinRequested -= HandleGameLobbyJoinRequested;
        }

        private void Start()
        {
            // Check if game was launched from a Steam invite (command line: +connect_lobby <id>)
            CheckCommandLineJoin();
        }

        /// <summary>
        /// Create a friends-only lobby with the specified max players.
        /// </summary>
        public async Task<bool> CreateLobby(int maxPlayers = 4)
        {
            try
            {
                var result = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
                if (!result.HasValue)
                {
                    Debug.LogError("[SteamLobby] Failed to create lobby.");
                    return false;
                }

                CurrentLobby = result.Value;
                CurrentLobby.Value.SetFriendsOnly();
                IsHost = true;

                Debug.Log($"[SteamLobby] Created lobby {CurrentLobby.Value.Id}");
                OnLobbyCreated?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SteamLobby] CreateLobby exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// Join an existing lobby by its ID.
        /// </summary>
        public async Task<bool> JoinLobby(SteamId lobbyId)
        {
            try
            {
                var lobby = new Lobby(lobbyId);
                var result = await lobby.Join();
                if (result != RoomEnter.Success)
                {
                    Debug.LogError($"[SteamLobby] Failed to join lobby: {result}");
                    return false;
                }

                CurrentLobby = lobby;
                IsHost = false;
                Debug.Log($"[SteamLobby] Joined lobby {lobbyId}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SteamLobby] JoinLobby exception: {e}");
                return false;
            }
        }

        /// <summary>
        /// Open Steam overlay to invite friends.
        /// </summary>
        /// <summary>
        /// Try the Steam overlay first; if unavailable, fire OnFriendListRequested
        /// so the UI can show a friend picker.
        /// </summary>
        public void InviteFriend()
        {
            if (!CurrentLobby.HasValue)
            {
                Debug.LogWarning("[SteamLobby] No lobby to invite to.");
                return;
            }

            if (SteamUtils.IsOverlayEnabled)
            {
                SteamFriends.OpenGameInviteOverlay(CurrentLobby.Value.Id);
            }
            else
            {
                Debug.Log("[SteamLobby] Overlay not available, requesting friend list UI.");
                OnFriendListRequested?.Invoke(GetOnlineFriends());
            }
        }

        /// <summary>
        /// Directly invite a friend by SteamId (no overlay needed).
        /// </summary>
        public bool InviteFriendDirect(SteamId friendId)
        {
            if (!CurrentLobby.HasValue) return false;
            bool sent = CurrentLobby.Value.InviteFriend(friendId);
            Debug.Log($"[SteamLobby] Direct invite to {friendId}: {(sent ? "sent" : "failed")}");
            return sent;
        }

        /// <summary>
        /// Get online friends list.
        /// </summary>
        public List<Friend> GetOnlineFriends()
        {
            var friends = new List<Friend>();
            foreach (var friend in SteamFriends.GetFriends())
            {
                if (friend.IsOnline || friend.IsPlayingThisGame)
                    friends.Add(friend);
            }
            return friends;
        }

        /// <summary>Fired when overlay is unavailable and UI should show a friend picker.</summary>
        public event Action<List<Friend>> OnFriendListRequested;

        /// <summary>
        /// Leave the current lobby and clean up.
        /// </summary>
        public void LeaveLobby()
        {
            if (!CurrentLobby.HasValue) return;

            CurrentLobby.Value.Leave();
            CurrentLobby = null;
            IsHost = false;

            Debug.Log("[SteamLobby] Left lobby.");
            OnLobbyLeft?.Invoke();
        }

        /// <summary>
        /// Store weapon loadout in lobby member metadata.
        /// Format: "weaponId:level,weaponId:level,..."
        /// </summary>
        public void SetWeaponLoadout(string loadoutData)
        {
            CurrentLobby?.SetMemberData("weapons", loadoutData);
        }

        /// <summary>
        /// Get a member's weapon loadout from lobby metadata.
        /// </summary>
        public string GetMemberWeaponLoadout(Friend member)
        {
            return CurrentLobby?.GetMemberData(member, "weapons") ?? "";
        }

        /// <summary>
        /// Lock the lobby and signal all clients that the game is starting.
        /// The host sets lobby data "gameStarting" = "1" and "hostSteamId".
        /// Clients detect this via HandleLobbyDataChanged and connect as Netcode clients.
        /// </summary>
        public void LockLobby()
        {
            if (!CurrentLobby.HasValue) return;
            CurrentLobby.Value.SetJoinable(false);
            CurrentLobby.Value.SetData("hostSteamId", SteamClient.SteamId.ToString());
            CurrentLobby.Value.SetData("gameStarting", "1");
            Debug.Log("[SteamLobby] Lobby locked, game start signaled.");
        }

        /// <summary>
        /// Called when lobby metadata changes. Non-host clients detect "gameStarting"
        /// and connect as Netcode clients to the host.
        /// </summary>
        private void HandleLobbyDataChanged(Lobby lobby)
        {
            if (IsHost) return;
            if (!CurrentLobby.HasValue) return;
            if (CurrentLobby.Value.Id != lobby.Id) return;

            string starting = lobby.GetData("gameStarting");
            if (starting != "1") return;

            string hostIdStr = lobby.GetData("hostSteamId");
            if (!ulong.TryParse(hostIdStr, out ulong hostId))
            {
                Debug.LogError("[SteamLobby] Could not parse host SteamId from lobby data.");
                return;
            }

            Debug.Log($"[SteamLobby] Game starting! Connecting to host {hostId}.");

            // Populate session data on client side
            var members = lobby.Members.ToArray();
            MultiplayerSessionData.Clear();
            MultiplayerSessionData.IsMultiplayer = true;
            MultiplayerSessionData.IsHost = false;
            MultiplayerSessionData.PlayerCount = members.Length;

            for (int i = 0; i < members.Length; i++)
            {
                MultiplayerSessionData.PlayerSteamIds[i] = members[i].Id;
                MultiplayerSessionData.PlayerNames[i] = members[i].Name;

                string weaponData = GetMemberWeaponLoadout(members[i]);
                MultiplayerSessionData.PlayerWeapons[i] =
                    MultiplayerSessionData.ParseWeaponLoadout(weaponData);

                if (members[i].Id == SteamClient.SteamId)
                    MultiplayerSessionData.LocalPlayerIndex = i;
            }

            // Connect as Netcode client — scene load happens automatically via NGO
            NetworkSetup.Instance.StartClient(new SteamId { Value = hostId });
        }

        /// <summary>
        /// Check if the game was launched via Steam invite (+connect_lobby arg).
        /// </summary>
        private async void CheckCommandLineJoin()
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "+connect_lobby" && ulong.TryParse(args[i + 1], out ulong lobbyId))
                {
                    Debug.Log($"[SteamLobby] Launched with +connect_lobby {lobbyId}");
                    // Wait a moment for Steam to be ready
                    await Task.Delay(1000);
                    await JoinLobby(lobbyId);
                    return;
                }
            }
        }

        private void HandleLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobby] {friend.Name} joined the lobby.");
            OnLobbyMemberJoined?.Invoke();
        }

        private void HandleLobbyMemberLeft(Lobby lobby, Friend friend)
        {
            Debug.Log($"[SteamLobby] {friend.Name} left the lobby.");
            OnLobbyMemberLeft?.Invoke();
        }

        private void HandleLobbyEntered(Lobby lobby)
        {
            CurrentLobby = lobby;
            Debug.Log($"[SteamLobby] Entered lobby {lobby.Id}");
            OnLobbyJoined?.Invoke();
        }

        private async void HandleGameLobbyJoinRequested(Lobby lobby, SteamId friendId)
        {
            Debug.Log($"[SteamLobby] Join requested for lobby {lobby.Id} from friend {friendId}");
            OnGameLobbyJoinRequested?.Invoke(lobby);
            await JoinLobby(lobby.Id);
        }
    }
}
