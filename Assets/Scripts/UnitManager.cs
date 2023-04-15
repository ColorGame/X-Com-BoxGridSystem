using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// РЕШЕНИЕ //- НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА UnitManager и Unit , добавим в Project Settings/ Script Execution Order и поместим выполнение UnitManager выше Default Time), чтобы UnitManager запустился раньше чем сценарий Unit
// (что бы сначала запустить слушателя в UnitManager а потом запустить само событие в Unit)
// Иначе если скрипт Unit проснется раньше то юнит не добавиться в СПИСОК ЮНИТОВ т.к. тот еще не запустился
public class UnitManager : MonoBehaviour // Менеджер (администратор) Юнитов
{

    public static UnitManager Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                               // instance - экземпляр, У нас будет один экземпляр UnitActionSystem можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    public static event EventHandler OnAnyUnitDeadAndRemoveList; // Событие Любой Юнит Умер И Удален из Списка

    private List<Unit> _unitList;       // Список юнитов (ОБЩИЙ)
    private List<Unit> _friendlyUnitList;// Дружественный список юнитов
    private List<Unit> _enemyUnitList;  // Вражеский список юнитов


    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one UnitManager!(Там больше, чем один UnitManager!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр UnitManager прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;

        // Проведем инициализацию списков
        _unitList = new List<Unit>();
        _friendlyUnitList = new List<Unit>();
        _enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned; // Подпишемся на событие (Любой Рожденный(созданный) Юнит)  // ПРОБЛЕММА с порядком операций МЫ ПОДПИСЫВАЕМСЯ НА СОБЫТИЕ КОТОРОЕ ЗАПУСКАЕТСЯ ТОЖЕ В СТАРТЕ И КТО РАНЬШЕ ЭТО ВОПРОС  (РЕШЕНИЕ СМОТРИ В ШАПКЕ)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // Подпишемся на событие (Любой Мертвый Юнит)
    }

    private void Unit_OnAnyUnitSpawned(object sender, System.EventArgs e) // При рождении юнитов распределим их по спискам
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        //Debug.Log(unit + " spawner"); // Для теста

        _unitList.Add(unit); // Добавим Юнита в общий список

        if (unit.IsEnemy()) // Если отправитель Враг то ...
        {
            _enemyUnitList.Add(unit); // Добавим его в список Вражеских Юнитов
        }
        else// если нет
        {
            _friendlyUnitList.Add(unit); // Добавим его в список Дружественных Юнитов
        }

    }
    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - другая запись// Получим Юнита который является отправителем

        //Debug.Log(unit + " dead"); // Для теста

        _unitList.Remove(unit); // Удалить Юнита из общего списка

        if (unit.IsEnemy()) // Если отправитель Враг то ...
        {
            _enemyUnitList.Remove(unit); // Удалим его из списка Вражеских Юнитов
        }
        else// если нет
        {
            _friendlyUnitList.Remove(unit); // Удалим его из списка Дружественных Юнитов
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // Запустим событьие
    }

    public List<Unit> GetUnitList() // Откроем доступ для чтения
    {
        return _unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return _friendlyUnitList;
    }


    public List<Unit> GetEnemyUnitList()
    {
        return _enemyUnitList;
    }


}
