using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА LevelGrid, добавим в Project Settings/ Script Execution Order и поместим выполнение LevelGrid выше Default Time, чтобы LevelGrid запустился РАНЬШЕ до того как ктонибудь совершит поиск пути ( В Start() мы запускаем класс Pathfinding - настроику поиска пути)

public class LevelGrid : MonoBehaviour // Основной скрипт который управляет СЕТКОЙ данного УРОВНЯ . Оснавная задача Присвоить или Получить определенного Юнита К заданной Позиции Сетки
{

    public static LevelGrid Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                             // instance - экземпляр, У нас будет один экземпляр LevelGrid можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition; //Запутим событие когда - Любой Юнит Перемещен в Сеточной позиции  // <OnAnyUnitMovedGridPositionEventArgs>- вариант передачи через событие нужные параметры

    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs // Расширим класс событий, чтобы в аргументе события передать юнита и сеточные позиции
    {
        public Unit unit;
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }


    [SerializeField] private Transform _gridDebugObjectPrefab; // Префаб отладки сетки //Передоваемый тип должен совподать с типом аргумента метода CreateDebugObject

    [SerializeField] private int _width = 10;     // Ширина
    [SerializeField] private int _height = 10;    // Высота
    [SerializeField] private float _cellSize = 2f;// Размер ячейки

    private GridSystem<GridObject> _gridSystem; // Сеточная система .В дженерик предаем тип GridObject

    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one LevelGrid!(Там больше, чем один LevelGrid!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр LevelGrid прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;

        _gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize,                               // ПОСТРОИМ СЕТКУ 10 на 10 и размером 2 еденицы и в каждой ячейки создадим объект типа GridObject
                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition)); //в четвертом параметре аргумента зададим функцию ананимно через лямбду => new GridObject(g, _gridPosition) И ПЕРЕДАДИМ ЕЕ ДЕЛЕГАТУ. (лямбда выражение можно вынести в отдельный метод)
        // _gridSystem.CreateDebugObject(_gridDebugObjectPrefab); // Создадим наш префаб в каждой ячейки // Закоментировал т.к. PathfindingGridDebugObject будет выполнять базовыедействия вместо _gridDebugObjectPrefab
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(_width, _height, _cellSize); // ПОСТРОИМ СЕТКУ УЗЛОВ ПОИСКА ПУТИ // УБЕДИМСЯ ЧТО ЭТОТ МЕТОД СТАРТУЕТ РАНЬШЕ до того как ктонибудь совершит поиск пути
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) // Добавить определенного Юнита К заданной Позиции Сетки
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        gridObject.AddUnit(unit); // Добавить юнита 
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) // Получить Список Юнитов В заданной Позиции Сетки
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        return gridObject.GetUnitList();// получим юнита
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) // Удаление юнита из заданной позиции сетки
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        gridObject.RemoveUnit(unit); // удалим юнита
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) // Юнит Перемещен в Сеточной позиции из позиции fromGridPosition в позицию toGridPosition
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit); // Удалим юнита из прошлой позиции сетки

        AddUnitAtGridPosition(toGridPosition, unit);  // Добавим юнита к следующей позиции сетки

        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs // создаем новый экземпляр класса OnAnyUnitMovedGridPositionEventArgs
        {
            unit = unit, 
            fromGridPosition = fromGridPosition, 
            toGridPosition = toGridPosition,

        }); // Запустим событие Любой Юнит Перемещен в Сеточной позиции ( в аргументе передадим Какой юнит Откуда и Куда)
    }

    // Что бы не раскрывать внутриние компоненты LevelGrid (и не делать публичным поле_gridSystem) но предоставить доступ к GridPosition сделаем СКВОЗНУЮ функцию для доступа к GridPosition
    /*public GridPosition GetGridPosition(Vector3 worldPosition)
    {
      return _gridSystem.GetGridPosition(worldPosition);
    }*/
    public GridPosition GetGridPosition(Vector3 worldPosition) => _gridSystem.GetGridPosition(worldPosition); // Сокращенная запись кода выше
    public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition); // Сквозная функция
    public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition); // Сквозная функция для получения доступа к IsValidGridPosition из _gridSystem
    public int GetWidth() => _gridSystem.GetWidth();
    public int GetHeight() => _gridSystem.GetHeight();
    public float GetCellSize() => _gridSystem.GetCellSize();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) // Есть ли какой нибудь юнит на этой сеточной позиции
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        return gridObject.HasAnyUnit();
    }
    public Unit GetUnitAtGridPosition(GridPosition gridPosition) // Получить Юнита в этой сеточной позиции
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        return gridObject.GetUnit();
    }


    // IInteractable Интерфейс Взаимодействия - позволяет в классе InteractAction взаимодействовать с любым объектом (дверь, сфера, кнопка...) - который реализует этот интерфейс
    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition) // Получить Интерфейс Взаимодействия в этой сеточной позиции
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        return gridObject.GetInteractable();
    }
    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) // Установить полученный Интерфейс Взаимодействия в этой сеточной позиции
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        gridObject.SetInteractable(interactable);
    }
    public void ClearInteractableAtGridPosition (GridPosition gridPosition) // Очистить Интерфейс Взаимодействия в этой сеточной позиции
    {
        GridObject gridObject = _gridSystem.GetGridObject(gridPosition); // Получим GridObject который находится в _gridPosition
        gridObject.ClearInteractable(); // Очистить Интерфейс Взаимодействия в эточ сеточном объекте
    }

}
