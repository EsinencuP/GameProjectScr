using UnityEngine; // Подключаем пространство имён Unity, чтобы использовать MonoBehaviour, Rigidbody2D, Input и другие классы движка

[RequireComponent(typeof(Rigidbody2D))] // Требуем, чтобы на объекте обязательно был Rigidbody2D, иначе скрипт работать не будет корректно
public class movePlayer : MonoBehaviour // Основной класс контроллера игрока для 2D платформера
{
    [Header("Movement")] // Заголовок секции настроек движения в инспекторе Unity
    [SerializeField] private float moveSpeed = 5f; // Скорость горизонтального перемещения игрока

    [Header("Jump")] // Заголовок секции настроек прыжка
    [SerializeField] private float jumpForce = 7f; // Сила первого прыжка
    [SerializeField] private int maxJumps = 2; // Максимальное количество прыжков, например 2 = двойной прыжок
    [SerializeField] private float secondJumpMultiplier = 0.8f; // Множитель силы для второго прыжка, чтобы он был немного слабее первого
    [SerializeField] private float coyoteTime = 0.12f; // Время после схода с платформы, когда прыжок ещё можно выполнить
    [SerializeField] private float jumpBufferTime = 0.12f; // Время буфера нажатия прыжка, чтобы нажатие чуть заранее тоже сработало

    [Header("Ground Check")] // Заголовок секции проверки земли
    [SerializeField] private Transform groundCheckPoint; // Точка, из которой мы будем проверять, касается ли игрок земли
    [SerializeField] private float groundCheckRadius = 0.2f; // Радиус проверки земли вокруг точки groundCheckPoint
    [SerializeField] private LayerMask groundLayer; // Слой земли, по которому определяется, стоит ли игрок на поверхности

    [Header("Dash")] // Заголовок секции настроек рывка
    [SerializeField] private float dashSpeed = 20f; // Скорость рывка
    [SerializeField] private float dashDuration = 0.15f; // Длительность рывка в секундах
    [SerializeField] private float dashCooldown = 1f; // Время перезарядки рывка после использования
    [SerializeField] private LayerMask wallLayer; // Слой стен, чтобы останавливать дэш при ударе о стену
    [SerializeField] private float wallCheckDistance = 0.6f; // Дистанция проверки стены перед игроком во время дэша

    [Header("Effects")] // Заголовок секции визуальных эффектов
    [SerializeField] private TrailRenderer trail; // След за игроком во время рывка
    [SerializeField] private ParticleSystem dashParticles; // Частицы при начале рывка

    private Rigidbody2D rb; // Ссылка на Rigidbody2D игрока для управления физикой
    private float defaultGravityScale; // Исходное значение gravityScale, чтобы после дэша вернуть именно его, а не жёстко 1

    private float horizontalInput; // Текущее значение горизонтального ввода: -1, 0 или 1
    private bool jumpPressed; // Флаг, что кнопка прыжка была нажата в текущем кадре
    private bool dashPressed; // Флаг, что кнопка дэша была нажата в текущем кадре

    private bool isGrounded; // Находится ли игрок на земле в текущий момент
    private bool wasGrounded; // Был ли игрок на земле на предыдущем кадре проверки
    private int jumpsUsed; // Сколько прыжков уже использовано с момента последнего касания земли

    private float coyoteTimer; // Таймер coyote time, позволяющий прыгнуть спустя короткое время после схода с края
    private float jumpBufferTimer; // Таймер буфера прыжка, чтобы прыжок срабатывал даже если кнопку нажали чуть раньше касания земли

    private bool isDashing; // Находится ли игрок сейчас в состоянии рывка
    private bool canDash = true; // Может ли игрок использовать рывок в данный момент
    private float dashTimer; // Таймер оставшегося времени текущего рывка
    private float dashCooldownTimer; // Таймер оставшегося времени до восстановления рывка
    private float dashDirection; // Направление рывка: -1 влево или 1 вправо

    private bool facingRight = true; // Направление взгляда игрока: true = вправо, false = влево

    private void Awake() // Метод вызывается при инициализации объекта до Start
    {
        rb = GetComponent<Rigidbody2D>(); // Получаем Rigidbody2D с этого же объекта и сохраняем ссылку
        defaultGravityScale = rb.gravityScale; // Запоминаем исходную гравитацию, чтобы потом корректно восстановить после дэша
    }

