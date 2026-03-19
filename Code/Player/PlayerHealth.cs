using UnityEngine; // Подключаем пространство имён Unity, чтобы использовать MonoBehaviour, Debug, Time и другие классы движка

public class PlayerHealth : MonoBehaviour // Класс, который отвечает за здоровье игрока
{
    [Header("Health Settings")] // Заголовок секции настроек здоровья в инспекторе Unity
    [SerializeField] private int maxHealth = 5; // Максимальное количество здоровья игрока
    [SerializeField] private float damageCooldown = 1f; // Время неуязвимости между получениями урона

    private int currentHealth; // Текущее количество здоровья игрока
    private float lastDamageTime = -999f; // Время последнего получения урона; большое отрицательное значение нужно, чтобы первый удар прошёл сразу
    private bool isDead; // Флаг, который показывает, умер ли уже игрок

    public int CurrentHealth => currentHealth; // Свойство только для чтения, чтобы другие скрипты могли получить текущее здоровье
    public int MaxHealth => maxHealth; // Свойство только для чтения, чтобы можно было получить максимум здоровья
    public bool IsDead => isDead; // Свойство только для чтения, чтобы другие скрипты могли проверить, жив ли игрок

    private void Awake() // Awake вызывается раньше Start и удобен для базовой инициализации
    {
        currentHealth = maxHealth; // При запуске устанавливаем текущее здоровье равным максимальному
        Debug.Log("HP: " + currentHealth); // Выводим стартовое здоровье в консоль
    }

    public bool TakeDamage(int damage) // Метод получения урона; возвращает true, если урон реально был нанесён
    {
        if (isDead) // Если игрок уже мёртв
        {
            return false; // Ничего не делаем и сообщаем, что урон не применён
        }

        if (damage <= 0) // Если передали нулевой или отрицательный урон
        {
            return false; // Просто выходим, потому что такой урон применять не нужно
        }

        if (Time.time < lastDamageTime + damageCooldown) // Проверяем, не находится ли игрок ещё в окне неуязвимости
        {
            return false; // Если кулдаун урона не закончился, урон не наносим
        }

        currentHealth -= damage; // Уменьшаем текущее здоровье на величину урона
        currentHealth = Mathf.Max(currentHealth, 0); // Страхуемся и не даём здоровью уйти ниже нуля

        lastDamageTime = Time.time; // Запоминаем время последнего успешного получения урона

        Debug.Log("HP: " + currentHealth); // Выводим новое значение здоровья в консоль

        if (currentHealth <= 0) // Если здоровье закончилось
        {
            Die(); // Вызываем метод смерти
        }

        return true; // Сообщаем, что урон был успешно нанесён
    }

    public void Heal(int amount) // Метод восстановления здоровья
    {
        if (isDead) // Если игрок уже мёртв
        {
            return; // Не лечим мёртвого объекта
        }

        if (amount <= 0) // Если количество лечения некорректно
        {
            return; // Выходим без изменений
        }

        currentHealth += amount; // Добавляем указанное количество здоровья
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ограничиваем здоровье сверху максимальным значением

        Debug.Log("HP: " + currentHealth); // Выводим текущее здоровье после лечения
    }

    public int GetHealth() // Метод для совместимости с твоим старым кодом
    {
        return currentHealth; // Возвращаем текущее здоровье
    }

    private void Die() // Метод смерти игрока
    {
        if (isDead) // Если метод смерти уже вызывался ранее
        {
            return; // Повторно ничего не делаем
        }

        isDead = true; // Помечаем игрока как мёртвого
        Debug.Log("Игрок умер"); // Выводим сообщение о смерти в консоль

        Destroy(gameObject); // Удаляем объект игрока со сцены
    }
}