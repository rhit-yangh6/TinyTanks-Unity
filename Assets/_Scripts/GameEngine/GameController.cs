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
    public class GameController : AbstractGameController
    {
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
