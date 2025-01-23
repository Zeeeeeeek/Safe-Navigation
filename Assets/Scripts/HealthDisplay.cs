using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public int health = 10;
    public TextMeshProUGUI healthText;

    public void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        healthText.text = $"{health}";
    }

    public void DecreaseHealth()
    {
        if (health <= 0) return;
        health--;
        UpdateText();
    }

}