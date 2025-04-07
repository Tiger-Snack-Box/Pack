using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarDecay : MonoBehaviour
{
    public RectTransform healthBar; // Assign in Inspector
    public float relativeOffset = 0.05f; // 5% down from the top
    public Slider slider;
    private float currentHealth;

    // Set the initial maximum health and start the coroutine to increase health
    void Start()
    {
        AdjustHealthBarPosition();
        SetMaxHealth(100f);  // Set the max health to 100
        currentHealth = 0f;  // Start with 0 health
        StartCoroutine(IncreaseHealthOverTime());  // Start the health increase coroutine
    }

    void AdjustHealthBarPosition()
    {
        float screenHeight = Screen.height;
        float yOffset = screenHeight * relativeOffset;

        healthBar.anchoredPosition = new Vector2(healthBar.anchoredPosition.x, -yOffset);
    }

    // Set the max health and initial slider value
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Increase health by 0.1 every 0.01 seconds
    private IEnumerator IncreaseHealthOverTime()
    {
        while (currentHealth < slider.maxValue)
        {
            yield return new WaitForSeconds(0.001f);  // Wait for 0.01 seconds
            currentHealth += 0.1f;  // Increase health by 0.1
            SetHealth(currentHealth);  // Update the slider value

            if (currentHealth >= slider.maxValue)
            {
                currentHealth = slider.maxValue;  // Ensure health doesn't exceed max
                SetHealth(currentHealth);  // Update slider value when health reaches max
            }
        }
    }

    // Set the health value on the slider
    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
