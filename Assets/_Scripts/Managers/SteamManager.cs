using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using _Scripts.Utils;
using Steamworks;
using Steamworks.Data;
using Unity.VisualScripting;
using UnityEngine;
using Achievement = Steamworks.Data.Achievement;

namespace _Scripts.Managers
{
    public class SteamManager : MonoBehaviour
    {
        private static bool _isSteamConnected;
        private static SteamManager _i;
        
        public static SteamManager Instance
        {
            get
            {
                if (_i != null) return _i;
                _i = Instantiate(Resources.Load<SteamManager>("SteamManager"));
                DontDestroyOnLoad(_i);
                return _i;
            }
        }
        // Start is called before the first frame update
        private void Start()
        {
            try
            {
                SteamClient.Init(Constants.SteamAppId);
                PrintYourName();
                _isSteamConnected = true;
                EventBus.Broadcast(EventTypes.SteamConnected);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        private void Update()
        {
            Steamworks.SteamClient.RunCallbacks();
        }

        public string GetClientName()
        {
            if (_isSteamConnected)
            {
                return SteamClient.Name;
            }
            return "ERROR";
        }

        public bool IsConnected()
        {
            return _isSteamConnected;
        }
        
        public async Task<Image?> GetAvatar()
        {
            try
            {
                // Get Avatar using await
                return await SteamFriends.GetLargeAvatarAsync( SteamClient.SteamId );
            }
            catch ( Exception e )
            {
                // If something goes wrong, log it
                Debug.Log( e );
                return null;
            }
        }

        private void PrintYourName()
        {
            Debug.Log($"Steam - Logged in as {Steamworks.SteamClient.Name}");
        }

        private void OnDisable()
        {
            SteamClient.Shutdown();
        }

        public void OpenWebOverlay(string url)
        {
            SteamFriends.OpenWebOverlay(url);
        }

        // private void OnApplicationQuit()
        // {
        //     try
        //     {
        //         Steamworks.SteamClient.Shutdown();
        //     }
        //     catch (Exception)
        //     {
        //         // ignored
        //     }
        // }

        public void IsThisAchievementUnlocked(string id)
        {
            try
            {
                var ach = new Achievement(id);
                Debug.Log($"Achievement {id} status: " + ach.State);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public static void UnlockAchievement(string id)
        {
            try
            {
                var ach = new Steamworks.Data.Achievement(id);
                ach.Trigger();
                
                
                Debug.Log($"Achievement {id} unlocked");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            } 
        }
        
        public static void ClearAchievement(string id)
        {
            try
            {
                var ach = new Steamworks.Data.Achievement(id);
                ach.Clear();
                
                Debug.Log($"Achievement {id} cleared");
            }
            catch (Exception e)
            {
                Debug.Log(e);
            } 
        }
        
        public static int IncrementStat(string id)
        {
            try
            {
                var stat = new Steamworks.Data.Stat(id);
                var value = stat.GetInt() + 1;
                stat.Set(value);
                
                Debug.Log($"Stat {id} incremented to {value}");
                return value;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return 0;
        }

        public int GetStatValue(string id)
        {
            try
            {
                return new Stat(id).GetInt();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return 0;
        }

        public IEnumerable<Achievement> GetAllAchievements()
        {
            if (_isSteamConnected)
            {
                return SteamUserStats.Achievements;
            }
            return new List<Achievement>();
        }
    }
}
