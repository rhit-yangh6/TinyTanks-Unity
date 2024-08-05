using System;
using System.Linq;
using _Scripts.Managers;
using _Scripts.Utils;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace _Scripts.UI
{
    public class AchievementsController : MonoBehaviour
    {
        [SerializeField] private GameObject achievementButton;
        [SerializeField] private GameObject achievementScrollListContent;
        [SerializeField] private GameObject achievedHeader, notAchievedHeader;

        public void PopulateAchievements()
        {
            foreach (Transform child in achievementScrollListContent.transform) {
                Destroy(child.gameObject);
            }

            // Get all achievements from Steam and split them based on state
            var achievements = SteamManager.Instance.GetAllAchievements().ToArray();
            // Filter all achievements that isn't defined in the json file
            achievements = Array.FindAll(achievements,
                a => AchievementManager.Instance.GetAchievementById(a.Identifier) != null);
            // Split into achieved/un-achieved lists
            var achievedList = Array.FindAll<Achievement>(achievements, a => a.State);
            var notAchievedList = Array.FindAll<Achievement>(achievements, a => !a.State);
            
            // Sort achieved list by completion time
            // Array.Sort(notAchievedList,
            //     delegate(Achievement a1, Achievement a2) {  
            //         return a1.UnlockTime.CompareTo(a2.UnlockTime);
            //     });
            
            // Sort un-achieved list by isHidden field
            Array.Sort(notAchievedList,
                delegate(Achievement a1, Achievement a2) {  
                    var storedAchievement1 = AchievementManager.Instance.GetAchievementById(a1.Identifier);
                    var storedAchievement2 = AchievementManager.Instance.GetAchievementById(a2.Identifier);
                    return storedAchievement1.isHidden.CompareTo(storedAchievement2.isHidden);
                });

            // Achieved header
            Instantiate(achievedHeader, achievementScrollListContent.transform);

            // Achieved list
            foreach (var achievement in achievedList)
            {
                var storedAchievement = AchievementManager.Instance.GetAchievementById(achievement.Identifier);
                var buttonObj = Instantiate(achievementButton, achievementScrollListContent.transform);
                var achievementIcon = buttonObj.GetComponentsInChildren<Image>()[3];
                var achievementSlider = buttonObj.GetComponentInChildren<Slider>();
                var achievementNameEvent = buttonObj.GetComponentsInChildren<LocalizeStringEvent>()[0];
                var achievementDescEvent = buttonObj.GetComponentsInChildren<LocalizeStringEvent>()[1];

                achievementSlider.gameObject.SetActive(false);
                achievementIcon.sprite = storedAchievement.achievementIconSprite;
                // achievementIcon.texture = ImageUtils.Covert(achievement.GetIcon().GetValueOrDefault());
                achievementNameEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableAchievementText, 
                        achievement.Identifier + "_name");
                achievementDescEvent.StringReference =
                    new LocalizedString(Constants.LocalizationTableAchievementText, 
                        achievement.Identifier + "_desc");
            }
            
            // Un-achieved header
            Instantiate(notAchievedHeader, achievementScrollListContent.transform);

            // Un-achieved list
            foreach (var achievement in notAchievedList)
            {
                var storedAchievement = AchievementManager.Instance.GetAchievementById(achievement.Identifier);
                var buttonObj = Instantiate(achievementButton, achievementScrollListContent.transform);
                var canvasGroup = buttonObj.GetComponent<CanvasGroup>();
                var achievementIcon = buttonObj.GetComponentsInChildren<Image>()[3];
                var achievementSlider = buttonObj.GetComponentInChildren<Slider>();
                var achievementNameEvent = buttonObj.GetComponentsInChildren<LocalizeStringEvent>()[0];
                var achievementDescEvent = buttonObj.GetComponentsInChildren<LocalizeStringEvent>()[1];
                
                canvasGroup.alpha = 0.5f;

                if (storedAchievement.isHidden)
                {
                    achievementSlider.gameObject.SetActive(false);
                    achievementIcon.sprite = Resources.Load<Sprite>(
                        "AchievementIcons/HIDDEN");
                    achievementNameEvent.StringReference =
                        new LocalizedString(Constants.LocalizationTableUIText, 
                            Constants.LocalizationHiddenAchievementNameKey);
                    achievementDescEvent.StringReference =
                        new LocalizedString(Constants.LocalizationTableUIText, 
                            Constants.LocalizationHiddenAchievementDescKey);
                }
                else
                {
                    if (storedAchievement.trackerStats != null)
                    {
                        var currentValue = SteamManager.Instance.GetStatValue(storedAchievement.trackerStats);
                        achievementSlider.maxValue = storedAchievement.targetValue;
                        achievementSlider.value = currentValue;
                    }
                    else
                    {
                        achievementSlider.gameObject.SetActive(false);
                    }
                    achievementIcon.sprite = storedAchievement.achievementLockedIconSprite;
                    achievementNameEvent.StringReference =
                        new LocalizedString(Constants.LocalizationTableAchievementText, 
                            achievement.Identifier + "_name");
                    achievementDescEvent.StringReference =
                        new LocalizedString(Constants.LocalizationTableAchievementText, 
                            achievement.Identifier + "_desc");
                }
            }
        }
    }
}