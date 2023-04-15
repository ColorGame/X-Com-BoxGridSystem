using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour // Система очереди (ходов) . Обрабатывает логику ходов
{
    public static TurnSystem Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                              // instance - экземпляр, У нас будет один экземпляр UnitActionSystem можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    public event EventHandler OnTurnChanged; // Ход Изменен (когда меняется ход мы запустим событие Event)

    private int _turnNumber = 1; // Номер очереди (хода)
    private bool _isPlayerTurn = true; // Ход игрока, по умолчанию true т.к. он ходит первым


    private void Awake() //Для избежания ошибок Awake Лучше использовать только для инициализации и настроийки объектов
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one TurnSystem!(Там больше, чем один TurnSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр TurnSystem прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;
    }

    public void NextTurn() // Следующая очередь (ход) ВЫЗЫВАЕМ ПРИ НАЖАТИИ НА Button "END TURN"
    {
        _turnNumber++; // Увеличиваем число на еденицу
        _isPlayerTurn = !_isPlayerTurn; // перевернем ход игрока. Каждый второй ход будет активна для игрока

        OnTurnChanged?.Invoke(this, EventArgs.Empty); // "?"- проверяем что !=0. Invoke вызвать (this-ссылка на объект который запускает событие "отправитель" а класс TurnSystemUI и UnitActionSystemUI, Unit будет его прослушивать "обрабатывать")
    }

    public int GetTurnNumber() //Вернуть Номер хода
    {
        return _turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return _isPlayerTurn;
    }
}
