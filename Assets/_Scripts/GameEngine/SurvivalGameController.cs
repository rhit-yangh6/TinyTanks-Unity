using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.SurvivalUpgrades;
using _Scripts.UI;
using _Scripts.Utils;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        // Survival spawns enemies dynamically — only init the player at start,
        // then rebuild turn order each wave.
        protected override void InitializeEntities()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerCharacter = player.GetComponent<PlayerController>();

            enemies = new GameObject[0];
            enemyCharacters = new EnemyController[0];
            RebuildTurnOrder();
        }

        protected override void OnStartComplete()
        {
            _waveText = GameObject.FindGameObjectWithTag("Wave").GetComponent<TextMeshProUGUI>();
            AdvanceWave();
        }

        protected override void BroadcastDiscordState()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceSurvivalModeDetail, Constants.RichPresenceSurvivalModeStatePrefix + 1);
        }

        private void AdvanceWave()
        {
            _wave += 1;
            _waveText.text = "Wave " + _wave;

            Instantiate(enemyPrefab,
                spawnPoints[Random.Range(0, spawnPoints.Length)].position,
                Quaternion.identity);

            // Re-discover all living enemies and rebuild turn order
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemyCharacters = new EnemyController[enemies.Length];
            for (var i = 0; i < enemies.Length; i++)
            {
                enemyCharacters[i] = enemies[i].GetComponent<EnemyController>();
            }
            RebuildTurnOrder();

            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceSurvivalModeDetail, Constants.RichPresenceSurvivalModeStatePrefix + _wave);
        }

        protected override void TransitionToNextTurn()
        {
            if (currentState == TurnState.GameOver) return;

            if (playerCharacter.Health <= 0)
            {
                HandleLose();
                return;
            }

            if (IsAllEnemyDead())
            {
                ShowUpgradesWindow();
                return;
            }

            base.TransitionToNextTurn();
        }

        protected override void HandleLose()
        {
            currentState = TurnState.GameOver;
            StopAllCoroutines();
            pauseMenu.Lose();
        }

        private void UpgradeSelected()
        {
            pauseMenuObject.SetActive(true);
            pauseMenuAnimator.Play("Window Out");
            backgroundBlurManager.BlurOutAnim();
            ResumeGame();

            AdvanceWave();
            BeginFirstTurn();
        }

        private void ShowUpgradesWindow()
        {
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            upgradesModalWindowManager.ModalWindowIn();

            foreach (Transform child in upgradeModalContent.transform)
            {
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
                >= 0.65f => mediumProbabilityUpgrades[Random.Range(0, mediumProbabilityUpgrades.Length)],
                _ => highProbabilityUpgrades[Random.Range(0, highProbabilityUpgrades.Length)]
            };
        }
    }
}
