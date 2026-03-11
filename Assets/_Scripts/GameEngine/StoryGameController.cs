using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Utils;

namespace _Scripts.GameEngine
{
    public class StoryGameController : AbstractGameController
    {
        protected override void HandleWin()
        {
            PlayerData.Instance.CompleteLevel();
            SteamManager.UnlockAchievement(Constants.AchievementFirstWinId);

            var level = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId);
            var prize = level.prize;
            winCoinText.text = "+" + prize;
            PlayerData.Instance.GainMoney(prize);

            if (level.weaponPrize != 0)
            {
                WeaponManager.UnlockWeapon(level.weaponPrize);
            }

            if (level.unlocksAchievement != "")
            {
                SteamManager.UnlockAchievement(level.unlocksAchievement);
            }

            /* Achievement - Duality */
            if (level.isBossLevel && PlayerData.Instance.WeaponsSelected() == 2)
            {
                SteamManager.UnlockAchievement(Constants.AchievementDuality);
                WeaponManager.UnlockWeapon(24);
            }

            /* Achievement - Glory in the Last Breath */
            var playerHealth = player.GetComponent<Entity>().Health;
            if (playerHealth is < 2f and > 0)
            {
                SteamManager.UnlockAchievement(Constants.AchievementGloryInTheLastBreath);
                WeaponManager.UnlockWeapon(36);
            }

            base.HandleWin();
        }
    }
}
