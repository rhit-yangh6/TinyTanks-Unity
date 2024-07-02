using System;
using System.Linq;
using _Scripts.Managers;
using _Scripts.Utils;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

            var achievements = SteamManager.Instance.GetAllAchievements().ToArray();
            var achievedList = Array.FindAll<Achievement>(achievements, a => a.State);
            var notAchievedList = Array.FindAll<Achievement>(achievements, a => !a.State);

            Instantiate(achievedHeader, achievementScrollListContent.transform);

            foreach (var achievement in achievedList)
            {
                var buttonObj = Instantiate(achievementButton, achievementScrollListContent.transform);
                var achievementIcon = buttonObj.GetComponentInChildren<RawImage>();
                var achievementName = buttonObj.GetComponentsInChildren<TextMeshProUGUI>()[0];
                var achievementDesc = buttonObj.GetComponentsInChildren<TextMeshProUGUI>()[1];

                achievementIcon.texture = ImageUtils.Covert(achievement.GetIcon().GetValueOrDefault());
                achievementName.text = achievement.Name;
                achievementDesc.text = achievement.Description;
            }
            
            Instantiate(notAchievedHeader, achievementScrollListContent.transform);

            foreach (var achievement in notAchievedList)
            {
                var buttonObj = Instantiate(achievementButton, achievementScrollListContent.transform);
                var canvasGroup = buttonObj.GetComponent<CanvasGroup>();
                var achievementIcon = buttonObj.GetComponentInChildren<RawImage>();
                
                achievementIcon.texture = ImageUtils.Covert(achievement.GetIcon().GetValueOrDefault());
                
                canvasGroup.alpha = 0.5f;
            }
        }
    }
}