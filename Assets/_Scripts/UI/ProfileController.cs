using System;
using System.Threading.Tasks;
using _Scripts.GameEngine;
using _Scripts.Managers;
using _Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ProfileController : MonoBehaviour
    {
        [SerializeField] private RawImage avatarImage;
        [SerializeField] private TextMeshProUGUI profileText;
        [SerializeField] private TextMeshProUGUI coinText;
        
        private void Awake()
        {
            EventBus.AddListener(EventTypes.CoinChanged, UpdateCoins);
            EventBus.AddListener(EventTypes.SteamConnected, UpdateProfile);
        }
        
        private void Start()
        {
            UpdateCoins();
            UpdateProfile();
        }
        
        private void UpdateCoins()
        {
            coinText.text = PlayerData.Instance.coins.ToString();
        }

        private void OnDisable()
        {
            EventBus.RemoveListener(EventTypes.CoinChanged, UpdateCoins);
            EventBus.RemoveListener(EventTypes.SteamConnected, UpdateProfile);
        }

        private async void UpdateProfile()
        {
            if (!SteamManager.Instance.IsConnected()) { return; }
            var avatar = SteamManager.Instance.GetAvatar();
            // Use Task.WhenAll, to cache multiple items at the same time
            await Task.WhenAll( avatar );
            
            profileText.text = SteamManager.Instance.GetClientName();

            if (avatar.Result != null) avatarImage.texture = ImageUtils.Covert(avatar.Result.Value);
        }
    }
}