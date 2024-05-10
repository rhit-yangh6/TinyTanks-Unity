using System;
using System.Collections;
using _Scripts.GameEngine;
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
        private static readonly int FadeIn = Animator.StringToHash("FadeIn");

        private Animator musicAnimator;
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.sceneLoaded += OnSceneLoaded;
                musicAnimator = musicSource.GetComponent<Animator>();
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
                    musicAnimator.SetTrigger(FadeOut);
                    yield return new WaitForSeconds(waitTime); 
                    PlayStoryModeMusic();
                    break;
                case "MenuScene":
                    musicAnimator.SetTrigger(FadeOut);
                    yield return new WaitForSeconds(waitTime); 
                    PlayMusic("Menu");
                    break;
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
                musicAnimator.SetTrigger(FadeIn);
                musicSource.clip = s.clip;
                musicSource.Play();
            }
        }
    }
}
