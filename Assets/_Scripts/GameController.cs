using System;
using System.Collections;
using System.Globalization;
using _Scripts.Buffs;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float turnTime = 45f;
        [SerializeField] private PauseMenu pauseMenu;
        
        [HideInInspector] public bool projectileShot;
        
        private PlayerController _playerCharacter;
        private EnemyController[] _enemyCharacters;
        private GameObject[] _enemies;

        private int _turn = 0, _playerNum = 0;
        private float _remainingTime;

        private void Start()
        {
            _enemies = GameObject.FindGameObjectsWithTag("Enemy");
            _playerNum = _enemies.Length + 1;
            _enemyCharacters = new EnemyController[_enemies.Length];
            
            for (var i = 0; i < _enemies.Length; i++)
            {
                _enemyCharacters[i] = _enemies[i].GetComponent<EnemyController>();
            }
            _playerCharacter = player.GetComponent<PlayerController>();
            
            StartCoroutine(HandleMovements());
        }

        private void Update()
        {

            // If It's the player's turn and the player has not performed a shot
            if (_turn == 0 && !projectileShot)
            {
                _remainingTime -= Time.deltaTime;
                timerText.text = Math.Round(_remainingTime).ToString(CultureInfo.InvariantCulture);
                // Time Out, Change Turn
                if (_remainingTime <= 0)
                {
                    ChangeTurn();
                }
            } 
            // If It's the player's turn and the player has shot
            else if (_turn == 0 && projectileShot)
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

        private void CheckAggressiveProjectiles()
        {
            var aggressiveProjectiles = GameObject.FindGameObjectsWithTag("AggressiveProjectile");
            if (aggressiveProjectiles.Length == 0)
            {
                ChangeTurn();
            }
        }

        private void ChangeTurn()
        {

            if (PauseMenu.GameIsEnded) return;

            if (_playerCharacter.Health <= 0)
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
            _turn = (_turn + 1) % _playerNum;
            StartCoroutine(HandleMovements());
        }

        private bool IsAllEnemyDead()
        {
            for ( int i = 0; i < _enemyCharacters.Length; i++ ) {
                if ( !_enemyCharacters[ i ].isDead ) {
                    return false;
                }
            }
 
            return true;
        }

        private IEnumerator HandleMovements()
        {
            if (_turn == 0)
            {
                _playerCharacter.TickBuffs();
                _playerCharacter.moveable = true;
                _remainingTime = turnTime;
            }
            else
            {
                _playerCharacter.moveable = false;
                if (_enemyCharacters[_turn - 1].isDead)
                {
                    ChangeTurn();
                }
                else
                {
                    _enemyCharacters[_turn - 1].TickBuffs();
                    yield return StartCoroutine(_enemyCharacters[_turn - 1].MakeMove());
                }
            }
        }

    }
}
