using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Networking;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using EventBus = _Scripts.Managers.EventBus;

namespace _Scripts.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private AsyncLoader asyncLoader;
        [SerializeField] private OptionsController optionsController;
        [SerializeField] private CanvasGroup dayCanvasGroup, nightCanvasGroup;
        [SerializeField] private AnimationCurve fadeOutCurve;
        [SerializeField] private AnimationCurve fadeInCurve;
        [SerializeField] private float waitTime = 2f;

        private bool isDay;
        
        public void Awake()
        {
            // TODO: Probably move this to somewhere else, facade?
            // Create a DiscordManager Singleton
            var unusedDiscordManager = DiscordManager.Instance;
            
            // Create a SteamManager Singleton
            var unusedSteamManager = SteamManager.Instance;

            // Create networking singletons for multiplayer
            if (SteamLobbyManager.Instance == null)
            {
                var lobbyMgr = new GameObject("SteamLobbyManager");
                lobbyMgr.AddComponent<SteamLobbyManager>();
            }

            // optionsController.LoadSettings();
        }

        private void Start()
        {
            if (SteamLobbyManager.Instance != null)
                SteamLobbyManager.Instance.OnLobbyJoined += OnInviteJoined;

            if (debugSimulateInviteJoin)
                _pendingNavigateToLobby = true;
        }

        private void OnInviteJoined()
        {
            // Only auto-navigate for non-host (i.e. accepted an invite)
            if (SteamLobbyManager.Instance != null && !SteamLobbyManager.Instance.IsHost)
                _pendingNavigateToLobby = true;
        }

        private void OnDestroy()
        {
            if (SteamLobbyManager.Instance != null)
                SteamLobbyManager.Instance.OnLobbyJoined -= OnInviteJoined;
        }

        public void OnEnable()
        {

            // isDay = CheckIsDay();
            // if (isDay)
            // {
            //     nightCanvasGroup.alpha = 0;
            //     dayCanvasGroup.alpha = 1;
            // }
            // else
            // {
            //     dayCanvasGroup.alpha = 0;
            //     nightCanvasGroup.alpha = 1;
            // }
        }

        private void Update()
        {
            if (!_pendingNavigateToLobby) return;

            // Wait until splash screen is done — mainPanelsAnimator plays "Invisible"
            // during splash, then "Start" when finished.
            if (mainPanelsAnimator != null
                && mainPanelsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Invisible"))
                return;

            _pendingNavigateToLobby = false;

            // Layer 1: top-level → Play
            if (topPanelManager != null)
                topPanelManager.OpenPanel("Play");
            // Layer 2: inner → Multiplayer
            if (playPanelManager != null)
                playPanelManager.OpenPanel("Multiplayer");
        }

        private IEnumerator SwitchBackground()
        {
            float e = 0;
            var display = isDay ? dayCanvasGroup : nightCanvasGroup;
            var nonDisplay = isDay ? nightCanvasGroup : dayCanvasGroup;
            while (e < waitTime)
            {
                var displayVal = fadeInCurve.Evaluate(Mathf.Clamp01(e / waitTime));
                var nonDisplayVal = fadeOutCurve.Evaluate(Mathf.Clamp01(e / waitTime));
                display.alpha = displayVal;
                nonDisplay.alpha = nonDisplayVal;
                e += Time.deltaTime;
                yield return null;
            }
        }

        public void ExitButtonYes()
        {
            // AudioManager.Instance.FadeOutMusic();
            SaveSystem.SavePlayer();
            Application.Quit();
        }

        public void EnterSurvivalMode() { asyncLoader.LoadSurvivalMode(); }

        public void EnterShootingRange() { asyncLoader.LoadShootingRange(); }

        public void EnterTutorial() { asyncLoader.LoadTutorial(); }

        [SerializeField] private GameObject multiplayerLobbyPanel;

        [Header("Multiplayer Panel Navigation")]
        [SerializeField] private MainPanelManager topPanelManager;
        [SerializeField] private MainPanelManager playPanelManager;
        [SerializeField] private Animator mainPanelsAnimator;
        [SerializeField] private bool debugSimulateInviteJoin;

        private bool _pendingNavigateToLobby;

        public void EnterMultiplayer()
        {
            if (multiplayerLobbyPanel != null)
                multiplayerLobbyPanel.SetActive(true);
        }

        public void ExitMultiplayerLobby()
        {
            if (multiplayerLobbyPanel != null)
                multiplayerLobbyPanel.SetActive(false);
        }

        public void NavigateToMultiplayerLobby()
        {
            _pendingNavigateToLobby = true;
        }

        private static bool CheckIsDay()
        {
            var now = DateTime.Now;
            return now.Hour is >= 6 and < 18;
        }

        public void SwitchLanguage(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }

        public void OpenSteamPage()
        {
            SteamManager.Instance.OpenWebOverlay("https://store.steampowered.com/news/app/2190060?updates=true");
        }
    }
}
