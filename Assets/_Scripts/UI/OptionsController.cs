using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
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
                WeaponManager.Instance.UnlockWeapon(22); // Boombox 22
            }
            
            LoadSettings();
        }

        public void LoadSettings()
        {
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
        }
    }
}