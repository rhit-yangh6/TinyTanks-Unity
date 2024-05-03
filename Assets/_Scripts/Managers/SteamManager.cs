using System;
using System.Runtime.CompilerServices;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Managers
{
    public class SteamManager : MonoBehaviour
    {
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
                Steamworks.SteamClient.Init(Constants.SteamAppId);
                PrintYourName();
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

        private void PrintYourName()
        {
            Debug.Log($"Steam - Logged in as {Steamworks.SteamClient.Name}");
        }

        private void OnDisable()
        {
            Steamworks.SteamClient.Shutdown();
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
                var ach = new Steamworks.Data.Achievement(id);
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
    }
}
