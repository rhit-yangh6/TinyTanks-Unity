using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _Scripts.Managers;
using _Scripts.Networking;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.GameEngine
{
    /// <summary>
    /// Multiplayer turn state enum.
    /// </summary>
    public enum MultiplayerTurnState
    {
        WaitingForPlayers,
        ActivePlayerMoving,
        ActivePlayerShooting,
        WaitingForProjectiles,
        InterTurn,
        GameOver
    }

    /// <summary>
    /// Host-authoritative multiplayer game controller.
    /// Manages turn flow for 2-4 networked players.
    /// Does NOT extend AbstractGameController to avoid single-player coupling.
    /// </summary>
    public class MultiplayerGameController : NetworkBehaviour
    {
        [SerializeField] private float turnTime = 45f;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private GameObject playerTankPrefab;

        [Header("Debug")]
        [SerializeField] private float debugBotTurnDuration = 2f;

        /// <summary>Current player index (0-3) whose turn it is.</summary>
        public NetworkVariable<int> CurrentTurnPlayerIndex = new(
            writePerm: NetworkVariableWritePermission.Server);

        /// <summary>Turn countdown timer.</summary>
        public NetworkVariable<float> RemainingTime = new(
            writePerm: NetworkVariableWritePermission.Server);

        /// <summary>Current turn phase.</summary>
        public NetworkVariable<MultiplayerTurnState> Phase = new(
            MultiplayerTurnState.WaitingForPlayers,
            writePerm: NetworkVariableWritePermission.Server);

        /// <summary>Number of players in the match.</summary>
        public NetworkVariable<int> PlayerCount = new(
            writePerm: NetworkVariableWritePermission.Server);

        private readonly List<MultiplayerTankController> _tanks = new();
        private readonly List<int> _alivePlayerIndices = new();
        private int _turnOrderIndex;
        private Coroutine _projectileCheckCoroutine;

        // Events for UI to subscribe to
        public event System.Action<int> OnTurnChanged;     // playerIndex
        public event System.Action<int> OnPlayerEliminated; // playerIndex
        public event System.Action<int> OnGameOver;         // winnerIndex

        public IReadOnlyList<MultiplayerTankController> Tanks => _tanks;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

                if (MultiplayerSessionData.IsBotMode)
                    SpawnBotMode();
                else
                    StartCoroutine(WaitForClientsAndSpawn());
            }

            Phase.OnValueChanged += OnPhaseChanged;
            CurrentTurnPlayerIndex.OnValueChanged += OnCurrentTurnChanged;

            // Play multiplayer music on all clients
            PlayMultiplayerMusic();
        }

        private void PlayMultiplayerMusic()
        {
            var audioClip = Resources.Load<AudioClip>("AudioClips/Music/Multiplayer");
            if (audioClip == null) return;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMusicClip(audioClip);
            }
            else
            {
                // AudioManager not available — play with a local AudioSource
                var source = gameObject.AddComponent<AudioSource>();
                source.clip = audioClip;
                source.loop = true;
                source.volume = PlayerPrefs.GetFloat(Constants.MusicVolumeValue, 5f) / 10f;
                source.Play();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            }

            Phase.OnValueChanged -= OnPhaseChanged;
            CurrentTurnPlayerIndex.OnValueChanged -= OnCurrentTurnChanged;

            base.OnNetworkDespawn();
        }

        #region Player Spawning (Host Only)

        /// <summary>
        /// Spawn tanks in bot mode — all owned by host, bot auto-ends turns.
        /// Session data is already populated by LobbyUI before scene load.
        /// </summary>
        private void SpawnBotMode()
        {
            int count = MultiplayerSessionData.PlayerCount;
            Debug.Log($"[MultiplayerGC] Bot mode: spawning {count} tanks locally.");

            if (count == 0 || playerTankPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("[MultiplayerGC] Bot mode: missing players, prefab, or spawn points.");
                return;
            }

            PlayerCount.Value = count;

            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = spawnPoints[i % spawnPoints.Length].position;
                GameObject tankObj = Instantiate(playerTankPrefab, spawnPos, Quaternion.identity);

                var no = tankObj.GetComponent<NetworkObject>();
                no.SpawnWithOwnership(NetworkManager.ServerClientId);

                var tank = tankObj.GetComponent<MultiplayerTankController>();
                var tankNet = tankObj.GetComponent<MultiplayerTankNetwork>();
                tankNet.PlayerIndex.Value = i;
                tankNet.DisplayName.Value = MultiplayerSessionData.PlayerNames[i] ?? $"Player {i + 1}";

                _tanks.Add(tank);
                _alivePlayerIndices.Add(i);
            }

            // Apply weapon loadouts
            ApplyWeaponLoadouts(count);

            StartCoroutine(StartGameDelayed());
        }

        /// <summary>
        /// Wait until all expected clients have connected before spawning.
        /// </summary>
        private IEnumerator WaitForClientsAndSpawn()
        {
            int expectedClients = MultiplayerSessionData.PlayerCount;
            float timeout = 15f;
            float elapsed = 0f;

            Debug.Log($"[MultiplayerGC] Waiting for {expectedClients} clients (currently {NetworkManager.Singleton.ConnectedClientsIds.Count})...");

            while (NetworkManager.Singleton.ConnectedClientsIds.Count < expectedClients && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            Debug.Log($"[MultiplayerGC] {NetworkManager.Singleton.ConnectedClientsIds.Count}/{expectedClients} clients connected after {elapsed:F1}s. Spawning.");
            SpawnPlayers();
        }

        /// <summary>
        /// Called by host after scene load to spawn all player tanks.
        /// </summary>
        public void SpawnPlayers()
        {
            if (!IsServer) return;

            int count = MultiplayerSessionData.PlayerCount;
            Debug.Log($"[MultiplayerGC] SpawnPlayers: count={count}, spawnPoints={spawnPoints?.Length}, prefab={playerTankPrefab != null}");

            if (count == 0 || playerTankPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("[MultiplayerGC] Cannot spawn: missing players, prefab, or spawn points.");
                return;
            }

            PlayerCount.Value = count;

            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = spawnPoints[i % spawnPoints.Length].position;
                GameObject tankObj = Instantiate(playerTankPrefab, spawnPos, Quaternion.identity);

                var no = tankObj.GetComponent<NetworkObject>();
                // Spawn with ownership assigned to respective client
                no.SpawnWithOwnership(GetClientIdForPlayerIndex(i));

                var tank = tankObj.GetComponent<MultiplayerTankController>();
                var tankNet = tankObj.GetComponent<MultiplayerTankNetwork>();
                tankNet.PlayerIndex.Value = i;
                tankNet.DisplayName.Value = MultiplayerSessionData.PlayerNames[i] ?? $"Player {i + 1}";

                _tanks.Add(tank);
                _alivePlayerIndices.Add(i);
            }

            // Apply weapon loadouts
            ApplyWeaponLoadouts(count);

            // Start the game
            StartCoroutine(StartGameDelayed());
        }

        private void ApplyWeaponLoadouts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var weapons = MultiplayerSessionData.PlayerWeapons[i];
                if (weapons == null || weapons.Length == 0) continue;

                int weaponId = weapons[0].weaponId;
                int level = weapons[0].level;

                if (WeaponManager.Instance == null || WeaponManager.Instance.GetWeaponById(weaponId) == null)
                {
                    Debug.LogWarning($"[MultiplayerGC] Weapon ID {weaponId} not found for player {i}, skipping.");
                    continue;
                }

                var launcher = _tanks[i].GetComponent<MultiplayerLaunchProjectile>();
                launcher.SwitchWeapon(weaponId, level, null);
            }
        }

        private IEnumerator StartGameDelayed()
        {
            yield return new WaitForSeconds(2f);
            BeginTurn(0);
        }

        private ulong GetClientIdForPlayerIndex(int playerIndex)
        {
            // Map player index → Steam ID → NGO client ID
            var steamId = MultiplayerSessionData.PlayerSteamIds[playerIndex];
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport
                as Netcode.Transports.Facepunch.FacepunchTransport;

            if (transport != null)
            {
                ulong clientId = transport.GetClientIdForSteamId(steamId);
                Debug.Log($"[MultiplayerGC] Player {playerIndex} SteamId={steamId} → ClientId={clientId}");
                return clientId;
            }

            // Fallback
            Debug.LogWarning("[MultiplayerGC] FacepunchTransport not found, falling back to list order.");
            var clients = NetworkManager.Singleton.ConnectedClientsList;
            if (playerIndex < clients.Count)
                return clients[playerIndex].ClientId;
            return NetworkManager.ServerClientId;
        }

        #endregion

        #region Turn Management (Host Only)

        private void BeginTurn(int turnOrderIdx)
        {
            if (!IsServer) return;
            if (Phase.Value == MultiplayerTurnState.GameOver) return;

            _turnOrderIndex = turnOrderIdx;
            int playerIdx = _alivePlayerIndices[_turnOrderIndex % _alivePlayerIndices.Count];

            // Deactivate all players' turns
            foreach (var tank in _tanks)
            {
                if (tank != null)
                    tank.GetComponent<MultiplayerTankNetwork>().IsMyTurn.Value = false;
            }

            // Activate current player
            var currentTank = _tanks[playerIdx];
            if (currentTank == null || currentTank.IsDead)
            {
                // Skip dead player
                AdvanceToNextTurn();
                return;
            }

            currentTank.GetComponent<MultiplayerTankNetwork>().IsMyTurn.Value = true;
            currentTank.TickBuffs();

            CurrentTurnPlayerIndex.Value = playerIdx;
            RemainingTime.Value = turnTime;
            Phase.Value = MultiplayerTurnState.ActivePlayerMoving;

            OnTurnChanged?.Invoke(playerIdx);

            // In bot mode, auto-end the bot's turn
            if (MultiplayerSessionData.IsBotMode && playerIdx != MultiplayerSessionData.LocalPlayerIndex)
            {
                StartCoroutine(BotAutoEndTurn(playerIdx));
            }
        }

        private IEnumerator BotAutoEndTurn(int botPlayerIdx)
        {
            yield return new WaitForSeconds(debugBotTurnDuration);

            // Make sure it's still the bot's turn
            if (CurrentTurnPlayerIndex.Value != botPlayerIdx) yield break;
            if (Phase.Value != MultiplayerTurnState.ActivePlayerMoving) yield break;

            Debug.Log($"[MultiplayerGC] Bot player {botPlayerIdx} auto-ending turn.");
            Phase.Value = MultiplayerTurnState.InterTurn;
            StartCoroutine(InterTurnDelay());
        }

        private void AdvanceToNextTurn()
        {
            if (!IsServer) return;

            // Remove dead players
            _alivePlayerIndices.RemoveAll(idx =>
                idx >= _tanks.Count || _tanks[idx] == null || _tanks[idx].IsDead);

            // Check win condition (need at least 2 players for game over to matter)
            if (_alivePlayerIndices.Count == 0)
            {
                HandleGameOver();
                return;
            }

            if (_alivePlayerIndices.Count == 1 && PlayerCount.Value > 1)
            {
                HandleGameOver();
                return;
            }

            _turnOrderIndex = (_turnOrderIndex + 1) % _alivePlayerIndices.Count;
            BeginTurn(_turnOrderIndex);
        }

        private void HandleGameOver()
        {
            Phase.Value = MultiplayerTurnState.GameOver;

            int winnerIndex = _alivePlayerIndices.Count > 0 ? _alivePlayerIndices[0] : -1;
            OnGameOver?.Invoke(winnerIndex);
            HandleGameOverClientRpc(winnerIndex);

            // Trigger achievements on winner
            if (winnerIndex >= 0)
            {
                var winnerId = MultiplayerSessionData.PlayerSteamIds[winnerIndex];
                if (winnerId == Steamworks.SteamClient.SteamId)
                {
                    SteamManager.UnlockAchievement(Constants.AchievementMultiplayerWin);
                    SteamManager.IncrementStat(Constants.StatMultiplayerWins);
                }
            }
        }

        [ClientRpc]
        private void HandleGameOverClientRpc(int winnerIndex)
        {
            OnGameOver?.Invoke(winnerIndex);
        }

        #endregion

        #region Update (Host)

        private void Update()
        {
            if (!IsServer) return;

            switch (Phase.Value)
            {
                case MultiplayerTurnState.ActivePlayerMoving:
                    RemainingTime.Value -= Time.deltaTime;
                    if (RemainingTime.Value <= 0)
                    {
                        Phase.Value = MultiplayerTurnState.InterTurn;
                        StartCoroutine(InterTurnDelay());
                    }
                    break;

                case MultiplayerTurnState.ActivePlayerShooting:
                case MultiplayerTurnState.WaitingForProjectiles:
                    BeginProjectileCheck();
                    break;
            }
        }

        #endregion

        #region Projectile Resolution

        private void OnProjectileShot()
        {
            if (!IsServer) return;

            if (Phase.Value == MultiplayerTurnState.ActivePlayerMoving)
            {
                Phase.Value = MultiplayerTurnState.ActivePlayerShooting;
            }
        }

        private void OnEnable()
        {
            EventBus.AddListener(EventTypes.ProjectileShot, OnProjectileShot);
        }

        private void OnDisable()
        {
            EventBus.RemoveListener(EventTypes.ProjectileShot, OnProjectileShot);
        }

        private void BeginProjectileCheck()
        {
            if (_projectileCheckCoroutine == null)
            {
                _projectileCheckCoroutine = StartCoroutine(WaitForProjectilesAndAdvance());
            }
        }

        private IEnumerator WaitForProjectilesAndAdvance()
        {
            // Wait for all aggressive projectiles to be destroyed
            while (GameObject.FindGameObjectsWithTag("AggressiveProjectile").Length > 0)
            {
                yield return new WaitForSeconds(0.25f);
            }

            _projectileCheckCoroutine = null;

            // Check for eliminations
            for (int i = _alivePlayerIndices.Count - 1; i >= 0; i--)
            {
                int idx = _alivePlayerIndices[i];
                if (idx < _tanks.Count && (_tanks[idx] == null || _tanks[idx].IsDead))
                {
                    _alivePlayerIndices.RemoveAt(i);
                    OnPlayerEliminated?.Invoke(idx);
                    PlayerEliminatedClientRpc(idx);
                }
            }

            Phase.Value = MultiplayerTurnState.InterTurn;
            yield return new WaitForSeconds(1f);
            AdvanceToNextTurn();
        }

        [ClientRpc]
        private void PlayerEliminatedClientRpc(int playerIndex)
        {
            OnPlayerEliminated?.Invoke(playerIndex);
        }

        #endregion

        #region InterTurn

        private IEnumerator InterTurnDelay()
        {
            // Deactivate current player's turn
            int currentIdx = CurrentTurnPlayerIndex.Value;
            if (currentIdx < _tanks.Count && _tanks[currentIdx] != null)
            {
                _tanks[currentIdx].GetComponent<MultiplayerTankNetwork>().IsMyTurn.Value = false;
            }

            yield return new WaitForSeconds(1f);
            AdvanceToNextTurn();
        }

        #endregion

        #region Disconnection

        private void HandleClientDisconnect(ulong clientId)
        {
            if (!IsServer) return;

            // Find which player disconnected
            for (int i = 0; i < _tanks.Count; i++)
            {
                var tank = _tanks[i];
                var tankNet = tank != null ? tank.GetComponent<MultiplayerTankNetwork>() : null;
                if (tankNet != null && tankNet.OwnerClientId == clientId)
                {
                    Debug.Log($"[MultiplayerGC] Player {i} disconnected.");
                    tank.IsDead = true;
                    _alivePlayerIndices.Remove(i);
                    OnPlayerEliminated?.Invoke(i);
                    PlayerEliminatedClientRpc(i);

                    // If it was their turn, advance
                    if (CurrentTurnPlayerIndex.Value == i)
                    {
                        CancelProjectileCheck();
                        AdvanceToNextTurn();
                    }

                    // Check if game should end
                    if (_alivePlayerIndices.Count <= 1)
                    {
                        HandleGameOver();
                    }

                    if (tank.gameObject != null)
                        Destroy(tank.gameObject);
                    break;
                }
            }
        }

        private void CancelProjectileCheck()
        {
            if (_projectileCheckCoroutine != null)
            {
                StopCoroutine(_projectileCheckCoroutine);
                _projectileCheckCoroutine = null;
            }
        }

        #endregion

        #region Phase Change Callbacks

        private void OnPhaseChanged(MultiplayerTurnState oldValue, MultiplayerTurnState newValue)
        {
            // Clients can react to phase changes here for UI updates
        }

        private void OnCurrentTurnChanged(int oldValue, int newValue)
        {
            OnTurnChanged?.Invoke(newValue);
        }

        #endregion
    }
}
