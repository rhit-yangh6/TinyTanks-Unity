using _Scripts.GameEngine;
using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class LevelDetailPanel : MonoBehaviour
    {
        [SerializeField] private Image levelPreviewImage;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button startButton;
        [SerializeField] private AsyncLoader asyncLoader;
        
        public void SetDetails(string levelId)
        {
            var level = LevelManager.Instance.GetLevelById(levelId);
            levelPreviewImage.sprite = level.levelPreviewSprite;
            levelText.text = "Level " + level.id + ": " + level.name;

            startButton.onClick.RemoveAllListeners();
            
            startButton.onClick.AddListener(() =>
            {
                asyncLoader.LoadLevelBtn(levelId);
            });
        }
    }
}