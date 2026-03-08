using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int damage = 1;

    private PlayerHealth playerInside;

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();

        if (player != null)
        {
            playerInside = player;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();

        if (player != null)
        {
            playerInside = null;
        }
    }

    void Update()
    {
        if (playerInside != null)
        {
            playerInside.TakeDamage(damage);
        }
    }
}