using System;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Projectiles
{
    public class DiceProjectile : MonoBehaviour, IProjectile
    {
        private static float _radius, _maxMagnitude, _damage, _unitDiceDamage;
        private static int _steps;
        public GameObject explosionFX;
        public GameObject topDownDisplay;

        private int _diceResult = 0;
        private void OnCollisionEnter2D(Collision2D col)
        {
            
            if (col.gameObject.CompareTag("DangerZone"))
            {
                Destroy(gameObject);
                OnFinish();
            }
            else
            {
                Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0;


                _diceResult = HandleDiceResult();
                DisplayTownDownResult(_diceResult);
                Invoke(nameof(Detonate), 2.1f);
            }
        }

        private int HandleDiceResult()
        {
            return Random.Range(1, 6);
        }
        
        public void Detonate()
        {
            Vector2 pos = transform.position;
            
            DamageHandler.i.HandleCircularDamage(pos, _radius, _damage + _diceResult * _unitDiceDamage);

            TerrainDestroyer.Instance.DestroyTerrain(pos, _radius);
        
            SpawnExplosionFX();
            DoCameraShake();
        
            Destroy(gameObject);
            OnFinish();
        }

        public void SpawnExplosionFX()
        {
            GameObject insExpl = Instantiate(explosionFX, transform.position, quaternion.identity);
            insExpl.transform.localScale *= _radius;
            Destroy(insExpl, .2f);
        }

        public void DoCameraShake()
        {
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.1f;
        }

        public void OnFinish()
        {
            GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
            gameController.ChangeTurn();
        }

        public void SetParameters(float damage, float radius, float maxMagnitude, int steps, ExtraWeaponTerm[] extraWeaponTerms)
        {
            _damage = damage;
            _radius = radius;
            _maxMagnitude = maxMagnitude;
            _steps = steps;
            
            _unitDiceDamage = Array.Find(extraWeaponTerms, ewt => ewt.term == "unitDiceDamage").value;
        }

        public float getMaxMagnitude()
        {
            return _maxMagnitude;
        }
        public int getSteps()
        {
            return _steps;
        }

        private void DisplayTownDownResult(int number)
        {
            Vector3 cameraCenterPos = Camera.main.transform.position;
            GameObject insTopdown = Instantiate(topDownDisplay, 
                new Vector3(cameraCenterPos.x, cameraCenterPos.y, 0), 
                quaternion.identity);
            insTopdown.GetComponent<SpriteRenderer>().sprite = GameAssets.i.diceNumbers[number - 1];
            insTopdown.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-18.0f, 18.0f)));
            Destroy(insTopdown, 1.9f);
        }
    }
}
