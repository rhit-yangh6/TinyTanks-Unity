using System;
using _Scripts.GameEngine;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Exception = System.Exception;

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
            ConnectDiscord();
            ChangeActivity("Browsing Menu", "");
        }

        private void OnDestroy()
        {
            try
            {
                EventBus.RemoveListener<string, string>(EventTypes.DiscordStateChange, ChangeActivity);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }

        private void OnApplicationQuit()
        {
            DisconnectDiscord();
        }

        private void ChangeActivity(string newDetails, string newState)
        {
            try
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
            catch
            {
                Debug.Log("Discord is not connected.");
            }
        }

        private static bool IsInGame()
        {
            return SceneManager.GetActiveScene().name == "Story" ||
                   SceneManager.GetActiveScene().name == "Survival" ||
                   SceneManager.GetActiveScene().name == "ShootingRange" ||
                   SceneManager.GetActiveScene().name == "Tutorial";
        }

        private static bool IsInStoryMode()
        {
            return SceneManager.GetActiveScene().name == "Story";
        }

        private static string GetLevelOrNormalState(string newState)
        {
            // Only Story Mode need to display Level state
            if (!IsInStoryMode())
            {
                return newState;
            }
            var currentLevel = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId);
            if (currentLevel == null)
            {
                return newState;
            }
            return Constants.RichPresenceStoryModeLevelStatePrefix + currentLevel.path;
        }

        private void Update()
        {
            try
            {
                discord.RunCallbacks();
            }
            catch (Exception)
            {
                // Debug.Log("Bad Discord Connection. " + e.Message);
            }
        }

        private void ConnectDiscord()
        {
            try
            {
                discord = new Discord.Discord(Constants.DiscordApplicationId,
                    (ulong)Discord.CreateFlags.NoRequireDiscord);
                EventBus.AddListener<string, string>(EventTypes.DiscordStateChange, ChangeActivity);
                Debug.Log("Connected to Discord successfully!");
            }
            catch (Exception e)
            {
                Debug.Log("Failed to Connect to discord. " + e.Message);
            }
        }

        private void DisconnectDiscord()
        {
            try
            {
                discord.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log("Failed to Dispose. " + e.Message);
            }
        }
    }
}
