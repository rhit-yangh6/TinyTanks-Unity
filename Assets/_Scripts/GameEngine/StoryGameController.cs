using System;
using System.Collections;
using System.Globalization;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;

namespace _Scripts.GameEngine
{
    public class StoryGameController : AbstractGameController
    {
        protected override void HandleWin()
        {
            PlayerData.Instance.CompleteLevel();
            // Unlock FIRST_WIN achievement during level completion
            SteamManager.UnlockAchievement(Constants.AchievementFirstWinId);
            
            var level = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId);
            var prize = level.prize;
            winCoinText.text = "+" + prize;
            PlayerData.Instance.GainMoney(prize);

            // Unlock weapon?
            if (level.weaponPrize != 0)
            {
                WeaponManager.UnlockWeapon(level.weaponPrize);
            }
            
            // Unlock achievement?
            if (level.unlocksAchievement != "")
            {
                SteamManager.UnlockAchievement(level.unlocksAchievement);
            }
            
            // Custom checkers
            if (level.isBossLevel && PlayerData.Instance.WeaponsSelected() == 2)
            {
                SteamManager.UnlockAchievement(Constants.AchievementDuality);
                WeaponManager.UnlockWeapon(24); // Yin-yang 24
            }
            
            base.HandleWin();
        }
    }
}
