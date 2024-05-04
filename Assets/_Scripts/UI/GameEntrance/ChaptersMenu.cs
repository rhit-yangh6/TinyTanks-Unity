using _Scripts.GameEngine;
using _Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class ChaptersMenu : MonoBehaviour
    {
        [SerializeField] private GameObject chapterScrollListContent;
        [SerializeField] private GameObject chapterCellPrefab;
        [SerializeField] private GameObject levelsMenu;
        
        private void OnEnable()
        {
            PopulateChapters();
        }

        private void PopulateChapters()
        {
            foreach (Transform child in chapterScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            var chapters = LevelManager.Instance.GetAllChapters();
             
            foreach (var c in chapters)
            {
                var cellObj = Instantiate(chapterCellPrefab, chapterScrollListContent.transform);
                var s = cellObj.GetComponentsInChildren<Image>()[1];
                var lockedImg = cellObj.GetComponentsInChildren<Image>()[2];
                var tmpGUI = cellObj.GetComponentInChildren<TextMeshProUGUI>();
                var button = cellObj.GetComponent<Button>();

                tmpGUI.text = c.name;

                if (PlayerData.Instance.IsChapterUnlocked(c.id))
                {
                    button.onClick.AddListener(() =>
                    { 
                        GameStateController.currentChapterId = c.id; 
                        levelsMenu.SetActive(true);
                        gameObject.SetActive(false);
                    });
                    var color = lockedImg.color;
                    color.a = 0;
                    lockedImg.color = color;
                }
                else
                {
                    var color = lockedImg.color;
                    color.a = 0.7f;
                    lockedImg.color = color;
                }
                
                s.sprite = c.chapterPreviewSprite;
            }
        }
    }
}
