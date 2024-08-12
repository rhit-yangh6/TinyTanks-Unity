using System;
using System.Collections;
using _Scripts.GameEngine;
using _Scripts.Utils;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace _Scripts.UI
{
    public class CutSceneController: MonoBehaviour
    {
        [SerializeField] private GameObject bossNameTextPrefab, bossDescTextPrefab;
        [SerializeField] private string nameKey, descKey;
        
        private StoryGameController _gameController;
        private GameObject _inGameUI;
        private MMFeedbacks _mmFeedbacks;

        private string bossName, bossDesc;

        private async void Start()
        {
            _gameController = GameObject.FindGameObjectWithTag("GC").GetComponent<StoryGameController>();
            _inGameUI = GameObject.FindGameObjectWithTag("UI");
            _mmFeedbacks = GetComponent<MMFeedbacks>();
            
            // Get the localized strings
            var nameOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableEnemiesText, nameKey);
            
            var descOperationAsync =
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                    Constants.LocalizationTableEnemiesText, descKey);
            
            await nameOperationAsync.Task;
            await descOperationAsync.Task;
            if (nameOperationAsync.IsDone && descOperationAsync.IsDone)
            {
                bossName = nameOperationAsync.Result;
                bossDesc = descOperationAsync.Result;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _mmFeedbacks.PlayFeedbacks();
        }

        public void SpawnText()
        {
            StartCoroutine(SpawnTextEnumerator());
        }

        IEnumerator SpawnTextEnumerator()
        {
            yield return new WaitForSeconds(0.5f);
            var nameTextObject = Instantiate(bossNameTextPrefab, _inGameUI.transform);
            nameTextObject.GetComponent<TextMeshProUGUI>().text = bossName;
            // nameTextObject.GetComponent<LocalizeStringEvent>().StringReference =
            //     new LocalizedString(Constants.LocalizationTableEnemiesText, nameKey);
            yield return new WaitForSeconds(0.5f);
            var descTextObject =Instantiate(bossDescTextPrefab, _inGameUI.transform);
            descTextObject.GetComponent<TextMeshProUGUI>().text = bossDesc;
            // descTextObject.GetComponent<LocalizeStringEvent>().StringReference =
            //     new LocalizedString(Constants.LocalizationTableEnemiesText, descKey);
            Destroy(nameTextObject, 2f);
            Destroy(descTextObject, 2f);
        }

        public void EnterCutscene()
        {
            _gameController.EnterCutscene();
        }

        public void EndCutscene()
        {
            _gameController.ExitCutscene();
        }
    }
}