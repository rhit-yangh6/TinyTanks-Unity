using System.Collections;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Utils;

namespace _Scripts.GameEngine
{
    public class ShootingRangeGameController : AbstractGameController
    {
        private TargetController[] _targets;

        protected override void InitializeEntities()
        {
            base.InitializeEntities();

            _targets = new TargetController[enemies.Length];
            for (var i = 0; i < enemies.Length; i++)
            {
                _targets[i] = enemies[i].GetComponent<TargetController>();
            }
        }

        protected override void BroadcastDiscordState()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceShootingRangeDetail, "");
        }

        // Targets don't shoot — just tick buffs and immediately end their turn
        protected override IEnumerator HandleMovements()
        {
            var participant = CurrentParticipant;

            if (participant.Faction == TurnFaction.Player)
            {
                playerCharacter.TickBuffs();
                if (playerCharacter.Health <= 0)
                {
                    HandleLose();
                    yield break;
                }

                playerCharacter.moveable = true;
                remainingTime = turnTime;
            }
            else
            {
                playerCharacter.moveable = false;
                if (!participant.Entity.IsDead)
                {
                    participant.Entity.TickBuffs();
                }
                TransitionToNextTurn();
            }
        }
    }
}
