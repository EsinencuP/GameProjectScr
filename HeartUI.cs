using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public PlayerHealth player;
    public GameObject[] hearts;

    void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < player.GetHealth()) // если у тебя есть свойство Health
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }
}