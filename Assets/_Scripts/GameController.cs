using System;
using System.Collections;
using System.Globalization;
using _Scripts.Entities;
using _Scripts.Managers;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private float turnTime = 45f;
        
        [HideInInspector] public bool projectileShot;
        
        private GameObject _player;
        private PlayerController _playerCharacter;
        private EnemyController[] _enemyCharacters;
        private GameObject[] _enemies;
        private TextMeshProUGUI _timerText;
        private PauseMenu _pauseMenu;

        private int _turn = 0, _playerNum = 0;
        private float _remainingTime;
        private bool _isInterTurn;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _enemies = GameObject.FindGameObjectsWithTag("Enemy");
            _playerNum = _enemies.Length + 1;
            _enemyCharacters = new EnemyController[_enemies.Length];
            
            for (var i = 0; i < _enemies.Length; i++)
            {
                _enemyCharacters[i] = _enemies[i].GetComponent<EnemyController>();
            }
            _playerCharacter = _player.GetComponent<PlayerController>();

            _timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            _pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponent<PauseMenu>();
            
            // Register listeners
            EventBus.AddListener<int>(EventTypes.DamageDealt, PrintDamage);
            
            StartCoroutine(HandleMovements());
        }

        private void Update()
        {

            // If It's the player's turn and the player has not performed a shot
            if (_turn == 0 && !projectileShot)
            {
                _remainingTime -= Time.deltaTime;
                _timerText.text = Math.Round(_remainingTime).ToString(CultureInfo.InvariantCulture);
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
                _timerText.text = "Please Wait...";
            } 
            // Enemies turn
            else
            {
                _timerText.text = "Waiting for Opponent...";
                if (projectileShot)
                {
                    CheckAggressiveProjectiles();
                }
            }
            
        }

        private void PrintDamage(int damageAmount)
        {
            Debug.Log("Damage! " + damageAmount);
        }

        private void CheckAggressiveProjectiles()
        {
            var aggressiveProjectiles = GameObject.FindGameObjectsWithTag("AggressiveProjectile");
            if (aggressiveProjectiles.Length == 0 && !_isInterTurn)
            {
                _isInterTurn = true;
                Invoke(nameof(ChangeTurn), 1f);
            }
        }

        public void ChangeTurn()
        {
            if (PauseMenu.GameIsEnded) return;

            if (_playerCharacter.Health <= 0)
            {
                _pauseMenu.Lose();
                return;
            }
            
            if (IsAllEnemyDead())
            {
                _pauseMenu.Win();
                return;
            }

            projectileShot = false;
            _turn = (_turn + 1) % _playerNum;
            _isInterTurn = false;
            StartCoroutine(HandleMovements());
        }

        // Hitting the edge or dying in their turn
        public void EndTurnByCharacter(BuffableEntity be)
        {
            if (_playerCharacter.Equals(be))
            {
                if (_turn != 0) return;
                ChangeTurn();
                return;
            }

            var idx = Array.IndexOf(_enemyCharacters, (EnemyController)be);
            if (idx != _turn - 1) return;
            ChangeTurn();
        }

        private bool IsAllEnemyDead()
        {
            for ( int i = 0; i < _enemyCharacters.Length; i++ ) {
                if ( !_enemyCharacters[ i ].IsDead ) {
                    return false;
                }
            }
 
            return true;
        }

        private IEnumerator HandleMovements()
        {
            var turn = _turn;
            if (turn == 0)
            {
                _playerCharacter.TickBuffs();
                _playerCharacter.moveable = true;
                _remainingTime = turnTime;
            }
            else
            {
                _playerCharacter.moveable = false;
                if (_enemyCharacters[turn - 1].IsDead)
                {
                    ChangeTurn();
                }
                else
                {
                    _enemyCharacters[turn - 1].TickBuffs();
                    if (turn == _turn) // Not Skipped
                    {
                        yield return StartCoroutine(_enemyCharacters[turn - 1].MakeMove());
                    }
                }
            }
        }

    }
}
