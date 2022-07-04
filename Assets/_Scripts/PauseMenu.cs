using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool gameIsPaused = false;
        public GameObject pauseMenuUI;
        public GameObject winMenuUI;
        public GameObject loseMenuUI;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
            gameIsPaused = true;
            winMenuUI.SetActive(true);
        }

        public void Lose()
        {
            gameIsPaused = true;
            loseMenuUI.SetActive(true);
        }
    }
}
