using _Scripts.GameEngine;
using _Scripts.Managers;
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

        public void Restart()
        {
            Resume();
            EventBus.Broadcast(EventTypes.DiscordStateChange, "Playing Story Mode", "");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Win()
        {
            // TODO: Give Weapon
            var prize = LevelManager.Instance.GetLevelByPath(GameStateController.currentLevelPath).prize;

            coinText.text = "+" + prize;
            PlayerData.Instance.GainMoney(prize);
            
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
            var nextLevel = LevelManager.Instance.GetNextLevel(GameStateController.currentLevelPath);
            if (nextLevel != null)
            {
                GameStateController.currentLevelPath = nextLevel.path;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
