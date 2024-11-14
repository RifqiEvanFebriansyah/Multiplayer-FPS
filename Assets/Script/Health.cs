using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI Elements")]
    public Slider healthBar;

    [Header("Damage Settings")]
    public int damageAmount = 10; // Damage yang diberikan saat terkena tembakan

    void Start()
    {
        // Set current health to max health at the start
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Method untuk menambah darah (untuk power-up)
    public void AddHealth(int healthAmount)
    {
        currentHealth += healthAmount;

        // Clamp health agar tidak melebihi maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();
    }

    // Method untuk menerima damage dari pemain lain
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Clamp health agar tidak kurang dari 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        // Cek jika health mencapai 0
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Update health bar UI
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    // Handle death
    private void Die()
    {
        Debug.Log("Player has died.");
        // Logika tambahan jika pemain mati, misalnya menonaktifkan pemain
    }
}
