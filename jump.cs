using UnityEngine;

public class PlayerJump : MonoBehaviour // Это скрипт для прыжков игрока. Он проверяет, находится ли игрок на земле, и позволяет ему прыгать при нажатии пробела.
{
    public float jumpForce = 7f; // Сила прыжка

    private Rigidbody2D rb; // Ссылка на компонент Rigidbody2D
    private bool isGrounded; // Флаг, указывающий, находится ли игрок на земле

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Получаем компонент Rigidbody2D при старте игры
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // Проверяем, нажата ли клавиша пробела и находится ли игрок на земле
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Устанавливаем вертикальную скорость для прыжка, сохраняя горизонтальную скорость
        }
    }

    void OnCollisionEnter2D(Collision2D collision) // Этот метод вызывается при столкновении с другим объектом
    {
        if (collision.gameObject.CompareTag("Ground")) // Проверяем, столкнулись ли мы с объектом, который имеет тег "Ground"
        {
            isGrounded = true; //  Если да, то устанавливаем флаг isGrounded в true, что позволяет игроку прыгать снова
        }
    }

    void OnCollisionExit2D(Collision2D collision) // Этот метод вызывается, когда мы перестаем сталкиваться с другим объектом
    {
        if (collision.gameObject.CompareTag("Ground")) // Проверяем, перестали ли мы сталкиваться с объектом, который имеет тег "Ground"
        {
            isGrounded = false; // Если да, то устанавливаем флаг isGrounded в false, что не позволяет игроку прыгать, пока он не вернется на землю
        }
    }
}