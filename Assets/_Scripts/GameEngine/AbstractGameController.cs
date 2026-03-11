using System;
using System.Collections;
using System.Collections.Generic;
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
    public struct TurnParticipant
    {
        public BuffableEntity Entity;
        public TurnFaction Faction;
    }

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

        // Entity references (kept for subclass convenience)
        protected GameObject player;
        protected PlayerController playerCharacter;
        protected EnemyController[] enemyCharacters;
        protected GameObject[] enemies;

        // UI
        protected TextMeshProUGUI timerText;
        protected PauseMenu pauseMenu;
        protected float remainingTime;

        // Turn system
        protected readonly List<TurnParticipant> turnOrder = new();
        protected int turnIndex;
        protected TurnState currentState = TurnState.PlayerMoving;

        private bool _isInCutScene;
        private Coroutine _activeMovementCoroutine;
        private Coroutine _projectileCheckCoroutine;

        #region Initialization

        protected virtual void Start()
        {
            HandleBgm();
            InitializeEntities();
            InitializeUI();
            RegisterEventListeners();
            OnStartComplete();
            BroadcastDiscordState();
            BeginFirstTurn();
        }

        protected virtual void OnDisable()
        {
            UnregisterEventListeners();
        }

        protected virtual void InitializeEntities()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerCharacter = player.GetComponent<PlayerController>();

            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemyCharacters = new EnemyController[enemies.Length];
            for (var i = 0; i < enemies.Length; i++)
            {
                enemyCharacters[i] = enemies[i].GetComponent<EnemyController>();
            }

            RebuildTurnOrder();
        }

        protected void RebuildTurnOrder()
        {
            turnOrder.Clear();

            // 1. Player always first
            turnOrder.Add(new TurnParticipant { Entity = playerCharacter, Faction = TurnFaction.Player });

            // 2. Allies
            foreach (var obj in SafeFindWithTag("Ally"))
            {
                var entity = obj.GetComponent<BuffableEntity>();
                if (entity != null)
                    turnOrder.Add(new TurnParticipant { Entity = entity, Faction = TurnFaction.Ally });
            }

            // 3. Enemies
            foreach (var ec in enemyCharacters)
            {
                turnOrder.Add(new TurnParticipant { Entity = ec, Faction = TurnFaction.Enemy });
            }

            // 4. Neutrals
            foreach (var obj in SafeFindWithTag("Neutral"))
            {
                var entity = obj.GetComponent<BuffableEntity>();
                if (entity != null)
                    turnOrder.Add(new TurnParticipant { Entity = entity, Faction = TurnFaction.Neutral });
            }
        }

        protected virtual void InitializeUI()
        {
            var timerObj = GameObject.FindGameObjectWithTag("Timer");
            if (timerObj != null)
                timerText = timerObj.GetComponent<TextMeshProUGUI>();

            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
        }

        private void RegisterEventListeners()
        {
            EventBus.AddListener(EventTypes.ProjectileShot, OnProjectileShot);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            EventBus.AddListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
        }

        private void UnregisterEventListeners()
        {
            EventBus.RemoveListener(EventTypes.ProjectileShot, OnProjectileShot);
            EventBus.RemoveListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            EventBus.RemoveListener<int>(EventTypes.WeaponUnlocked, ShowNewWeaponWindow);
        }

        protected virtual void OnStartComplete() { }

        protected virtual void BroadcastDiscordState()
        {
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceStoryModeDetail, "");
        }

        protected virtual void BeginFirstTurn()
        {
            turnIndex = 0;
            currentState = TurnState.PlayerMoving;
            StartMovementCoroutine();
        }

        #endregion

        #region Update Loop

        private void Update()
        {
            if (_isInCutScene) return;

            switch (currentState)
            {
                case TurnState.PlayerMoving:
                    UpdatePlayerMovingState();
                    break;

                case TurnState.PlayerShooting:
                    if (timerText != null) timerText.text = "Please Wait...";
                    BeginProjectileCheck();
                    break;

                case TurnState.NpcTurn:
                    if (timerText != null) timerText.text = "Waiting for Opponent...";
                    break;

                case TurnState.WaitingForProjectiles:
                    if (timerText != null) timerText.text = "Waiting for Opponent...";
                    BeginProjectileCheck();
                    break;

                case TurnState.InterTurn:
                case TurnState.GameOver:
                    break;
            }
        }

        private void UpdatePlayerMovingState()
        {
            remainingTime -= Time.deltaTime;
            if (timerText != null)
                timerText.text = Math.Round(remainingTime).ToString(CultureInfo.InvariantCulture);

            if (remainingTime <= 0)
            {
                TransitionToNextTurn();
            }
        }

        #endregion

        #region Projectile Shot Callback

        private void OnProjectileShot()
        {
            switch (currentState)
            {
                case TurnState.PlayerMoving:
                    currentState = TurnState.PlayerShooting;
                    break;
                case TurnState.NpcTurn:
                    currentState = TurnState.WaitingForProjectiles;
                    break;
            }
        }

        #endregion

        #region Projectile Resolution

        private void BeginProjectileCheck()
        {
            if (_projectileCheckCoroutine == null)
            {
                _projectileCheckCoroutine = StartCoroutine(WaitForProjectilesAndChangeTurn());
            }
        }

        private IEnumerator WaitForProjectilesAndChangeTurn()
        {
            while (GameObject.FindGameObjectsWithTag("AggressiveProjectile").Length > 0)
            {
                yield return new WaitForSeconds(0.25f);
            }

            currentState = TurnState.InterTurn;
            yield return new WaitForSeconds(1f);

            _projectileCheckCoroutine = null;
            TransitionToNextTurn();
        }

        #endregion

        #region Turn Management

        protected TurnParticipant CurrentParticipant => turnOrder[turnIndex];

        protected bool IsPlayerTurn => CurrentParticipant.Faction == TurnFaction.Player;

        protected virtual void TransitionToNextTurn()
        {
            if (currentState == TurnState.GameOver) return;

            CancelProjectileCheck();

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

            AdvanceTurnIndex();

            currentState = IsPlayerTurn ? TurnState.PlayerMoving : TurnState.NpcTurn;
            StartMovementCoroutine();
        }

        private void AdvanceTurnIndex()
        {
            var start = turnIndex;
            do
            {
                turnIndex = (turnIndex + 1) % turnOrder.Count;
                var p = turnOrder[turnIndex];

                // Player turn is always valid (alive check is in TransitionToNextTurn)
                if (p.Faction == TurnFaction.Player) break;

                // Skip dead non-player participants
                if (!p.Entity.IsDead) break;
            } while (turnIndex != start);
        }

        protected void EndTurnByCharacter(BuffableEntity be)
        {
            if (currentState == TurnState.InterTurn || currentState == TurnState.GameOver) return;

            // Only end turn if this entity is the current participant
            if (turnIndex >= turnOrder.Count) return;
            if (!CurrentParticipant.Entity.Equals(be)) return;

            CancelProjectileCheck();
            TransitionToNextTurn();
        }

        protected virtual IEnumerator HandleMovements()
        {
            var participant = CurrentParticipant;
            var entity = participant.Entity;

            // Tick buffs for the active participant
            entity.TickBuffs();

            if (entity.IsDead)
            {
                TransitionToNextTurn();
                yield break;
            }

            if (participant.Faction == TurnFaction.Player)
            {
                playerCharacter.moveable = true;
                remainingTime = turnTime;
            }
            else
            {
                playerCharacter.moveable = false;

                // Execute AI move if the entity has one
                var actor = entity.GetComponent<ITurnActor>();
                if (actor != null)
                {
                    yield return StartCoroutine(actor.MakeMove());
                }
            }
        }

        protected bool IsAllEnemyDead()
        {
            return turnOrder
                .Where(p => p.Faction == TurnFaction.Enemy)
                .All(p => p.Entity.IsDead);
        }

        #endregion

        #region Win / Lose

        protected virtual void HandleWin()
        {
            SetGameOver();
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            winModalManager.ModalWindowIn();
            PauseGame();
        }

        protected virtual void HandleLose()
        {
            SetGameOver();
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            loseModalManager.ModalWindowIn();
            PauseGame();
        }

        private void SetGameOver()
        {
            currentState = TurnState.GameOver;
            StopActiveCoroutines();
        }

        #endregion

        #region Weapon Unlock Modal

        protected void ShowNewWeaponWindow(int weaponId)
        {
            pauseMenuAnimator.Play("Window In");
            backgroundBlurManager.BlurInAnim();
            blurManager.BlurInAnim();
            weaponUnlockedModalWindow.Display(weaponId);
            newWeaponModalManager.ModalWindowIn();
            PauseGame();
        }

        #endregion

        #region Pause / Resume

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

        public void EnterCutscene()
        {
            _isInCutScene = true;
        }

        public void ExitCutscene()
        {
            _isInCutScene = false;
        }

        #endregion

        #region BGM

        protected void HandleBgm()
        {
            var audioFileName = "";
            if (SceneManager.GetActiveScene().name == "Story")
            {
                var level = LevelManager.Instance.GetLevelById(GameStateController.currentLevelId);
                audioFileName = level.isBossLevel ? "Boss Fight" : "Chapter" + GameStateController.currentChapterId;
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
            var audioClip = Instantiate(Resources.Load<AudioClip>("AudioClips/Music/" + audioFileName));

            bgmAudioSource.clip = audioClip;
            if (!bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Play();
            }
        }

        #endregion

        #region Helpers

        private void StartMovementCoroutine()
        {
            if (_activeMovementCoroutine != null)
                StopCoroutine(_activeMovementCoroutine);
            _activeMovementCoroutine = StartCoroutine(HandleMovements());
        }

        private void CancelProjectileCheck()
        {
            if (_projectileCheckCoroutine != null)
            {
                StopCoroutine(_projectileCheckCoroutine);
                _projectileCheckCoroutine = null;
            }
        }

        private void StopActiveCoroutines()
        {
            if (_activeMovementCoroutine != null)
            {
                StopCoroutine(_activeMovementCoroutine);
                _activeMovementCoroutine = null;
            }
            CancelProjectileCheck();
            CancelInvoke();
        }

        private static GameObject[] SafeFindWithTag(string tag)
        {
            try { return GameObject.FindGameObjectsWithTag(tag); }
            catch { return Array.Empty<GameObject>(); }
        }

        #endregion
    }
}
