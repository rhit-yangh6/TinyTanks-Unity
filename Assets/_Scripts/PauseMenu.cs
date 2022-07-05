using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool gameIsPaused = false;
        public static bool gameIsEnded = false;
        public GameObject pauseMenuUI;
        public GameObject winMenuUI;
        public GameObject loseMenuUI;

        // Update is called once per frame
        void Update()
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
            gameIsEnded = false;
            Resume();
            SceneManager.LoadScene("MenuScene");
        }

        public void Restart()
        {
            gameIsEnded = false;
            Resume();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Win()
        {
            // Give money?
            gameIsEnded = true;
            winMenuUI.SetActive(true);
        }

        public void Lose()
        {
            gameIsEnded = true;
            loseMenuUI.SetActive(true);
        }
    }
}
