using _Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class MenuController : MonoBehaviour
    {
        public void Awake()
        {
            // TODO: Probably move this to somewhere else, facade?
            // Create a DiscordManager Singleton
            var discordManager = DiscordManager.Instance;
            
            // Create a SteamManager Singleton
            var steamManager = SteamManager.Instance;
        }

        public void OnEnable()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange, "Browsing Menu", "");
        }

        public void ExitButtonYes()
        {
            SaveSystem.SavePlayer();
            DiscordManager.Instance.DisconnectDiscord();
            Application.Quit();
        }
    }
}
