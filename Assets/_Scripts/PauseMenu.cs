using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        public static bool GameIsEnded = false;
        
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject winMenuUI;
        [SerializeField] private GameObject loseMenuUI;
        [SerializeField] private TextMeshProUGUI coinText;
        
        private void Awake()
        {
            GameIsEnded = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !GameIsEnded)
            {
                if (GameIsPaused)
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
            GameIsPaused = false;
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Win()
        {
            // Give money?
            
            var sceneName = SceneManager.GetActiveScene().name;
            var prize = LevelManager.Instance.GetLevelByPath(sceneName).prize;

            coinText.text = "+" + prize;
            PlayerData.Instance.GainMoney(prize);
            
            GameIsEnded = true;
            winMenuUI.SetActive(true);
        }

        public void Lose()
        {
            GameIsEnded = true;
            loseMenuUI.SetActive(true);
        }
    }
}
