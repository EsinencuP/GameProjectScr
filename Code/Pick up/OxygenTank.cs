using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public float oxygenAmount = 30f; //* Количество кислорода, которое добавляет этот танк

    void OnTriggerEnter(Collider other) //! Когда игрок входит в зону действия танка
    {
        PlayerOxygen player = other.GetComponent<PlayerOxygen>(); //? Пытаемся получить компонент PlayerOxygen у объекта, который вошел в триггер

        if (player != null) //? Если компонент найден, значит это игрок, и мы можем добавить ему кислород
        {
            player.AddOxygen(oxygenAmount); //?  Добавляем кислород игроку
            Destroy(gameObject); //? Уничтожаем танк после использования
        }
    }
}