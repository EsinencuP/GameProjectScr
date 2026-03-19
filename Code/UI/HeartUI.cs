using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public PlayerHealth player; //* Ссылка на компонент PlayerHealth, который содержит информацию о здоровье игрока
    public GameObject[] hearts; //* Массив объектов, представляющих сердца в UI

    void Update() //! Обновляем отображение сердец в UI в зависимости от текущего здоровья игрока
    {
        for (int i = 0; i < hearts.Length; i++) //? Проходим по каждому объекту сердца в массиве
        {
            if (i < player.GetHealth()) //? Если индекс сердца меньше текущего здоровья игрока, отображаем его
            {
                hearts[i].SetActive(true); //? Активируем объект сердца, чтобы он был видимым в UI
            }
            else
            {
                hearts[i].SetActive(false); //  ? Деактивируем объект сердца, чтобы он не был видимым в UI
            }
        }
    }
}