using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public float waitTime;
        
        public Sound[] musicSounds, sfxSounds;
        public AudioSource musicSource, sfxSource;
        
        [SerializeField] private AnimationCurve fadeOutCurve;
        [SerializeField] private AnimationCurve fadeInCurve;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.sceneLoaded += OnSceneLoaded;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(HandleChangeMusic(scene.name));
        }

        private IEnumerator HandleChangeMusic(string sceneName)
        {
            switch (sceneName)
            {
                case "Story":
                    yield return StartCoroutine(MusicFadeOut());
                    PlayStoryModeMusic();
                    break;
                case "MenuScene":
                    yield return StartCoroutine(MusicFadeOut());
                    PlayMusic("Menu");
                    break; 
            }
            StartCoroutine(MusicFadeIn());
        }

        private IEnumerator MusicFadeOut()
        {
            float e = 0;
            var musicVolumeValue = PlayerPrefs.GetFloat(Constants.MusicVolumeValue);
            while (e < waitTime)
            {
                var val = fadeOutCurve.Evaluate(Mathf.Clamp01(e / waitTime)) * musicVolumeValue;
                SetMusicVolumeValue(val);
                e += Time.deltaTime;
                yield return null;
            }
        }
        
        private IEnumerator MusicFadeIn()
        {
            float e = 0;
            var musicVolumeValue = PlayerPrefs.GetFloat(Constants.MusicVolumeValue);
            while (e < waitTime)
            {
                var val = fadeInCurve.Evaluate(Mathf.Clamp01(e / waitTime)) * musicVolumeValue;
                SetMusicVolumeValue(val);
                e += Time.deltaTime;
                yield return null;
            }
        }

        private void PlayStoryModeMusic()
        {
            PlayMusic($"Chapter{GameStateController.currentChapterId}Theme");
        }

        private void PlayMusic(string soundName)
        {
            var s = Array.Find(musicSounds, m => m.name == soundName);

            if (s == null)
            {
                Debug.Log($"Music {soundName} not found.");
            }
            else
            {
                musicSource.clip = s.clip;
                musicSource.Play();
            }
        }

        public void FadeOutMusic()
        {
            StartCoroutine(MusicFadeOut());
        }

        public void SetMusicVolumeValue(float volume)
        {
            musicSource.volume = volume;
        }
        
        public void SetSfxVolumeValue(float volume)
        {
            sfxSource.volume = volume;
        }
    }
}
