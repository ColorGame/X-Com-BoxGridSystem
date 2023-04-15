using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour // Этот клас будет отвечать за позицию на сетке и очки действий, получение урона
{

    private const int ACTION_POINTS_MAX = 3; //НАДО НАСТРОИТЬ//

    // для // РЕШЕНИЕ 2 //в UnitActionSystemUI
    public static event EventHandler OnAnyActionPointsChanged;  // static - обозначает что event будет существовать для всего класса не зависимо от того скольго у нас созданно Юнитов. Поэтому для прослушивания этого события слушателю не нужна ссылка на какую-либо конкретную единицу, они могут получить доступ к событию через класс, который затем запускает одно и то же событие для каждой единицы. 
                                                                // Мы запустим событие Event при изменении очков действий у ЛЮБОГО(Any) юнита а не только у выбранного.

    public static event EventHandler OnAnyUnitSpawned; // Событие Любой Рожденный(созданный) Юнит
    public static event EventHandler OnAnyUnitDead; // Событие Любой Мертвый Юнит


    [SerializeField] private bool _isEnemy; //В инспекторе у префаба Врага поставить галочку

    // Частные случаи
    private GridPosition _gridPosition;
    private HealthSystem _healthSystem; 
    private BaseAction[] _baseActionsArray; // Массив базовых действий // Будем использовать при создании кнопок
    private int _actionPoints = ACTION_POINTS_MAX; // Очки действия

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>(); 
        _baseActionsArray = GetComponents<BaseAction>(); // _moveAction и _spinAction также будут храниться внутри этого массива
    }

    private void Start()
    {
        // Когда Unit запускается он вычисляет свое положение в сетке и добовляет себя к GridObject(объектам сетки) в данной ячейки
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Получим позицию юнита на сетке. Для этого преобразуем мировую позицию ЮНИТА в позицию на СЕТКЕ
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this); // Зайдем в LevelGrid получим доступ к статическому экземпляру и вызовим AddUnitAtGridPosition

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // подписываемся на Event // Будет выполняться (сброс очков действий) каждый раз когда изменен номер хода.

        _healthSystem.OnDead += HealthSystem_OnDead; // подписываемся на Event. Будет выполняться при смерти юнита

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty); // Запустим событие Любой Рожденный(созданный) Юнит. Событие статичное поэтому будет выполняться для всех созданных Юнитов
    }
       

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //Получим новую позицию юнита на сетке.
        if (newGridPosition != _gridPosition) // Если новая позиция на сетке отличается от последней то ...
        {
            // Изменем положение юнита на сетке
            GridPosition oldGridPosition = _gridPosition; // Сохраним старую позицию что бы передать в event
            _gridPosition = newGridPosition; //Обновим позицию - Новая позиция становиться текущей
           
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition); //в UnitMovedGridPosition запускаем Событие. Поэтому эту строку поместим в КОНЦЕЦ . Иначе мы запускаем событие сетка обнавляется а юнит еще не перемещен
        }
    }

    public T GetAction<T>() where T : BaseAction //Фунцкция для получения любого типа базового действия // Создадим метод с GENERICS и ограничим типами в  BaseAction
    {
        foreach (BaseAction baseAction in _baseActionsArray) // Переберем Массив базовых действий
        {
            if (baseAction is T) // если T совподает с каким нибудь baseAction то ...
            {
                return baseAction as T; // Вернем это базовое действие КАК Т // (T)baseAction; - еще один метод записи
            }
        }
        return null; // Если нет совпадений то вернем ноль
    }
        
    public GridPosition GetGridPosition() // Получить сеточную позицию
    {
        return _gridPosition;
    }


    public Vector3 GetWorldPosition() // Получить мировую позицию
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionsArray() // Получить Массив базовых действий
    {
        return _baseActionsArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) // ПОПРОБУЕМ Потратить Очки Действия, Чтобы Выполнить Действие // Этот метод выполняет вместе два нижних метода
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) //мы МОЖЕМ Потратить Очки Действия, Чтобы Выполнить Действие ? 
    {
        if (_actionPoints >= baseAction.GetActionPointCost()) // Если очков действия хватает то...
        {
            return true; // Можем выполнить действие
        }
        else
        {
            return false; // Увы очков не хватает
        }

        /*// Альтернативная запись кода выше
        return _actionPoints >= baseAction.GetActionPointCost();*/
    }

    private void SpendActionPoints(int amount) //Потратить очки действий (amount- количество которое надо потратить)
    {
        _actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // запускаем событие ПОСЛЕ обнавления очков действий.(для // РЕШЕНИЕ // 2 //в UnitActionSystemUI)
    }

    public int GetActionPoints() // Получить очки действия
    {
        return _actionPoints;
    }


    public void TurnSystem_OnTurnChanged(object sender, EventArgs empty) // Сбросим очки действий до максимальных
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || // Если это враг И его очередь (НЕ очередь игрока) ИЛИ это НЕ враг(игрок) и очередь игрока то...
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            _actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // запускаем событие ПОСЛЕ обнавления очков действий.(для // РЕШЕНИЕ // 2 //в UnitActionSystemUI)
        }
    }

    public bool IsEnemy() // Раскроем поле
    {
        return _isEnemy;
    }

    public void Damage(int damageAmount) // Урон. Разные оружия будут наносить разный урон поэтому в аргументе определим damageAmount
    {
        _healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) // Будет выполняться при смерти юнита

    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this); // Удалим из сеточной позиции умершего юнита

        Destroy(gameObject); // Уничтожим игровой объект к которому прикриплен данный скрипт

        // Вслучае смерти активного Юнита надо предать ход следующему юниту        

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty); // Запустим событие Любой Мертвый Юнит. Событие статичное поэтому будет выполняться для любого мертвого Юнита      
    }

    public float GetHealthNormalized() // Раскроем для чтения
    {
        return _healthSystem.GetHealthNormalized();
    }

    public int GetHealth() // Раскроем для чтения
    {
        return _healthSystem.GetHealth();
    }
    
    public int GetHealthMax() // Раскроем для чтения
    {
        return _healthSystem.GetHealthMax();
    }

    public bool IsDead()
    {
        return _healthSystem.IsDead();
    }
}
