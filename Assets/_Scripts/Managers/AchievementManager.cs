using System;
using UnityEngine;

namespace _Scripts.Managers
{
    public class AchievementManager : MonoBehaviour
    {
        private static AchievementManager _i;
        private static StoredAchievements _achievementsFromJson;
        
        public static AchievementManager Instance
        {
            get
            {
                if (_i == null)
                {
                    _i = Instantiate(Resources.Load<AchievementManager>("AchievementManager"));
                    LoadAchievements();
                }
                return _i;
            }
        }
        
        private static void LoadAchievements()
        {
            var jsonFile = Resources.Load<TextAsset>("Data/Achievements");

            _achievementsFromJson = JsonUtility.FromJson<StoredAchievements>(jsonFile.text);
            foreach (var storedAchievement in _achievementsFromJson.achievements)
            {
                storedAchievement.achievementIconSprite = Resources.Load<Sprite>(
                    "AchievementIcons/" + storedAchievement.id + "_ACHIEVED");
                storedAchievement.achievementLockedIconSprite = Resources.Load<Sprite>(
                    "AchievementIcons/" + storedAchievement.id);
            }
        }

        public StoredAchievement GetAchievementById(string idToFind)
        {
            return Array.Find(_achievementsFromJson.achievements, a => a.id == idToFind); 
        }
    }
    
    [Serializable]
    public class StoredAchievement
    {
        // Read from Weapons.json
        public string id, trackerStats;
        public bool isHidden;
        public int targetValue;
        
        // Later Generated
        public Sprite achievementIconSprite;
        public Sprite achievementLockedIconSprite;
    }
    
    [Serializable]
    public class StoredAchievements
    {
        public StoredAchievement[] achievements;
    }
}