    private void Update() // Update вызывается каждый кадр и лучше подходит для чтения ввода
    {
        ReadInput(); // Считываем нажатия клавиш и осей управления
        UpdateGroundState(); // Проверяем, стоит ли игрок на земле
        UpdateTimers(); // Обновляем все таймеры: буфер прыжка, coyote time и кулдаун дэша
        HandleFlip(); // Разворачиваем игрока в сторону движения
    }

    private void FixedUpdate() // FixedUpdate вызывается с постоянным шагом времени и лучше подходит для физики
    {
        if (isDashing) // Если сейчас активен рывок
        {
            HandleDash(); // Обрабатываем логику рывка отдельно
            return; // Выходим, чтобы обычное движение и прыжок не вмешивались во время дэша
        }

        HandleMovement(); // Выполняем обычное горизонтальное движение
        HandleJump(); // Обрабатываем прыжок
        HandleDashStart(); // Проверяем, нужно ли начать дэш
    }

    private void ReadInput() // Метод считывает ввод игрока
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // Получаем горизонтальный ввод: -1 влево, 1 вправо, 0 если кнопки не нажаты
        jumpPressed = Input.GetKeyDown(KeyCode.Space); // Проверяем, была ли нажата кнопка прыжка в этом кадре
        dashPressed = Input.GetKeyDown(KeyCode.LeftShift); // Проверяем, была ли нажата кнопка рывка в этом кадре

