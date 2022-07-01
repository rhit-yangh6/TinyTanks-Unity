using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarBehavior : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI textMesh;

    public void SetHealth(float health, float maxHealth)
    {
        Slider.maxValue = maxHealth;
        Slider.value = health;
        textMesh.text = health + " / " + maxHealth;
    }
}
