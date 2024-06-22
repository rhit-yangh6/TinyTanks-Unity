using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Managers;
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
            
            // optionsController.LoadSettings();
        }

        private void Start()
        {
            
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
            // var currentIsDay = CheckIsDay();
            // // Switch background
            // if (currentIsDay == isDay) return;
            //
            // SteamManager.UnlockAchievement(Constants.AchievementYinYang);
            // WeaponManager.UnlockWeapon(24); // YinYang 24
            // isDay = currentIsDay;
            // StartCoroutine(SwitchBackground());
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

        private static bool CheckIsDay()
        {
            var now = DateTime.Now;
            return now.Hour is >= 6 and < 18;
        }

        public void SwitchLanguage(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }
    }
}
