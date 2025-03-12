using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarDecay : MonoBehaviour
{
    public Slider slider;
    private float currentHealth;

    // Set the initial maximum health and start the coroutine to decrease health
    void Start()
    {
        SetMaxHealth(100f);  // Set the max health to 100
        currentHealth = 100f;  // Start with 100 health
        StartCoroutine(DecreaseHealthOverTime());  // Start the health decay coroutine
    }

    // Set the max health and initial slider value
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Decrease health by 0.1 every 0.01 seconds
    private IEnumerator DecreaseHealthOverTime()
    {
        while (currentHealth > 0f)
        {
            yield return new WaitForSeconds(0.005f);  // Wait for 0.01 seconds
            currentHealth -= 0.1f;  // Decrease health by 0.1
            SetHealth(currentHealth);  // Update the slider value

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;  // Ensure health doesn't go below 0
                SetHealth(currentHealth);  // Update slider value when health reaches 0
            }
        }
    }

    // Set the health value on the slider
    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
