using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class OptionsController : MonoBehaviour
    {
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI masterVolumeTextUI;
        [SerializeField] private TextMeshProUGUI musicVolumeTextUI;
        [SerializeField] private TextMeshProUGUI sfxVolumeTextUI;
        [SerializeField] private Sprite[] languageIcons;
        [SerializeField] private Image languageIcon;
        [SerializeField] private float defaultMasterVolume = 5f;
        [SerializeField] private float defaultMusicVolume = 5f;
        [SerializeField] private float defaultSfxVolume = 5f;

        private int languageSelection;
        
        private void Start()
        {
            LoadSettings();
        }
        
        public void MasterVolumeSlider(float volume)
        {
            masterVolumeTextUI.text = volume.ToString("0");
            AudioListener.volume = volume / 10f;
        }
        
        public void MusicVolumeSlider(float volume)
        {
            musicVolumeTextUI.text = volume.ToString("0");
            AudioManager.Instance.SetMusicVolumeValue(volume / 10f);
        }
        
        public void SfxVolumeSlider(float volume)
        {
            sfxVolumeTextUI.text = volume.ToString("0");
            AudioManager.Instance.SetSfxVolumeValue(volume / 10f);
        }

        public void SwitchLanguage()
        {
            languageSelection = (languageSelection + 1) % languageIcons.Length;
            languageIcon.sprite = languageIcons[languageSelection];
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageSelection];
        }

        public void SaveSettings()
        {
            Debug.Log("Saving Player Settings...");
            var masterVolumeValue = masterVolumeSlider.value;
            PlayerPrefs.SetFloat(Constants.MasterVolumeValue, masterVolumeValue);

            var musicVolumeValue = musicVolumeSlider.value;
            PlayerPrefs.SetFloat(Constants.MusicVolumeValue, musicVolumeValue);
            
            var sfxVolumeValue = sfxVolumeSlider.value;
            PlayerPrefs.SetFloat(Constants.SfxVolumeValue, sfxVolumeValue);
            
            /* Award Boombox when hitting the right values */
            if (masterVolumeValue == 5.0f && musicVolumeValue == 1.0f && sfxVolumeValue == 9.0f)
            {
                SteamManager.UnlockAchievement(Constants.AchievementBoombox);
                WeaponManager.UnlockWeapon(22); // Boombox 22
            }
            
            PlayerPrefs.SetInt(Constants.LanguageSelectionValue, languageSelection);

            PlayerPrefs.Save();
            
            LoadSettings();
        }

        public void LoadSettings()
        {
            if (CheckFirstLaunch())
            {
                Debug.Log("First launch, populating default settings...");

                masterVolumeSlider.value = defaultMasterVolume;
                AudioListener.volume = defaultMasterVolume / 10f;
                
                musicVolumeSlider.value = defaultMusicVolume;
                AudioManager.Instance.SetMusicVolumeValue(defaultMusicVolume / 10f);
                
                sfxVolumeSlider.value = defaultSfxVolume;
                AudioManager.Instance.SetSfxVolumeValue(defaultSfxVolume / 10f);

                languageSelection = 1; // EN
                languageIcon.sprite = languageIcons[languageSelection];
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageSelection];

                PlayerPrefs.Save();
                return;
            }
            
            Debug.Log("Loading Player Settings...");
            var masterVolumeValue = PlayerPrefs.GetFloat(Constants.MasterVolumeValue);
            masterVolumeSlider.value = masterVolumeValue;
            AudioListener.volume = masterVolumeValue / 10f;
            
            var musicVolumeValue = PlayerPrefs.GetFloat(Constants.MusicVolumeValue);
            musicVolumeSlider.value = musicVolumeValue;
            AudioManager.Instance.SetMusicVolumeValue(musicVolumeValue / 10f);
            
            var sfxVolumeValue = PlayerPrefs.GetFloat(Constants.SfxVolumeValue);
            sfxVolumeSlider.value = sfxVolumeValue;
            AudioManager.Instance.SetSfxVolumeValue(sfxVolumeValue / 10f);
            
            languageSelection = PlayerPrefs.GetInt(Constants.LanguageSelectionValue);
            languageIcon.sprite = languageIcons[languageSelection];
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageSelection];
        }

        private static bool CheckFirstLaunch()
        {
            // Reference: https://stackoverflow.com/questions/59037348/how-can-i-check-if-the-user-is-playing-the-game-for-the-first-time
            var isFirstLaunch = false;
            // Game hasn't launched before. 0 is the default value if the player pref doesn't exist yet.
            if(PlayerPrefs.GetInt(Constants.HasLaunchedValue, 0) == 0)
            {
                //Code to display your first time text
                isFirstLaunch = true;
            }
            else
            {
                //Code to show the returning user's text.
            }
            PlayerPrefs.SetInt(Constants.HasLaunchedValue, 1); // Set to 1, so we know the user has been here before
            return isFirstLaunch;
        }
    }
}