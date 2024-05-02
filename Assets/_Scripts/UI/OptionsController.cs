using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class OptionsController : MonoBehaviour
    {
        public void StartTutorial()
        {
            SceneManager.LoadScene("Tutorial");
        }
    }
}