using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    public float maxOxygen = 100f; // максимум кислорода
    public float oxygenDrainRate = 5f; // скорость расхода в секунду

    private float currentOxygen;

    private PlayerHealth playerHealth;

    void Start()
    {
        currentOxygen = maxOxygen;
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        DrainOxygen();
    }

    void DrainOxygen()
    {
        if (currentOxygen > 0)
        {
            currentOxygen -= oxygenDrainRate * Time.deltaTime;
        }
        else
        {
            currentOxygen = 0;

            // если кислород закончился — наносим урон
            playerHealth.TakeDamage(1);
        }
    }

    public void AddOxygen(float amount)
    {
        currentOxygen += amount;

        if (currentOxygen > maxOxygen)
            currentOxygen = maxOxygen;
    }

    public float GetOxygen()
    {
        return currentOxygen;
    }

    public float GetMaxOxygen()
    {
        return maxOxygen;
    }
}