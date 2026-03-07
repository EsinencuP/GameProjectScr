using UnityEngine;

public class TestMove : MonoBehaviour
{
    Rigidbody2D rb; // Ссылка на компонент Rigidbody2D, который отвечает за физическое поведение объекта

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D при старте игры
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal"); // Получаем значение горизонтального ввода (A/D или стрелки влево/вправо)
        rb.linearVelocity = new Vector2(move * 5f, rb.linearVelocity.y); // Устанавливаем горизонтальную скорость в зависимости от ввода, сохраняя вертикальную скорость
    }
}