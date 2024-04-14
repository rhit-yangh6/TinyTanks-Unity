using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class PlayerHealthBarBehavior : HealthBarBehavior
    {
        [SerializeField] private Slider playerSlider;
        public TextMeshProUGUI textMesh;

        protected override Slider Slider => playerSlider;

        public override void SetHealth(float health, float maxHealth)
        {
            Slider.maxValue = maxHealth;
            Slider.value = health;
            textMesh.text = health + " / " + maxHealth;
        }

        protected override void Update() { }
    }
}
