using System;
using System.Data;
using _Scripts.Utils;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
    public class DiscordManager : MonoBehaviour
    {
        private static DiscordManager _i;
        private long time;

        private static bool _instanceExists;
        private Discord.Discord discord;
        
        public static DiscordManager Instance
        {
            get
            {
                if (_i != null) return _i;
                _i = Instantiate(Resources.Load<DiscordManager>("DiscordManager"));
                DontDestroyOnLoad(_i);
                return _i;
            }
        }
    
        // Start is called before the first frame update
        private void Start()
        {
            discord = new Discord.Discord(Constants.DiscordApplicationId, (ulong)Discord.CreateFlags.NoRequireDiscord);
            // Register listeners
            EventBus.AddListener<string, string>(EventTypes.DiscordStateChange, ChangeActivity);
        }

        private void OnDestroy()
        {
            // EventBus.RemoveListener<string>(EventTypes.DiscordStateChange, ChangeActivity);
            discord.Dispose();
        }

        private void ChangeActivity(string newDetails, string newState)
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                State = GetLevelOrNormalState(newState),
                Details = newDetails,
                Timestamps =
                {
                    Start = IsInGame() ? DateTimeOffset.Now.ToUnixTimeMilliseconds() : 0
                },
                Assets =
                {
                    LargeImage = Constants.RichPresenceLargeImage,
                    LargeText = Constants.RichPresenceLargeText
                }
            };
            
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord.");
            });
        }

        private static bool IsInGame()
        {
            if (LevelManager.Instance.GetCurrentLevel(SceneManager.GetActiveScene().name) != null) return true;
            return SceneManager.GetActiveScene().name == "Survival" ||
                   SceneManager.GetActiveScene().name == "ShootingRange";
        }

        private static string GetLevelOrNormalState(string newState)
        {
            var currentLevel = LevelManager.Instance.GetCurrentLevel(SceneManager.GetActiveScene().name);
            if (currentLevel == null)
            {
                return newState;
            }
            return Constants.RichPresenceStoryModeLevelStatePrefix + currentLevel.id + ": " + currentLevel.name;
        }

        private void Update()
        {
            discord.RunCallbacks();
        }
    }
}
