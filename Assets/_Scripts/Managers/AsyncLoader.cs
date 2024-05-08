using System;
using System.Collections;
using _Scripts.GameEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.Managers
{
    public class AsyncLoader : MonoBehaviour
    {
        // Source: https://www.youtube.com/watch?v=NyFYNsC3H8k
        // Source 2: https://www.youtube.com/watch?v=tF9RMjF9wDc
        [Header("Menu Screens")]
        [SerializeField] private GameObject loadingScreen;
        // [SerializeField] private GameObject mainMenu;
        
        [Header("Slider")]
        [SerializeField] private Slider loadingSlider;

        [Header("LoadingTime")]
        [Range(0.1f, 3f)]
        [SerializeField] private float blackScreenDefaultWaitTime;

        private CanvasGroup _cg;
        private bool _fadeIn;

        private void Start()
        {
            _cg = loadingScreen.GetComponent<CanvasGroup>();
        }

        public void LoadLevelBtn(string levelToLoad)
        {
            // mainMenu.SetActive(false);
            loadingScreen.SetActive(true);
            
            // Set currentLevelId
            GameStateController.currentLevelId = levelToLoad;
            
            // Set _fadeIn
            _cg.alpha = 0;
            _fadeIn = true;
            
            // Run Async
            StartCoroutine(LoadLevelAsync("Story"));
        }

        public void LoadSurvivalMode()
        {
            loadingScreen.SetActive(true);
            
            // Set _fadeIn
            _cg.alpha = 0;
            _fadeIn = true;
            
            // Run Async
            StartCoroutine(LoadLevelAsync("Survival"));
        }
        
        public void LoadShootingRange()
        {
            loadingScreen.SetActive(true);
            
            // Set _fadeIn
            _cg.alpha = 0;
            _fadeIn = true;
            
            // Run Async
            StartCoroutine(LoadLevelAsync("ShootingRange"));
        }
        
        public void LoadTutorial()
        {
            loadingScreen.SetActive(true);
            
            // Set _fadeIn
            _cg.alpha = 0;
            _fadeIn = true;
            
            // Run Async
            StartCoroutine(LoadLevelAsync("Tutorial"));
        }

        IEnumerator LoadLevelAsync(string sceneToLoad)
        {
            yield return new WaitForSeconds(blackScreenDefaultWaitTime);
            
            // var level = LevelManager.Instance.GetLevelById(levelToLoad);
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

            while (!loadOperation.isDone)
            {
                var progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
                loadingSlider.value = progressValue;
                yield return null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_fadeIn) return;
            
            if (_cg.alpha < 1)
            {
                _cg.alpha += Time.deltaTime * 30;
                if (_cg.alpha >= 1)
                {
                    _fadeIn = false;
                }
            }
        }
    }
}
