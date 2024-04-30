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
    public class SurvivalGameController : AbstractGameController
    {
        [SerializeField] private GameObject enemyPrefab;
        private int _wave;
        private TextMeshProUGUI _waveText;

        private void Start()
        {
            _waveText = GameObject.FindGameObjectWithTag("Wave").GetComponent<TextMeshProUGUI>();
            player = GameObject.FindGameObjectWithTag("Player");
            
            playerCharacter = player.GetComponent<PlayerController>();

            timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            
            // Update Discord
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceSurvivalModeDetail, Constants.RichPresenceSurvivalModeStatePrefix + 1);
            AdvanceWave();
            
            StartCoroutine(HandleMovements());
        }

        private void AdvanceWave()
        {
            _wave += 1;
            _waveText.text = "Wave " + _wave;
            
            // Spawn Enemies
            Instantiate(enemyPrefab);
            
            //
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            playerNum = enemies.Length + 1;
            
            enemyCharacters = new EnemyController[enemies.Length];
            
            for (var i = 0; i < enemies.Length; i++)
            {
                enemyCharacters[i] = enemies[i].GetComponent<EnemyController>();
            }
            
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceSurvivalModeDetail, Constants.RichPresenceSurvivalModeStatePrefix + _wave);
        }
        
        protected override void ChangeTurn()
        {
            if (PauseMenu.gameIsEnded) return;

            if (playerCharacter.Health <= 0)
            {
                pauseMenu.Lose();
                return;
            }
            
            if (IsAllEnemyDead())
            {
                // pauseMenu.Win();
                // TODO: PERKS
                AdvanceWave();
                projectileShot = false;
                turn = 0;
                isInterTurn = false;
                StartCoroutine(HandleMovements());
                return;
            }

            projectileShot = false;
            turn = (turn + 1) % playerNum;
            isInterTurn = false;
            StartCoroutine(HandleMovements());
        }
    }
}
