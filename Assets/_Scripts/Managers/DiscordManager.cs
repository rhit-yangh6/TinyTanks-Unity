using System;
using System.Data;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
    public class DiscordManager : MonoBehaviour
    {
        private static DiscordManager _i;
        
        public long applicationID = 1226719540390400140;
        [Space] public string details = "Browsing Menu";
        [Space] public string largeImage = "game_icon_1024";
        public string largeText = "Tiny Tanks";

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
            discord = new Discord.Discord(applicationID, (ulong)Discord.CreateFlags.NoRequireDiscord);
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
                    LargeImage = largeImage,
                    LargeText = largeText
                }
            };
            
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord.");
            });
        }

        private static bool IsInGame()
        {
            return LevelManager.Instance.GetCurrentLevel(SceneManager.GetActiveScene().name) != null;
        }

        private static string GetLevelOrNormalState(string newState)
        {
            Level currentLevel = LevelManager.Instance.GetCurrentLevel(SceneManager.GetActiveScene().name);
            if (currentLevel == null)
            {
                return newState;
            }
            return "At Level " + currentLevel.id + ": " + currentLevel.name;
        }

        private void Update()
        {
            discord.RunCallbacks();
        }
    }
}
