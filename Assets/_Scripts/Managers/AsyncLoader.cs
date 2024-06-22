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
        [Header("Animator")]
        [SerializeField] private Animator loadingMenuAnimator;
        
        [Header("LoadingTime")]
        [Range(0.1f, 3f)]
        [SerializeField] private float blackScreenDefaultWaitTime;

        public void LoadLevelBtn(string levelToLoad)
        {
            // Set currentLevelId
            GameStateController.currentLevelId = levelToLoad;
            
            // Run Async
            StartCoroutine(LoadLevelAsync("Story"));
        }

        public void ReloadLevel()
        {
            LoadLevelBtn(GameStateController.currentLevelId);
        }

        public void LoadNextLevel()
        {
            var nextLevel = LevelManager.Instance.GetNextLevel(GameStateController.currentLevelId);
            if (nextLevel != null)
            {
                LoadLevelBtn(nextLevel.id);
            }
        }

        public void LoadSurvivalMode()
        {
            // loadingScreen.SetActive(true);
            
            // Run Async
            StartCoroutine(LoadLevelAsync("Survival"));
        }
        
        public void LoadShootingRange()
        {
            // Run Async
            StartCoroutine(LoadLevelAsync("ShootingRange"));
        }
        
        public void LoadTutorial()
        {
            // Run Async
            StartCoroutine(LoadLevelAsync("Tutorial"));
        }

        public void LoadMainMenu()
        {
            // Run Async
            StartCoroutine(LoadLevelAsync("Main Menu"));
        }

        private IEnumerator LoadLevelAsync(string sceneToLoad)
        {
            loadingMenuAnimator.Play("UI_LoadingMenu_Loading");
            yield return new WaitForSeconds(blackScreenDefaultWaitTime);
            
            var loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }
    }
}
