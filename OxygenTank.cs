using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public float oxygenAmount = 30f;

    void OnTriggerEnter(Collider other)
    {
        PlayerOxygen player = other.GetComponent<PlayerOxygen>();

        if (player != null)
        {
            player.AddOxygen(oxygenAmount);
            Destroy(gameObject);
        }
    }
}