using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.SurvivalUpgrades;
using _Scripts.UI;
using _Scripts.Utils;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.GameEngine
{
    public class SurvivalGameController : AbstractGameController
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] protected ModalWindowManager upgradesModalWindowManager;
        [SerializeField] protected GameObject upgradeModalContent;
        [SerializeField] private GameObject upgradeButtonPrefab;
        [SerializeField] private SurvivalUpgrade[] highProbabilityUpgrades;
        [SerializeField] private SurvivalUpgrade[] mediumProbabilityUpgrades;
        [SerializeField] private SurvivalUpgrade[] lowProbabilityUpgrades;
        [SerializeField] private SurvivalUpgrade[] extremeProbabilityUpgrades;

        private int _maxUpgradeNumber = 3;
        private int _wave;
        private TextMeshProUGUI _waveText;

        private void Start()
        {
            HandleBgm();
            _waveText = GameObject.FindGameObjectWithTag("Wave").GetComponent<TextMeshProUGUI>();
            player = GameObject.FindGameObjectWithTag("Player");
            
            playerCharacter = player.GetComponent<PlayerController>();

            timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            EventBus.AddListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
            
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
            Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            
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
                ShowUpgradesWindow();
                return;
            }

            projectileShot = false;
            turn = (turn + 1) % playerNum;
            isInterTurn = false;
            StartCoroutine(HandleMovements());
        }

        private void UpgradeSelected()
        {
            // Resume Game First
            pauseMenuObject.SetActive(true);
            pauseMenuAnimator.Play("Window Out");
            backgroundBlurManager.BlurOutAnim();
            ResumeGame();
            
            // Advance wave
            AdvanceWave();
            projectileShot = false;
            turn = 0;
            isInterTurn = false;
            StartCoroutine(HandleMovements());
        }
        
        private void ShowUpgradesWindow()
        {
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            upgradesModalWindowManager.ModalWindowIn();
            
            // Clear and Re-populate upgrade icons
            foreach (Transform child in upgradeModalContent.transform) {
                Destroy(child.gameObject);
            }

            for (var i = 0; i < _maxUpgradeNumber; i++)
            {
                var survivalUpgrade = GenerateSurvivalUpgrade();
                var buttonObj = Instantiate(upgradeButtonPrefab, upgradeModalContent.transform);
                var button = buttonObj.GetComponent<Button>();
                var survivalUpgradeButton = buttonObj.GetComponent<SurvivalUpgradeButton>();
                
                survivalUpgradeButton.UpdateDisplay(survivalUpgrade);
                
                button.onClick.AddListener(() =>
                {
                    survivalUpgrade.ApplyEffect();
                    UpgradeSelected();
                });
            }
            
            PauseGame();
        }

        private SurvivalUpgrade GenerateSurvivalUpgrade()
        {
            var randomValue = Random.value;

            return randomValue switch
            {
                // >= 0.98f => extremeProbabilityUpgrades[Random.Range(0, extremeProbabilityUpgrades.Length)],
                // >= 0.9f => lowProbabilityUpgrades[Random.Range(0, lowProbabilityUpgrades.Length)],
                >= 0.65f => mediumProbabilityUpgrades[Random.Range(0, mediumProbabilityUpgrades.Length)],
                _ => highProbabilityUpgrades[Random.Range(0, highProbabilityUpgrades.Length)]
            };
        }
    }
}
