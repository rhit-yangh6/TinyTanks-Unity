using System;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class ChaptersMenu : MonoBehaviour
    {
        [SerializeField] private GameObject chapterScrollListContent;
        [SerializeField] private GameObject chapterCellPrefab;
        [SerializeField] private MainPanelManager mainPanelManager;
        [SerializeField] private LevelsMenu levelsMenu;
        [SerializeField] private BlurManager modalWindows;
        [SerializeField] private ModalWindowManager chapterLockedModal;
        [SerializeField] private LayoutGroupPositionFix layoutGroupPositionFix;

        public async void PopulateChapters()
        {
            foreach (Transform child in chapterScrollListContent.transform) {
                Destroy(child.gameObject);
            }
            var chapters = LevelManager.Instance.GetAllChapters();
             
            foreach (var c in chapters)
            {
                var cellObj = Instantiate(chapterCellPrefab, chapterScrollListContent.transform);
                var chapterButton = cellObj.GetComponent<ChapterButton>();
                var button = cellObj.GetComponent<Button>();
                
                var nameOperationAsync =
                    LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                        Constants.LocalizationTableLevelsText, c.name);
                await nameOperationAsync.Task;
            
                if (nameOperationAsync.IsDone)
                {
                    chapterButton.backgroundImage = c.chapterPreviewSprite;
                    chapterButton.buttonTitle = nameOperationAsync.Result;
                    chapterButton.Refresh();
                }
                
                if (PlayerData.Instance.IsChapterUnlocked(c.id))
                {
                    chapterButton.statusItem = ChapterButton.StatusItem.None;
                    button.onClick.AddListener(() =>
                    {
                        GameStateController.currentChapterId = c.id; 
                        mainPanelManager.OpenPanel("Level");
                        levelsMenu.PopulateLevels();
                    });
                }
                else
                {
                    chapterButton.statusItem = ChapterButton.StatusItem.Locked;
                    button.onClick.AddListener(() =>
                    {
                        modalWindows.BlurInAnim();
                        chapterLockedModal.ModalWindowIn();
                    });
                }
                
                // var s = cellObj.GetComponentsInChildren<Image>()[1];
                // var lockedImg = cellObj.GetComponentsInChildren<Image>()[2];
                // var localizeEvent = cellObj.GetComponentInChildren<LocalizeStringEvent>();
                //
                // localizeEvent.StringReference =
                //     new LocalizedString(Constants.LocalizationTableLevelsText, 
                //         c.name);
                // s.sprite = c.chapterPreviewSprite;

                // if (PlayerData.Instance.IsChapterUnlocked(c.id))
                // {
                //     button.onClick.AddListener(() =>
                //     { 
                //         GameStateController.currentChapterId = c.id; 
                //         levelsMenu.SetActive(true);
                //         gameObject.SetActive(false);
                //     });
                //     var color = lockedImg.color;
                //     color.a = 0;
                //     lockedImg.color = color;
                // }
                // else
                // {
                //     var color = lockedImg.color;
                //     color.a = 0.7f;
                //     lockedImg.color = color;
                // }
            }
            
            layoutGroupPositionFix.FixLayout();
        }
    }
}
