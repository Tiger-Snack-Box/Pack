using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarDecay : MonoBehaviour
{
    public RectTransform healthBar; // Assign in Inspector
    public float relativeOffset = 0.05f; // 5% down from the top
    public Slider slider;
    public float maxHealth = 100f;
    private float currentHealth;

    public float gameTime = 60f; // Game duration in seconds
    private float timer;

    public GameObject endGamePanel; // Assign your end game UI panel in the Inspector

    private bool gameEnded = false;

    void Start()
    {
        AdjustHealthBarPosition();
        SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        timer = gameTime;
        if (endGamePanel != null) endGamePanel.SetActive(false); // Hide at start
    }

    void Update()
    {
        if (gameEnded) return;

        // Timer logic
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            EndGame();
        }
    }

    void AdjustHealthBarPosition()
    {
        float screenHeight = Screen.height;
        float yOffset = screenHeight * relativeOffset;
        healthBar.anchoredPosition = new Vector2(healthBar.anchoredPosition.x, -yOffset);
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Call this method when the player takes damage
    public void TakeDamage(float amount)
    {
        if (gameEnded) return;

        currentHealth -= amount;
        if (currentHealth < 0f) currentHealth = 0f;
        SetHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            EndGame();
        }
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    void EndGame()
    {
        gameEnded = true;
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            Time.timeScale = 0; // Optional: pause the game
        }
        // Or load a scene:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("EndGameScene");
    }
}