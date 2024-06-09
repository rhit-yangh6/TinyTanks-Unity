using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace _Scripts.UI.GameEntrance
{
    public class LevelDetailPanel : MonoBehaviour
    {
        [SerializeField] private Image levelPreviewImage;
        [SerializeField] private LocalizeStringEvent levelTextEvent;
        [SerializeField] private LocalizeStringEvent levelNameEvent;
        [SerializeField] private Button startButton;
        [SerializeField] private AsyncLoader asyncLoader;
        
        public void SetDetails(string levelId)
        {
            var level = LevelManager.Instance.GetLevelById(levelId);
            levelPreviewImage.sprite = level.levelPreviewSprite;
            
            levelTextEvent.StringReference = new LocalizedString(Constants.LocalizationTableUIText,
                    Constants.LocalizationLevelTextKey)
                {{ "level_id", new StringVariable() { Value = level.id } }};
            
            levelNameEvent.StringReference =
                new LocalizedString(Constants.LocalizationTableLevelsText, 
                    level.name);

            startButton.onClick.RemoveAllListeners();
            
            startButton.onClick.AddListener(() =>
            {
                asyncLoader.LoadLevelBtn(levelId);
            });
        }
    }
}