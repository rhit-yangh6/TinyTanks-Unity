using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;
using Michsky.UI.Shift;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.GameEngine
{
    public abstract class AbstractGameController : MonoBehaviour
    {
        [SerializeField] protected float turnTime = 45f;
        [SerializeField] protected BlurManager blurManager;
        [SerializeField] protected BlurManager backgroundBlurManager;
        [SerializeField] protected GameObject pauseMenuObject;
        [SerializeField] protected Animator pauseMenuAnimator;
        [SerializeField] protected ModalWindowManager winModalManager;
        [SerializeField] protected TextMeshProUGUI winCoinText;
        [SerializeField] protected ModalWindowManager loseModalManager;
        [SerializeField] protected ModalWindowManager newWeaponModalManager;
        [SerializeField] protected WeaponUnlockedModalWindow weaponUnlockedModalWindow;
        [SerializeField] protected AudioSource bgmAudioSource;
        
        protected bool projectileShot;
        
        protected GameObject player;
        protected PlayerController playerCharacter;
        protected EnemyController[] enemyCharacters;
        protected GameObject[] enemies;
        protected TextMeshProUGUI timerText;
        protected PauseMenu pauseMenu;
        protected float remainingTime;
        protected int turn, playerNum;
        protected bool isEnded;

        protected bool isInterTurn;
        private bool _isInCutScene;

        private void Start()
        {
            HandleBgm();
            player = GameObject.FindGameObjectWithTag("Player");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            playerNum = enemies.Length + 1;
            
            enemyCharacters = new EnemyController[enemies.Length];
            
            for (var i = 0; i < enemies.Length; i++)
            {
                enemyCharacters[i] = enemies[i].GetComponent<EnemyController>();
            }
            playerCharacter = player.GetComponent<PlayerController>();

            timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            EventBus.AddListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
            
            // Update Discord
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceStoryModeDetail, "");
            
            StartCoroutine(HandleMovements());
        }

        private void OnDisable()
        {
            EventBus.RemoveListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.RemoveListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            EventBus.RemoveListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
        }

        private void Update()
        {
            // If It's the player's turn and the player has not performed a shot
            if (turn == 0 && !projectileShot && !_isInCutScene)
            {
                remainingTime -= Time.deltaTime;
                timerText.text = Math.Round(remainingTime).ToString(CultureInfo.InvariantCulture);
                // Time Out, Change Turn
                if (remainingTime <= 0)
                {
                    ChangeTurn();
                }
            }
            // If It's the player's turn and the player has shot
            else if (turn == 0 && projectileShot)
            {
                CheckAggressiveProjectiles();
                timerText.text = "Please Wait...";
            } 
            // Enemies turn
            else
            {
                timerText.text = "Waiting for Opponent...";
                if (projectileShot)
                {
                    CheckAggressiveProjectiles();
                }
            }
            
        }

        protected void CheckAggressiveProjectiles()
        {
            var aggressiveProjectiles = GameObject.FindGameObjectsWithTag("AggressiveProjectile");
            if (aggressiveProjectiles.Length == 0 && !isInterTurn)
            {
                isInterTurn = true;
                Invoke(nameof(ChangeTurn), 1f);
            }
        }

        protected virtual void ChangeTurn()
        {
            if (isEnded) return;
        
            if (playerCharacter.Health <= 0)
            {
                HandleLose();
                return;
            }
            
            if (IsAllEnemyDead())
            {
                HandleWin();
                return;
            }
        
            projectileShot = false;
            turn = (turn + 1) % playerNum;
            isInterTurn = false;
            StartCoroutine(HandleMovements());
        }

        protected virtual void HandleWin()
        {
            isEnded = true;
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            winModalManager.ModalWindowIn();
            PauseGame();
        }

        protected virtual void HandleLose()
        {
            isEnded = true;
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            loseModalManager.ModalWindowIn();
            PauseGame();
        }

        // Hitting the edge or dying in their turn
        protected void EndTurnByCharacter(BuffableEntity be)
        {
            if (playerCharacter.Equals(be))
            {
                if (turn != 0) return;
                ChangeTurn();
                return;
            }

            var idx = Array.IndexOf(enemyCharacters, (EnemyController)be);
            if (idx != turn - 1) return;
            ChangeTurn();
        }

        protected bool IsAllEnemyDead()
        {
            return enemyCharacters.All(t => t.IsDead);
        }

        protected virtual IEnumerator HandleMovements()
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
                if (enemyCharacters[turn - 1].IsDead)
                {
                    ChangeTurn();
                }
                else
                {
                    enemyCharacters[turn - 1].TickBuffs();

                    EnemyController currentEnemy;
                    try
                    {
                        currentEnemy = enemyCharacters[turn - 1];
                    }
                    catch (Exception)
                    {
                        yield break;
                    }
                    
                    if (currentEnemy.IsDead)
                    {
                        // In case that the enemy is dead because of the buff tick
                        ChangeTurn();
                        yield break;
                    }
                    if (t == turn) // Not Skipped
                    {
                        yield return StartCoroutine(currentEnemy.MakeMove());
                    }
                }
            }
        }
        
        protected void ShowNewWeaponWindow(int weaponId)
        {
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            weaponUnlockedModalWindow.Display(weaponId);
            newWeaponModalManager.ModalWindowIn();
            PauseGame();
        }
        
        public void PauseGame()
        {
            Debug.Log("Game Paused");
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            Debug.Log("Game Resumed");
            Time.timeScale = 1f;
        }

        protected void HandleBgm()
        {
            AudioClip audioClip = null;
            var audioFileName = "";
            if (SceneManager.GetActiveScene().name == "Story")
            {
                var level = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId);
                if (level.isBossLevel)
                {
                    audioFileName = "Boss Fight";
                }
                else
                {
                    audioFileName = "Chapter" + GameStateController.currentChapterId;
                }
            }
            else if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                audioFileName = "Tutorial";
            }
            else if (SceneManager.GetActiveScene().name == "Survival")
            {
                audioFileName = "Survival";
            }
            else if (SceneManager.GetActiveScene().name == "ShootingRange")
            {
                audioFileName = "Shooting Range";
            }
            
            PlayerData.Instance.DiscoverMusic(audioFileName);
            audioClip = Instantiate(Resources.Load<AudioClip>("AudioClips/Music/" + audioFileName));

            bgmAudioSource.clip = audioClip;
            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Play();
            }
        }

        public void EnterCutscene()
        {
            _isInCutScene = true;
        }

        public void ExitCutscene()
        {
            _isInCutScene = false;
        }
    }
}