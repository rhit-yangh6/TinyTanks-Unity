using System;
using System.Collections;
using System.Globalization;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;
using TMPro;
using UnityEngine;

namespace _Scripts.GameEngine
{
    public class ShootingRangeGameController : AbstractGameController
    {

        private TargetController[] targets;
        
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            playerNum = enemies.Length + 1;
            
            targets = new TargetController[enemies.Length];
            
            for (var i = 0; i < enemies.Length; i++)
            {
                targets[i] = enemies[i].GetComponent<TargetController>();
            }
            
            playerCharacter = player.GetComponent<PlayerController>();

            timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            
            // Update Discord
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceShootingRangeDetail, "");
            
            StartCoroutine(HandleMovements());
        }
        
        private void OnDestroy()
        {
            EventBus.RemoveListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.RemoveListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
        }
        
        protected override void ChangeTurn()
        {
            if (PauseMenu.gameIsEnded) return;

            if (playerCharacter.Health <= 0)
            {
                pauseMenu.Lose();
                return;
            }

            projectileShot = false;
            turn = (turn + 1) % playerNum;
            isInterTurn = false;
            StartCoroutine(HandleMovements());
        }
        
        // Hitting the edge or dying in their turn
        private new void EndTurnByCharacter(BuffableEntity be)
        {
            if (playerCharacter.Equals(be))
            {
                if (turn != 0) return;
                ChangeTurn();
                return;
            }

            var idx = Array.IndexOf(targets, (TargetController)be);
            if (idx != turn - 1) return;
            ChangeTurn();
        }
        
        protected override IEnumerator HandleMovements()
        {
            var t = turn;
            if (turn == 0)
            {
                playerCharacter.TickBuffs();
                playerCharacter.moveable = true;
                remainingTime = turnTime;
            }
            else
            {
                playerCharacter.moveable = false;
                if (targets[turn - 1].IsDead)
                {
                    ChangeTurn();
                }
                else
                {
                    targets[turn - 1].TickBuffs();
                    ChangeTurn();
                }
            }
            yield return 0;
        }
    }
}
