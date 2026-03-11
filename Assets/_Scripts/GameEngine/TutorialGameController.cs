using _Scripts.Managers;
using _Scripts.Utils;

namespace _Scripts.GameEngine
{
    public class TutorialGameController : AbstractGameController
    {
        protected override void BroadcastDiscordState()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceTutorialDetail, "");
        }

        protected override void HandleWin()
        {
            SteamManager.UnlockAchievement(Constants.AchievementTutorialCompleted);
            PlayerData.Instance.isTutorialCompleted = true;
            base.HandleWin();
        }

        protected override void HandleLose()
        {
            SteamManager.UnlockAchievement(Constants.AchievementBabySteps);
            base.HandleLose();
        }
    }
}
