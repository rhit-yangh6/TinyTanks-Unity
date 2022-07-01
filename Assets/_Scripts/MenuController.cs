using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts
{
    public class MenuController : MonoBehaviour
    {
        public void PlayGameYes()
        {
            SceneManager.LoadScene("Level1");
        }

        public void LoadGameYes()
        {
        
        }

        public void ExitButtonYes()
        {
            SaveSystem.SavePlayer();
            Application.Quit();
        }
    }
}
