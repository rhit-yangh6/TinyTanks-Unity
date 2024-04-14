using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class HealthBarBehavior : MonoBehaviour
    {

        [SerializeField] private Slider slider;
        public Color low;
        public Color high;
        public Vector3 offset;

        protected virtual Slider Slider => slider;

        public virtual void SetHealth(float health, float maxHealth)
        {
            // Slider.gameObject.SetActive(health < maxHealth);
            Slider.maxValue = maxHealth;
            Slider.value = health;
            Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, slider.normalizedValue);
        }

        protected virtual void Update()
        {
            Slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
        }
    }
}
