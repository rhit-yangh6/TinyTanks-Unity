using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class LevelsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject levelScrollListContent;
        [SerializeField] private GameObject levelCellPrefab;
        [SerializeField] private AsyncLoader asyncLoader;
        
        public async void PopulateLevels()
        {
            foreach (Transform child in levelScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            
            var levels = LevelManager.Instance.GetAllLevelsFromChapter(GameStateController.currentChapterId);
            var progress = PlayerData.Instance.GetLevelStatusInChapter(GameStateController.currentChapterId);

            for (var i = 0; i < levels.Length; i++)
            {
                var l = levels[i];
                var cellObj = Instantiate(levelCellPrefab, levelScrollListContent.transform);
                var previewImage = cellObj.transform.Find("Content/Preview").GetComponent<Image>();
                var button = cellObj.GetComponent<Button>();
                var chapterButton = cellObj.GetComponent<ChapterButton>();
                
                var nameOperationAsync =
                    LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                        Constants.LocalizationTableLevelsText, l.name);
                await nameOperationAsync.Task;
                
                if (nameOperationAsync.IsDone)
                {
                    chapterButton.backgroundImage = l.chapterBackgroundSprite;
                    previewImage.sprite = l.levelPreviewSprite;
                    chapterButton.buttonTitle = nameOperationAsync.Result;
                    chapterButton.buttonDescription = l.id;
                    chapterButton.Refresh();
                }
                
                if (progress >= i)
                {
                    button.onClick.AddListener(() =>
                    {
                        asyncLoader.LoadLevelBtn(l.id);
                    });
                }

                // var chapterImage = cellObj.GetComponentsInChildren<Image>()[0];
                // var levelPreviewImage = cellObj.GetComponentsInChildren<Image>()[1];
                // var lockedImg = cellObj.GetComponentsInChildren<Image>()[2];
                //
                // var levelIdText = cellObj.GetComponentsInChildren<TextMeshProUGUI>()[1];
                // var localizeEvent = cellObj.GetComponentInChildren<LocalizeStringEvent>();
                
                // localizeEvent.StringReference =
                //     new LocalizedString(Constants.LocalizationTableLevelsText, 
                //         l.name);
                // levelIdText.text = l.id;
                //
                
                //
                // chapterImage.sprite = l.chapterBackgroundSprite;
                // levelPreviewImage.sprite = l.levelPreviewSprite;
            }
        }
    }
}
