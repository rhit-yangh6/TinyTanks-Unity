using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class HealthBarBehavior : MonoBehaviour
    {

        public Slider Slider;
        public Color Low;
        public Color High;
        public Vector3 Offset;

        public void SetHealth(float health, float maxHealth)
        {
            // Slider.gameObject.SetActive(health < maxHealth);
            Slider.maxValue = maxHealth;
            Slider.value = health;
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Low, High, Slider.normalizedValue);
        }

        void Update()
        {
            Slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + Offset);
        }
    }
}
