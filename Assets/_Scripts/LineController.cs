using System;
using System.Collections;
using UnityEngine;

namespace _Scripts
{
    public class LineController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        [SerializeField] private Texture[] _textures;

        private int animationStep;

        [SerializeField] private float fps = 30f;
        [SerializeField] private float lifeTime = 1f;
        private float fpsCounter;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            StartCoroutine(SelfDestroy());
        }

        private IEnumerator SelfDestroy()
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {
            fpsCounter += Time.deltaTime;
            if (fpsCounter >= 1f / fps)
            {
                animationStep++;
                if (animationStep == _textures.Length)
                {
                    animationStep = 0;
                }
                _lineRenderer.material.SetTexture(MainTex, _textures[animationStep]);
                fpsCounter = 0f;
            }
        }

        public void AssignPositions(Vector2 startPos, Vector2 endPos)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);
        }
    }
}
