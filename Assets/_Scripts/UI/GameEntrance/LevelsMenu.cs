using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class LevelsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject levelScrollListContent;
        [SerializeField] private GameObject levelCellPrefab;
        [SerializeField] private LevelDetailPanel detailPanel;
        
        private void OnEnable() { PopulateLevels(); }

        private void PopulateLevels()
        {
            detailPanel.gameObject.SetActive(false);
            
            foreach (Transform child in levelScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            
            Level[] levels = LevelManager.Instance.GetAllLevelsFromChapter(GameStateController.currentChapterId);
            var progress = PlayerData.Instance.GetLevelStatusInChapter(GameStateController.currentChapterId);

            for (var i = 0; i < levels.Length; i++)
            {
                var l = levels[i];
                var cellObj = Instantiate(levelCellPrefab, levelScrollListContent.transform);
                var s = cellObj.GetComponentsInChildren<Image>()[1];
                
                var lockedImg = cellObj.GetComponentsInChildren<Image>()[2];
                var localizeEvent = cellObj.GetComponentInChildren<LocalizeStringEvent>();
                var button = cellObj.GetComponent<Button>();
                
                localizeEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableLevelsText, 
                        l.name);
                
                if (progress >= i)
                {
                    button.onClick.AddListener(() =>
                    {
                        detailPanel.gameObject.SetActive(true);
                        detailPanel.SetDetails(l.id);
                    });
                    var color = lockedImg.color;
                    color.a = 0;
                    lockedImg.color = color;
                }
                else
                {
                    // No onclick event
                    var color = lockedImg.color;
                    color.a = 0.7f;
                    lockedImg.color = color;
                }

                s.sprite = l.levelPreviewSprite;
            }
        }
    }
}
