using System;
using _Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class PlayerFuelBarBehavior : MonoBehaviour
    {
        [SerializeField] private Slider fuelSlider;
        [SerializeField] public Color low;
        [SerializeField] public Color high;
        [SerializeField] public TextMeshProUGUI textMesh;

        private PlayerController _player;

        public void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        private void SetFuel(float fuel, float maxFuel)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = fuel;
            fuelSlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, fuelSlider.normalizedValue);
            textMesh.text = (int)fuel + " / " + maxFuel;
        }

        public void Update()
        {
            SetFuel(_player.fuel, _player.maxFuel);
        }

    }
}