        if (jumpPressed) // Если игрок нажал прыжок
        {
            jumpBufferTimer = jumpBufferTime; // Запускаем буфер прыжка, чтобы команда не потерялась
        }
    }

    private void UpdateGroundState() // Метод обновляет состояние нахождения на земле
    {
        wasGrounded = isGrounded; // Сохраняем предыдущее состояние земли, чтобы отследить момент приземления

        if (groundCheckPoint != null) // Проверяем, назначена ли точка проверки земли в инспекторе
        {
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer); // Проверяем, пересекается ли круг у ног игрока со слоем земли
        }
        else // Если точка проверки земли не назначена
        {
            isGrounded = false; // Считаем, что игрок не на земле, чтобы избежать ложной логики
        }

        if (isGrounded) // Если игрок сейчас стоит на земле
        {
            coyoteTimer = coyoteTime; // Обновляем таймер coyote time, чтобы прыжок был доступен сразу после схода с края
            if (!wasGrounded) // Если игрок только что приземлился в этом кадре
            {
                jumpsUsed = 0; // Сбрасываем количество использованных прыжков
            }
        }
    }

    private void UpdateTimers() // Метод уменьшает все временные счётчики
    {
        if (!isGrounded) // Если игрок не на земле
        {
            coyoteTimer -= Time.deltaTime; // Постепенно уменьшаем coyote time
        }

        if (jumpBufferTimer > 0f) // Если буфер прыжка ещё активен
        {
            jumpBufferTimer -= Time.deltaTime; // Уменьшаем таймер буфера прыжка
        }

        if (!canDash) // Если дэш сейчас на перезарядке
        {
            dashCooldownTimer -= Time.deltaTime; // Уменьшаем таймер кулдауна дэша
            if (dashCooldownTimer <= 0f) // Если кулдаун закончился
            {
                canDash = true; // Снова разрешаем использование дэша
            }
        }
    }

    private void HandleMovement() // Метод отвечает за обычное горизонтальное перемещение
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y); // Меняем только горизонтальную скорость, оставляя вертикальную без изменений
    }

    private void HandleJump() // Метод отвечает за логику прыжков
    {
        if (jumpBufferTimer <= 0f) // Если буфер прыжка уже истёк и команды на прыжок нет
        {
            return; // Выходим, так как прыгать не нужно
        }

        bool canUseGroundJump = coyoteTimer > 0f && jumpsUsed == 0; // Проверяем, доступен ли первый прыжок с земли или в рамках coyote time
        bool canUseAirJump = jumpsUsed > 0 && jumpsUsed < maxJumps; // Проверяем, доступен ли дополнительный прыжок в воздухе

        if (!canUseGroundJump && !canUseAirJump) // Если ни один вариант прыжка сейчас недоступен
        {
            return; // Выходим, прыжок выполнить нельзя
        }

        float currentJumpForce = jumpForce; // По умолчанию используем силу основного прыжка

        if (jumpsUsed > 0) // Если это не первый прыжок, а дополнительный
        {
            currentJumpForce *= secondJumpMultiplier; // Ослабляем силу второго прыжка множителем
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Сначала обнуляем вертикальную скорость, чтобы прыжок был одинаково стабильным
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpForce); // Устанавливаем новую вертикальную скорость для прыжка

        jumpsUsed++; // Увеличиваем счётчик использованных прыжков
        jumpBufferTimer = 0f; // Сбрасываем буфер прыжка, чтобы он не сработал повторно
        coyoteTimer = 0f; // Сбрасываем coyote time после прыжка, чтобы первый прыжок нельзя было использовать повторно
    }

    private void HandleDashStart() // Метод проверяет, нужно ли начать рывок
    {
        if (!dashPressed) // Если кнопка дэша не нажата
        {
            return; // Выходим, начинать дэш не нужно
        }

        if (!canDash) // Если дэш сейчас недоступен из-за кулдауна
        {
            return; // Выходим, дэш использовать нельзя
        }

        dashDirection = horizontalInput; // Пытаемся взять направление дэша из текущего ввода игрока

        if (dashDirection == 0f) // Если игрок не нажимает влево или вправо
        {
            dashDirection = facingRight ? 1f : -1f; // Используем направление, в которое смотрит персонаж
        }

        StartDash(); // Запускаем рывок
    }

    private void StartDash() // Метод запускает состояние рывка
    {
        isDashing = true; // Помечаем, что игрок начал дэш
        canDash = false; // Запрещаем новый дэш до окончания кулдауна
        dashTimer = dashDuration; // Устанавливаем таймер текущего рывка
        dashCooldownTimer = dashCooldown; // Запускаем таймер перезарядки дэша

        rb.gravityScale = 0f; // На время рывка отключаем влияние гравитации
        rb.linearVelocity = Vector2.zero; // Обнуляем текущую скорость, чтобы рывок начинался предсказуемо

        if (trail != null) // Если назначен TrailRenderer
        {
            trail.Clear(); // Очищаем старый след, чтобы новый рывок выглядел аккуратно
            trail.emitting = true; // Включаем отрисовку следа
        }

        if (dashParticles != null) // Если назначена система частиц
        {
            dashParticles.Play(); // Проигрываем эффект частиц в начале рывка
        }
    }

    private void HandleDash() // Метод выполняет сам рывок каждый физический тик
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * dashDirection, wallCheckDistance, wallLayer); // Пускаем луч в сторону рывка, чтобы заранее проверить стену

        if (hit.collider != null) // Если луч попал в стену
        {
            StopDash(); // Сразу останавливаем рывок, чтобы не врезаться и не залипнуть
            return; // Выходим из метода
        }

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f); // Во время рывка задаём фиксированную скорость по X и отключаем движение по Y
        dashTimer -= Time.fixedDeltaTime; // Уменьшаем оставшееся время рывка на один физический шаг

        if (dashTimer <= 0f) // Если время рывка истекло
        {
            StopDash(); // Завершаем рывок
        }
    }

    private void StopDash() // Метод завершает рывок и возвращает обычное состояние
    {
        isDashing = false; // Снимаем флаг активного дэша
        rb.gravityScale = defaultGravityScale; // Возвращаем исходное значение гравитации, которое было до дэша
        rb.linearVelocity = Vector2.zero; // Останавливаем остаточную скорость рывка, чтобы поведение было контролируемым

        if (trail != null) // Если след был назначен
        {
            trail.emitting = false; // Отключаем отрисовку следа после окончания рывка
        }
    }

    private void HandleFlip() // Метод отвечает за разворот персонажа в сторону движения
    {
        if (horizontalInput > 0f && !facingRight) // Если игрок движется вправо, но персонаж смотрит влево
        {
            Flip(true); // Разворачиваем персонажа вправо
        }
        else if (horizontalInput < 0f && facingRight) // Если игрок движется влево, но персонаж смотрит вправо
        {
            Flip(false); // Разворачиваем персонажа влево
        }
    }

    private void Flip(bool faceRight) // Метод непосредственно меняет направление персонажа
    {
        facingRight = faceRight; // Сохраняем новое направление взгляда

        Vector3 scale = transform.localScale; // Берём текущий масштаб объекта
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f); // Меняем знак по оси X в зависимости от направления взгляда
        transform.localScale = scale; // Применяем новый масштаб объекту
    }

    private void OnDrawGizmosSelected() // Метод рисует вспомогательные элементы в редакторе Unity при выделенном объекте
    {
        if (groundCheckPoint == null) // Если точка проверки земли не назначена
        {
            return; // Ничего не рисуем
        }

        Gizmos.color = Color.yellow; // Устанавливаем жёлтый цвет для визуализации зоны проверки земли
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius); // Рисуем окружность проверки земли в сцене Unity
    }
}