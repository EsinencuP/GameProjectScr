using UnityEngine;
using UnityEngine.UI;

public class OxygenUI : MonoBehaviour
{
    public PlayerOxygen player; //* Ссылка на компонент PlayerOxygen, который содержит информацию о кислороде игрока
    public Image oxygenBar; //* Ссылка на UI элемент, который будет отображать уровень кислорода

    void Update() //! В каждом кадре обновляем UI, чтобы он отображал текущий уровень кислорода игрока
    {
        float current = player.GetOxygen(); //? Получаем текущий уровень кислорода у игрока
        float max = player.GetMaxOxygen(); //? Получаем максимальный уровень кислорода у игрока

        oxygenBar.fillAmount = current / max; //? Устанавливаем заполнение UI элемента в зависимости от текущего уровня кислорода относительно максимального
    }
}