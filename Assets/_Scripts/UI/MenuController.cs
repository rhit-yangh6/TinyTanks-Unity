using System;
using _Scripts.GameEngine;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private AsyncLoader asyncLoader;
        [SerializeField] private OptionsController optionsController;
        
        public void Awake()
        {
            // TODO: Probably move this to somewhere else, facade?
            // Create a DiscordManager Singleton
            var unusedDiscordManager = DiscordManager.Instance;
            
            // Create a SteamManager Singleton
            var unusedSteamManager = SteamManager.Instance;
            
            optionsController.LoadSettings();
        }

        private void Start()
        {
            if (!PlayerData.Instance.isTutorialCompleted)
            {
                // Didn't pass tutorial, change to tutorial scene
                SceneManager.LoadScene("Tutorial");
            }
        }

        public void OnEnable()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange, "Browsing Menu", "");
        }

        public void ExitButtonYes()
        {
            AudioManager.Instance.FadeOutMusic();
            SaveSystem.SavePlayer();
            Application.Quit();
        }

        public void EnterSurvivalMode() { asyncLoader.LoadSurvivalMode(); }
        
        public void EnterShootingRange() { asyncLoader.LoadShootingRange(); }
        
        public void EnterTutorial() { asyncLoader.LoadTutorial(); }
    }
}
