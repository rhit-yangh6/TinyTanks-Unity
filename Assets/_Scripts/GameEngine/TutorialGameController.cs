using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;
using TMPro;
using UnityEngine;

namespace _Scripts.GameEngine
{
    public class TutorialGameController: AbstractGameController
    {
        
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            playerNum = enemies.Length + 1;
            
            enemyCharacters = new EnemyController[enemies.Length];
            
            for (var i = 0; i < enemies.Length; i++)
            {
                enemyCharacters[i] = enemies[i].GetComponent<EnemyController>();
            }
            playerCharacter = player.GetComponent<PlayerController>();

            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            
            // Update Discord
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceStoryModeDetail, "");
            
            StartCoroutine(HandleMovements());
        }
        
        private void Update()
        {
            if (projectileShot)
            {
                CheckAggressiveProjectiles();
            }
        }
        
        protected override void ChangeTurn()
        {
            if (PauseMenu.gameIsEnded) return;
        
            if (playerCharacter.Health <= 0)
            {
                SteamManager.Instance.UnlockAchievement(Constants.AchievementBabySteps);
                pauseMenu.Lose();
                return;
            }
            
            if (IsAllEnemyDead())
            {
                pauseMenu.Win();
                return;
            }
        
            projectileShot = false;
            turn = (turn + 1) % playerNum;
            isInterTurn = false;
            StartCoroutine(HandleMovements());
        }
    }
}