using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int damage = 1; //* Количество урона, наносимого ловушкой

    private PlayerHealth playerInside; //* Ссылка на компонент PlayerHealth игрока, находящегося внутри ловушки

    void OnTriggerEnter2D(Collider2D collision) //! Проверяем, когда игрок входит в ловушку
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>(); //? Получаем компонент PlayerHealth игрока, который вошел в ловушку

        if (player != null) //? Если игрок имеет компонент PlayerHealth, сохраняем ссылку на него
        {
            playerInside = player; //? Сохраняем ссылку на игрока, находящегося внутри ловушки
        }
    }

    void OnTriggerExit2D(Collider2D collision) //! Проверяем, когда игрок покидает ловушку
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>(); //? Получаем компонент PlayerHealth игрока, который покинул ловушку

        if (player != null) //? Если игрок имеет компонент PlayerHealth и это тот же игрок, который находился внутри ловушки, сбрасываем ссылку
        {
            playerInside = null; //? Сбрасываем ссылку на игрока, так как он покинул ловушку
        }
    }

    void Update() //! Наносим урон игроку, если он находится внутри ловушки
    {
        if (playerInside != null) //? Если игрок находится внутри ловушки, наносим ему урон
        {
            playerInside.TakeDamage(damage); //? Вызываем метод TakeDamage у игрока, передавая количество урона
        }
    }
}