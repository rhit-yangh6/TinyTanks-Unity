using System.Collections;
using _Scripts.Managers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Entities
{
    public class BasicEnemyController : EnemyController
    {
        public override IEnumerator MakeMove()
        {
            // Simple Enemy AI 0.0.1
            
            // Find and locate the target
            TargetObject = FindTarget();
            
            // Initial Wait
            yield return new WaitForSeconds(1);
            
            // Randomly get the direction of going
            var randomValue = Random.value;
            if (randomValue < approachTargetTendency)
            {
                XMovingDirection = TargetObject.transform.position.x < transform.position.x ? -1 : 1;
            }
            else
            {
                XMovingDirection = TargetObject.transform.position.x < transform.position.x ? 1 : -1;
            }

            // Flip if facing opposite direction
            if (XMovingDirection != FacingDirection)
            {
                Flip();
            }
            
            // Walk for fixed second(s)
            yield return new WaitForSeconds(1);
            // Disable Walking
            XMovingDirection = 0;

            // Aim and get the velocity
            Aim();
            // Aim for seconds
            yield return new WaitForSeconds(1);
            // Shoot projectile
            Shoot();
        }
        
        protected override void OnDeath()
        {
            base.OnDeath();
            SteamManager.IncrementStat(Constants.StatBasicEnemiesKilled);
        }
    }
}