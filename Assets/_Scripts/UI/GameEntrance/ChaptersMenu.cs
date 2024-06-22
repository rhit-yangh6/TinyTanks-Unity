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

        public GameObject scrollBar;
        private float scrollPos = 0;
        private float[] pos;
        private Scrollbar _sb;

        private void Start()
        {
            // _sb = scrollBar.GetComponent<Scrollbar>();
        }

        private void Update()
        {
            // Source: https://www.youtube.com/watch?v=GURPmGoAOoM&list=PLXLwAjIwNypwyI1p81KUaKsdjbIOPTD2A&index=4
            // pos = new float[chapterScrollListContent.transform.childCount];
            // float distance = 1f / (pos.Length - 1f);
            // for (var i = 0; i < pos.Length; i++)
            // {
            //     pos[i] = distance * i;
            // }
            //
            // if (Input.GetMouseButton(0))
            // {
            //     scrollPos = _sb.value;
            // }
            // else
            // {
            //     for (var i = 0; i < pos.Length; i++)
            //     {
            //         if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            //         {
            //             _sb.value = Mathf.Lerp(_sb.value, pos[i], 0.1f);
            //         }
            //     }
            // }
            //
            // for (var i = 0; i < pos.Length; i++)
            // {
            //     if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
            //     {
            //         chapterScrollListContent.transform.GetChild(i).localScale = Vector2.Lerp(
            //             chapterScrollListContent.transform.GetChild(i).localScale,
            //             new Vector2(1f, 1f),
            //             0.1f);
            //         for (var a = 0; a < pos.Length; a++)
            //         {
            //             if (a != i)
            //             {
            //                 chapterScrollListContent.transform.GetChild(a).localScale = Vector2.Lerp(
            //                     chapterScrollListContent.transform.GetChild(a).localScale,
            //                     new Vector2(0.8f, 0.8f),
            //                     0.1f);
            //             }
            //         }
            //     }
            // }
        }

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
        }
    }
}
