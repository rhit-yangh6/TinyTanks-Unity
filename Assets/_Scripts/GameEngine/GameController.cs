using System;
using System.Collections;
using System.Globalization;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;

namespace _Scripts.GameEngine
{
    public class GameController : AbstractGameController
    {
        protected override void HandleWin()
        {
            PlayerData.Instance.CompleteLevel();
            // Unlock FIRST_WIN achievement during level completion
            SteamManager.UnlockAchievement(Constants.AchievementFirstWinId);
            var prize = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId).prize;

            winCoinText.text = "+" + prize;
            PlayerData.Instance.GainMoney(prize);
            base.HandleWin();
        }
    }
}
