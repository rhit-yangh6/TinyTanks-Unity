using System;
using System.Globalization;
using _Scripts.GameEngine;
using _Scripts.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI.Multiplayer
{
    /// <summary>
    /// In-game multiplayer HUD.
    /// Shows health bars for all players, turn indicator, timer, and weapon panel.
    /// </summary>
    public class MultiplayerHUD : MonoBehaviour
    {
        [Header("Turn Info")]
        [SerializeField] private TextMeshProUGUI turnIndicatorText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Player Health Panels")]
        [SerializeField] private GameObject[] playerHealthPanels;
        [SerializeField] private TextMeshProUGUI[] playerNameTexts;
        [SerializeField] private Slider[] playerHealthBars;

        [Header("Local Player UI")]
        [SerializeField] private GameObject fuelBarObject;
        [SerializeField] private Slider fuelBar;
        [SerializeField] private MultiplayerWeaponControl weaponControl;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private Button returnToMenuButton;

        [Header("Quick Chat")]
        [SerializeField] private GameObject quickChatPanel;
        [SerializeField] private Button[] quickChatButtons;

        private MultiplayerGameController _gameController;
        private int _localPlayerIndex;
        private bool _weaponControlInitialized;

        private void Start()
        {
            _gameController = FindAnyObjectByType<MultiplayerGameController>();
            _localPlayerIndex = MultiplayerSessionData.LocalPlayerIndex;

            if (_gameController != null)
            {
                _gameController.OnTurnChanged += OnTurnChanged;
                _gameController.OnPlayerEliminated += OnPlayerEliminated;
                _gameController.OnGameOver += OnGameOver;
            }

            if (returnToMenuButton != null)
                returnToMenuButton.onClick.AddListener(ReturnToMenu);

            // Setup quick chat buttons
            SetupQuickChat();

            // Hide game over panel
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            // Initialize player panels
            InitializePlayerPanels();
        }

        private void OnDestroy()
        {
            if (_gameController != null)
            {
                _gameController.OnTurnChanged -= OnTurnChanged;
                _gameController.OnPlayerEliminated -= OnPlayerEliminated;
                _gameController.OnGameOver -= OnGameOver;
            }
        }

        private void Update()
        {
            if (_gameController == null) return;

            TryInitializeWeaponControl();

            // Update timer
            if (timerText != null)
            {
                var phase = _gameController.Phase.Value;
                switch (phase)
                {
                    case MultiplayerTurnState.ActivePlayerMoving:
                        timerText.text = Math.Round(_gameController.RemainingTime.Value)
                            .ToString(CultureInfo.InvariantCulture);
                        break;
                    case MultiplayerTurnState.ActivePlayerShooting:
                    case MultiplayerTurnState.WaitingForProjectiles:
                        timerText.text = "Please Wait...";
                        break;
                    default:
                        timerText.text = "";
                        break;
                }
            }

            // Update health bars
            UpdateHealthBars();

            // Update fuel bar for local player
            UpdateFuelBar();

            // Show/hide weapon panel and fuel bar based on whose turn
            bool isMyTurn = _gameController.CurrentTurnPlayerIndex.Value == _localPlayerIndex
                            && _gameController.Phase.Value == MultiplayerTurnState.ActivePlayerMoving;

            if (weaponControl != null)
                weaponControl.SetVisible(isMyTurn);

            if (fuelBarObject != null)
                fuelBarObject.SetActive(isMyTurn);
        }

        private void TryInitializeWeaponControl()
        {
            if (_weaponControlInitialized || weaponControl == null) return;

            var tanks = _gameController.Tanks;
            if (_localPlayerIndex >= tanks.Count || tanks[_localPlayerIndex] == null) return;

            var launcher = tanks[_localPlayerIndex].GetComponent<MultiplayerLaunchProjectile>();
            if (launcher == null) return;

            var weapons = MultiplayerSessionData.PlayerWeapons[_localPlayerIndex];
            if (weapons == null || weapons.Length == 0) return;

            weaponControl.Initialize(launcher, weapons);
            _weaponControlInitialized = true;
        }

        private void InitializePlayerPanels()
        {
            for (int i = 0; i < playerHealthPanels.Length; i++)
            {
                if (i < MultiplayerSessionData.PlayerCount)
                {
                    playerHealthPanels[i].SetActive(true);
                    if (playerNameTexts[i] != null)
                        playerNameTexts[i].text = MultiplayerSessionData.PlayerNames[i];
                }
                else
                {
                    playerHealthPanels[i].SetActive(false);
                }
            }
        }

        private void UpdateHealthBars()
        {
            if (_gameController == null) return;

            var tanks = _gameController.Tanks;
            for (int i = 0; i < playerHealthBars.Length && i < tanks.Count; i++)
            {
                if (playerHealthBars[i] == null) continue;

                if (tanks[i] != null && !tanks[i].IsDead)
                {
                    playerHealthBars[i].value = tanks[i].GetHealthPercentage();
                }
                else
                {
                    playerHealthBars[i].value = 0;
                }
            }
        }

        private void UpdateFuelBar()
        {
            if (fuelBar == null || _gameController == null) return;

            var tanks = _gameController.Tanks;
            if (_localPlayerIndex < tanks.Count && tanks[_localPlayerIndex] != null)
            {
                var localTank = tanks[_localPlayerIndex];
                fuelBar.value = localTank.Fuel / localTank.MaxFuel;
            }
        }

        private void OnTurnChanged(int playerIndex)
        {
            if (turnIndicatorText == null) return;

            if (playerIndex == _localPlayerIndex)
            {
                turnIndicatorText.text = "Your Turn!";
            }
            else
            {
                string name = MultiplayerSessionData.PlayerNames[playerIndex] ?? $"Player {playerIndex + 1}";
                turnIndicatorText.text = $"{name}'s Turn";
            }
        }

        private void OnPlayerEliminated(int playerIndex)
        {
            if (playerIndex < playerHealthPanels.Length)
            {
                // Grey out the eliminated player's panel
                var canvasGroup = playerHealthPanels[playerIndex].GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                    canvasGroup.alpha = 0.4f;
            }
        }

        private void OnGameOver(int winnerIndex)
        {
            if (gameOverPanel == null) return;

            gameOverPanel.SetActive(true);

            if (winnerIndex == _localPlayerIndex)
            {
                gameOverText.text = "You Win!";
            }
            else if (winnerIndex >= 0)
            {
                string name = MultiplayerSessionData.PlayerNames[winnerIndex] ?? $"Player {winnerIndex + 1}";
                gameOverText.text = $"{name} Wins!";
            }
            else
            {
                gameOverText.text = "Draw!";
            }
        }

        private void SetupQuickChat()
        {
            var quickChat = FindAnyObjectByType<QuickChat>();
            if (quickChat == null || quickChatButtons == null) return;

            for (int i = 0; i < quickChatButtons.Length && i < QuickChat.Messages.Length; i++)
            {
                int idx = i;
                quickChatButtons[i].onClick.AddListener(() => quickChat.SendMessage(idx));

                var text = quickChatButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = QuickChat.Messages[i];
            }
        }

        private void ReturnToMenu()
        {
            MultiplayerSessionData.Clear();
            if (NetworkSetup.Instance != null)
                NetworkSetup.Instance.Shutdown();
            if (SteamLobbyManager.Instance != null)
                SteamLobbyManager.Instance.LeaveLobby();

            Time.timeScale = 1f;
            SceneManager.LoadScene("Main Menu");
        }
    }
}
