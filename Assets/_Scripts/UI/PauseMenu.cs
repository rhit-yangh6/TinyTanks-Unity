using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool gameIsPaused = false;
        public static bool gameIsEnded = false;
        
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject winMenuUI;
        [SerializeField] private GameObject loseMenuUI;
        [SerializeField] private TextMeshProUGUI coinText;
        
        private void Awake()
        {
            gameIsEnded = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !gameIsEnded)
            {
                if (gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            gameIsPaused = false;
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            gameIsPaused = true;
        }

        public void LoadMenu()
        {
            SaveSystem.SavePlayer();
            Resume();
            SceneManager.LoadScene("MenuScene");
        }
        
        public void SkipTutorial()
        {
            PlayerData.Instance.isTutorialCompleted = true;
            SaveSystem.SavePlayer();
            Resume();
            SceneManager.LoadScene("MenuScene");
        }

        public void Restart()
        {
            Resume();
            EventBus.Broadcast(EventTypes.DiscordStateChange, "Playing Story Mode", "");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Win()
        {
            // TODO: Give Weapon
            if (SceneManager.GetActiveScene().name == "Story")
            {
                PlayerData.Instance.CompleteLevel();
                // Unlock FIRST_WIN achievement during level completion
                SteamManager.UnlockAchievement(Constants.AchievementFirstWinId);
                var prize = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId).prize;

                coinText.text = "+" + prize;
                PlayerData.Instance.GainMoney(prize);
            } else if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                SteamManager.UnlockAchievement(Constants.AchievementTutorialCompleted);
                PlayerData.Instance.isTutorialCompleted = true;
            }
            
            gameIsEnded = true;
            winMenuUI.SetActive(true);
        }

        public void Lose()
        {
            gameIsEnded = true;
            loseMenuUI.SetActive(true);
        }

        public void Next()
        {
            Resume();
            var nextLevel = LevelManager.Instance.GetNextLevel(GameStateController.currentLevelId);
            if (nextLevel != null)
            {
                GameStateController.currentLevelId = nextLevel.id;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
