using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.UI;
using _Scripts.Utils;
using TMPro;
using UnityEngine;

namespace _Scripts.GameEngine
{
    public abstract class AbstractGameController : MonoBehaviour
    {
        [SerializeField] private float turnTime = 45f;
        
        protected bool projectileShot;
        
        protected GameObject player;
        protected PlayerController playerCharacter;
        protected EnemyController[] enemyCharacters;
        protected GameObject[] enemies;
        protected TextMeshProUGUI timerText;
        protected PauseMenu pauseMenu;
        protected float remainingTime;
        protected int turn, playerNum;

        protected bool isInterTurn;

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

            timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.AddListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
            
            // Update Discord
            EventBus.Broadcast(EventTypes.DiscordStateChange,
                Constants.RichPresenceStoryModeDetail, "");
            
            StartCoroutine(HandleMovements());
        }

        private void OnDestroy()
        {
            EventBus.RemoveListener(EventTypes.ProjectileShot, () => projectileShot = true);
            EventBus.RemoveListener<BuffableEntity>(EventTypes.EndTurn, EndTurnByCharacter);
        }

        private void Update()
        {
            // If It's the player's turn and the player has not performed a shot
            if (turn == 0 && !projectileShot)
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
            if (PauseMenu.GameIsEnded) return;

            if (playerCharacter.Health <= 0)
            {
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

        protected IEnumerator HandleMovements()
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
                    if (t == turn) // Not Skipped
                    {
                        yield return StartCoroutine(enemyCharacters[turn - 1].MakeMove());
                    }
                }
            }
        }
    }
}