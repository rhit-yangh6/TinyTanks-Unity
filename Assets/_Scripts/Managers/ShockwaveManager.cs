using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Scripts.Managers
{
    public class ShockwaveManager: MonoBehaviour
    {
        [SerializeField] private float shockwaveTime = 0.75f;

        private Coroutine _shockwaveCoroutine;

        private Material _material;
        
        private static readonly int WaveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

        private void Awake()
        {
            _material = GetComponent<SpriteRenderer>().material;
        }

        public void CallShockwave(float waveTime, float endPos)
        {
            shockwaveTime = waveTime;
            _shockwaveCoroutine = StartCoroutine(ShockwaveAction(-0.1f, endPos));
        }

        private IEnumerator ShockwaveAction(float startPos, float endPos)
        {
            _material.SetFloat(WaveDistanceFromCenter, startPos);

            float lerpedAmount = 0f;
            float elapsedTime = 0f;
            while (elapsedTime < shockwaveTime)
            {
                elapsedTime += Time.deltaTime;

                lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime / shockwaveTime));
                _material.SetFloat(WaveDistanceFromCenter, lerpedAmount);

                yield return null;
            }
        }
    }
}