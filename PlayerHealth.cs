using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public float damageCooldown = 1f;

    private int currentHealth;
    private float lastDamageTime;
    

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
     if (Time.time < lastDamageTime + damageCooldown)
            return;

        currentHealth -= damage;

        Debug.Log("HP: " + currentHealth);
        lastDamageTime = Time.time;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("HP: " + currentHealth);
    }

    public int GetHealth()
{
    return currentHealth;
}

    void Die()
    {
        Debug.Log("Игрок умер");

        Destroy(gameObject);
    }
}