using UnityEngine;
using UnityEngine.UI;

namespace DemosShared
{
    [RequireComponent(typeof(Text))]
    public class FPSCounter : MonoBehaviour
    {
        private Text _text;
        private float _timer;
        private int _fps;

        private void Start()
        {
            _text = GetComponent<Text>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            _fps++;

            if (_timer >= 1)
            {
                _text.text = $"FPS = {_fps}";
                _timer = 0;
                _fps = 0;
            }
        }
    }
}