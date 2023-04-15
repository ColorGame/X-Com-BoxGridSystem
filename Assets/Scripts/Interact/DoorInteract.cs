using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DoorInteract : MonoBehaviour, IInteractable //Дверь-Взаимодействия Расширим класс интерфейсом 
{
    public static event EventHandler OnAnyDoorOpened; // Мы запустим событие Event ЛЮБАЯ(Any) Дверь открыта.
                                                      // static - обозначает что event будет существовать для всего класса, а не для оттдельной ДВЕРИ.
                                                      // Поэтому для прослушивания этого события слушателю не нужна ссылка на конкретный объект, они могут получить доступ к событию
                                                      // через класс, который затем запускает одно и то же событие для каждого объекта. 

    public static event EventHandler OnAnyDoorIsLocked; //Запутим событие когда -Любая Дверь Заперта(нельзя открыть вручную)
                                                        // static - обозначает что event будет существовать для всего класса, а не для оттдельной ДВЕРИ.

    public event EventHandler OnDoorOpened; //Запутим событие когда - Дверь открыта




    [SerializeField] private bool _isOpen; //Открыта (дверь)
    [SerializeField] private bool _isInteractable = true; //Можно взаимодействовать (по умолчанию true)

    private bool _isActive;
    private float _timer; // Таймер который не будет позволять непрерывно взаимодействовать с дверью
    private Transform[] _transformChildrenDoorArray;  //Массив Дочерних объектов двери (это сама дверь[0] левая[1] b правая[2] дверь)
    private Animator _animator; //Аниматор на двери

    private Action _onInteractionComplete; // Делегат Взаимодействие Завершено// Объявляю делегат в пространстве имен - using System;
                                           //Сохраним наш делегат как обыкновенную переменную (в ней будет храниться функия которую мы передадим).
                                           //Action- встроенный делегат. Есть еще встроен. делегат Func<>. 
                                           //СВОЙСТВО Делегата. После выполнения функции в которую мы передали делегата, можно ПОЗЖЕ, в определенном месте кода, выполниться сохраненный делегат.
                                           //СВОЙСТВО Делегата. Может вызывать закрытую функцию из другого класса

    private List<GridPosition> _doorGridPositionList = new List<GridPosition>();



    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transformChildrenDoorArray = GetComponentsInChildren<Transform>();
    }


    private void Start()
    {
        foreach (GridPosition gridPosition in GetDoorGridPositionList()) // Переберем список сеточных позиции которые занимает Дверь
        {
            LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this); // в полученную сеточную позицию установим наш дочерний объект Дверь с Интерфейсом Interactable(взаимодействия)
        }

        UpdateStateDoor(_isOpen);

    }

    private void Update()
    {
        if (!_isActive) // Если происходит взаимодействие с дверью то она активна и выполняется код ниже
        {
            return;// выходим и игнорируем код ниже
        }

        _timer -= Time.deltaTime; // Запустим таймер

        if (_timer <= 0f)
        {
            _isActive = false; // Выключаем Активное состояние
            _onInteractionComplete(); // Вызовим сохраненный делегат который нам передала функция Interact(). В нашем случае это ActionComplete() он снимает занятость с кнопок UI
        }
    }

    public void UpdateStateDoor(bool isOpen) //Обновим состояние двери в зависимочти от переданной булевой переменной
    {
        if (isOpen) // Обработаем установленные по умолчанию настроики двери 
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }



    public void Interact(Action onInteractionComplete) // Взаимодействие. В аргумент предаем Делегат Взаимодействие Завершено
    {
        _onInteractionComplete = onInteractionComplete; // Сохраним полученный делегат
        _isActive = true; // Переключаем Дверь в Активное состояние
        _timer = 1f; // Задаем время активного состояния  //НУЖНО НАСТРОИТЬ//

        if (_isInteractable) // Если с дверью можно взаимодействовать то
        {
            if (_isOpen) // При взаимодействии, если дверь открыта будем ее закрывать и наоборот
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
        else
        {
            // МОЖНО РЕАЛИЗОВАТЬ ЗВУК НЕУДАЧНОГО ОТКРЫВАНИЯ или запустить событие
            OnAnyDoorIsLocked?.Invoke(this, EventArgs.Empty); // Запустим событие любая дверь заперта (для реализации надписи)
        }
    }

    private void OpenDoor() // Открыит дверь
    {
        _isOpen = true;
        _animator.SetBool("IsOpen", _isOpen); // Настроим булевую переменную "IsOpen". Передадим ей значение _isOpen

        foreach (GridPosition gridPosition in _doorGridPositionList) // Переберем список сеточных позиции которые занимает Дверь
        {
            Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true); // Установим что Можно ходить по этой сеточной позиции
        }

        // Запустим события
        OnDoorOpened?.Invoke(this, EventArgs.Empty);
        OnAnyDoorOpened?.Invoke(this, EventArgs.Empty);
    }

    private void CloseDoor() // Закрыть дверь
    {
        _isOpen = false;
        _animator.SetBool("IsOpen", _isOpen); // Настроим булевую переменную "IsOpen". Передадим ей значение _isOpen

        foreach (GridPosition gridPosition in _doorGridPositionList) // Переберем список сеточных позиции которые занимает Дверь
        {
            Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false); // Установим что Нельзя ходить по этой сеточной позиции
        }
    }

    private List<GridPosition> GetDoorGridPositionList() //Получить Список Сеточных позиций двери  
    {
        //Большие двери и те, через которые можно стрелять https://community.gamedev.tv/t/larger-doors-and-doors-that-can-be-shot-through/220723*/

        float offsetFromEdgeGrid = 0.01f; // Смещение от Края Сетки (ячейки) //НУЖНО НАСТРОИТЬ//

        GridPosition childreGridPositionLeft = LevelGrid.Instance.GetGridPosition(_transformChildrenDoorArray[1].position + _transformChildrenDoorArray[1].right * offsetFromEdgeGrid); // Определим сеточную позицию Дочернего объекта двери Левая створока двери (сместимся от границы ячейки что бы не попасть на соседнию слева)
        GridPosition childreGridPositionRight = LevelGrid.Instance.GetGridPosition(_transformChildrenDoorArray[2].position - _transformChildrenDoorArray[2].right * offsetFromEdgeGrid); // Определим сеточную позицию Дочернего объекта двери Правая створока двери (сместимся от границы ячейки что бы не попасть на соседнию справа)

        //_doorGridPositionList = Pathfinding.Instance.FindPath(childreGridPositionRight, childreGridPositionLeft, out int pathLength); // Зависимость от поиска пути

        // Заместо поиска пути между дверьми можно использовать код ниже, но не подходит для дверей расположенных по диоганали
        // ПЕРЕНДИКУЛЯРНАЯ ДВЕРЬ
        if (childreGridPositionLeft.x == childreGridPositionRight.x)
        {
            if (childreGridPositionLeft.z <= childreGridPositionRight.z) //при условии что левая створка ниже правой
            {
                for (int z = childreGridPositionLeft.z; z <= childreGridPositionRight.z; z++)  // переберем сеточные позиции по оси Z (преберем ячейки от левой створки до правой).
                {
                    GridPosition testGridPosition = new GridPosition(childreGridPositionLeft.x, z); // Тестируемая сеточная позиция по оси Z. Сеточную позицию по X можно взять любой створки

                    _doorGridPositionList.Add(testGridPosition); // Добавим в список тестируемую ячейку    
                }
            }

            if (childreGridPositionRight.z <= childreGridPositionLeft.z) //при условии что правая створка ниже левой
            {
                for (int z = childreGridPositionRight.z; z <= childreGridPositionLeft.z; z++)  // переберем сеточные позиции по оси Z (преберем ячейки от правой створки до левой).
                {
                    GridPosition testGridPosition = new GridPosition(childreGridPositionLeft.x, z); // Тестируемая сеточная позиция по оси Z. Сеточную позицию по X можно взять любой створки

                    _doorGridPositionList.Add(testGridPosition); // Добавим в список тестируемую ячейку    
                }
            }
        }

        // ГОРИЗОНТАЛЬНАЯ ДВЕРЬ
        if (childreGridPositionLeft.z == childreGridPositionRight.z)
        {
            if (childreGridPositionLeft.x <= childreGridPositionRight.x) //при условии что левая створка слева от правой
            {
                for (int x = childreGridPositionLeft.x; x <= childreGridPositionRight.x; x++)  // переберем сеточные позиции по оси Х (преберем ячейки от левой створки до правой).
                {
                    GridPosition testGridPosition = new GridPosition(x, childreGridPositionLeft.z); // Тестируемая сеточная позиция по оси Х. Сеточную позицию по Z можно взять любой створки

                    _doorGridPositionList.Add(testGridPosition); // Добавим в список тестируемую ячейку    
                }
            }

            if (childreGridPositionLeft.x >= childreGridPositionRight.x) //при условии что левая створка справа от правой
            {
                for (int x = childreGridPositionRight.x; x <= childreGridPositionLeft.x; x++)  // переберем сеточные позиции по оси Х (преберем ячейки от правой створки до левой).
                {
                    GridPosition testGridPosition = new GridPosition(x, childreGridPositionLeft.z); // Тестируемая сеточная позиция по оси Х. Сеточную позицию по Z можно взять любой створки

                    _doorGridPositionList.Add(testGridPosition); // Добавим в список тестируемую ячейку    
                }
            }
        }

        return _doorGridPositionList;
    }

    public void SetIsInteractable(bool isInteractable) // Установить Можно Взаимодействовать
    {
        _isInteractable = isInteractable;
    }
}

