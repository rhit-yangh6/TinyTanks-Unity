using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private TextMeshProUGUI timerText;
        
        private PlayerController playerCharacter;
        private EnemyController[] enemyCharacters;
        private GameObject[] enemies;

        private int turn = 0, playerNum = 0;
        private float turnTime = 45f, remainingTime;
        private bool isTiming;

        private void Start()
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            playerNum = enemies.Length + 1;
            enemyCharacters = new EnemyController[enemies.Length];
            
            for (var i = 0; i < enemies.Length; i++)
            {
                enemyCharacters[i] = enemies[i].GetComponent<EnemyController>();
            }
            playerCharacter = player.GetComponent<PlayerController>();
            
            isTiming = true;
            StartCoroutine(HandleMovements());
        }

        private void Update()
        {
            
            // Timer Stuff
            // TODO: Turn off timing when projectile is out
            if (isTiming)
            {
                remainingTime -= Time.deltaTime;
                timerText.text = Math.Round(remainingTime).ToString(CultureInfo.InvariantCulture);
                if (remainingTime <= 0)
                {
                    ChangeTurn();
                }
            }
            else
            {
                timerText.text = "Waiting for Opponent...";
            }
        }

        public void ChangeTurn()
        {
            // TODO: Check Win / Lose
            
            turn = (turn + 1) % playerNum;
            StartCoroutine(HandleMovements());
        }

        private IEnumerator HandleMovements()
        {
            if (turn == 0)
            {
                playerCharacter.moveable = true;
                remainingTime = turnTime;
                isTiming = true;
            }
            else
            {
                isTiming = false;
                playerCharacter.moveable = false;
                if (enemyCharacters[turn - 1].isDead)
                {
                    ChangeTurn();
                }
                else
                {
                    yield return StartCoroutine(enemyCharacters[turn - 1].MakeMove());
                }
            }
        }

    }
}
