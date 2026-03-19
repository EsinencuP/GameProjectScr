using UnityEngine; // Подключаем пространство имён Unity для работы со скриптами Unity

[RequireComponent(typeof(PlayerHealth))] // Требуем, чтобы на объекте обязательно был компонент PlayerHealth
public class PlayerOxygen : MonoBehaviour // Класс, который отвечает за систему кислорода у игрока
{
    [Header("Oxygen Settings")] // Заголовок секции настроек кислорода в инспекторе
    [SerializeField] private float maxOxygen = 100f; // Максимальный запас кислорода
    [SerializeField] private float oxygenDrainRate = 5f; // Скорость расхода кислорода в секунду
    [SerializeField] private int damageWhenEmpty = 1; // Урон, который получает игрок, когда кислород закончился

    private float currentOxygen; // Текущий запас кислорода
    private PlayerHealth playerHealth; // Ссылка на компонент здоровья игрока

    public float CurrentOxygen => currentOxygen; // Свойство только для чтения, чтобы другие скрипты могли получить текущий кислород
    public float MaxOxygen => maxOxygen; // Свойство только для чтения, чтобы можно было узнать максимальный кислород

    private void Awake() // Метод инициализации, который вызывается до Start
    {
        playerHealth = GetComponent<PlayerHealth>(); // Получаем ссылку на компонент здоровья, который находится на этом же объекте
        currentOxygen = maxOxygen; // При запуске заполняем кислород до максимума
    }

    private void Update() // Update вызывается каждый кадр
    {
        if (playerHealth == null) // Если по какой-то причине компонент здоровья не найден
        {
            return; // Ничего не делаем, чтобы избежать ошибки NullReference
        }

        if (playerHealth.IsDead) // Если игрок уже мёртв
        {
            return; // Останавливаем работу логики кислорода
        }

        DrainOxygen(); // Уменьшаем кислород или наносим урон, если он закончился
    }

    private void DrainOxygen() // Метод, который отвечает за расход кислорода
    {
        if (currentOxygen > 0f) // Если кислород ещё есть
        {
            currentOxygen -= oxygenDrainRate * Time.deltaTime; // Плавно уменьшаем кислород с учётом времени между кадрами
            currentOxygen = Mathf.Max(currentOxygen, 0f); // Не даём значению уйти ниже нуля
            return; // Выходим, потому что пока кислород ещё был, урон не нужен
        }

        playerHealth.TakeDamage(damageWhenEmpty); // Если кислород закончился, пытаемся нанести урон игроку
    }

    public void AddOxygen(float amount) // Метод для пополнения кислорода, например от баллона или бонуса
    {
        if (amount <= 0f) // Если передано некорректное значение пополнения
        {
            return; // Просто выходим
        }

        currentOxygen += amount; // Увеличиваем текущий запас кислорода
        currentOxygen = Mathf.Min(currentOxygen, maxOxygen); // Не даём превысить максимальное значение
    }

    public float GetOxygen() // Метод для совместимости со старым кодом
    {
        return currentOxygen; // Возвращаем текущее количество кислорода
    }

    public float GetMaxOxygen() // Метод для совместимости со старым кодом
    {
        return maxOxygen; // Возвращаем максимальное количество кислорода
    }
}