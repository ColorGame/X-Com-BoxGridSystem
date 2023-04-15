using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour // Уровень. Отвечает за взаимодействие (дверей и врагов) (шара-взаимодействия и дверей) (шара-взаимодействия и бочки-взаимодействия)
{
    public static LevelScripting Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                                  // instance - экземпляр, У нас будет один экземпляр LevelScripting можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    public event EventHandler<DoorInteract> OnInteractSphereAndDoor; // Создадим событие - Сфера и Дверь Взаимодействуют //  <> -generic этот тип будем вторым аргументом


    // Враги будут активироваться при открывании опреденных дверей(Первые враги активируются при передвижении по сетке).
    // Некоторые двери будут открываться от шара-взаимодействия.
    // Неторые шары-взаимодействия будут появляться при взаимодействии с бочкой
    [Header("HIDER")]
    // Списки скрывающих полотен
  
    [SerializeField] private List<GameObject> _hider1List; 
    [SerializeField] private List<GameObject> _hider2List;
    [SerializeField] private List<GameObject> _hider3List;
    [SerializeField] private List<GameObject> _hider4List;
    [SerializeField] private List<GameObject> _hider5List;
    [SerializeField] private List<GameObject> _hider6List;
    [SerializeField] private List<GameObject> _hider7List;
    
    [Header("ENEMY LIST")]
    // Списки врагов
   
    [SerializeField] private List<GameObject> _enemy1List;
    [SerializeField] private List<GameObject> _enemy2List;
    [SerializeField] private List<GameObject> _enemy3List;
    [SerializeField] private List<GameObject> _enemy4List;
    [SerializeField] private List<GameObject> _enemy5List;
    [SerializeField] private List<GameObject> _enemy6List;
    [SerializeField] private List<GameObject> _enemy7List;

    [Header("DOOR")]
    // Двери
    
    [SerializeField] private DoorInteract _door1;
    [SerializeField] private DoorInteract _door2;
    [SerializeField] private DoorInteract _door3;
    [SerializeField] private DoorInteract _door4;
    [SerializeField] private DoorInteract _doorSphere5; // будут открываться с помощью сферы
    [SerializeField] private DoorInteract _doorSphere6;
    [SerializeField] private DoorInteract _doorSphere7;

    [Header("SPHERE")]
    // Сфера взаимодействия будут открывать определенные двери
    [SerializeField] private SphereInteract _sphere5; 
    [SerializeField] private SphereInteract _sphere6;
    [SerializeField] private SphereInteract _sphere7;

    [Header("BARREL")]
    // Бочка взаимодействия будет скрывать одну сферу взаимодействия
    [SerializeField] private BarrelInteract _barrelSphere7;


   /* private List<List<GameObject>> _hiderListList;
    private List<List<GameObject>> _enemyListList;
    private List<DoorInteract> _doorInteractList;*/
    //private bool _hasShownFirstHider = false; // Можем показать первый скрытый отряд врагов


    private void Awake() //Для избежания ошибок Awake Лучше использовать только для инициализации и настроийки объектов
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one LevelScripting!(Там больше, чем один LevelScripting!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр LevelScripting прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;

        // Отключим скрытых врагов
        SetActiveGameObjectList(_enemy1List, false);
        SetActiveGameObjectList(_enemy2List, false);
        SetActiveGameObjectList(_enemy3List, false);
        SetActiveGameObjectList(_enemy4List, false);
        SetActiveGameObjectList(_enemy5List, false);
        SetActiveGameObjectList(_enemy6List, false);
        SetActiveGameObjectList(_enemy7List, false);
    }


    private void Start()
    {
        //LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition; // Подпишемся на событие Любой Юнит Перемещен в Сеточной позиции
        
        // Отключим взаимодействие с дверьми которые открываются от Сферы
        _doorSphere5.SetIsInteractable(false);
        _doorSphere6.SetIsInteractable(false);
        _doorSphere7.SetIsInteractable(false);
                

        _door1.OnDoorOpened += (object sender, EventArgs e) => // Подпишемся на событие Дверь открыта. Функцию будем объявлять АНАНИМНО через лямбду () => {...} 
        {   
            // ЭКСПЕРЕМЕНТ РЕФАКТОРИНГ
            /*DoorInteract doorInteract = (DoorInteract)sender;
            int inexList =  _doorInteractList.IndexOf(doorInteract); // Получим индекс открытой двери
            SetActiveGameObjectList(_hiderListList[inexList], false);
            SetActiveGameObjectList(_enemyListList[inexList], true);*/

            SetActiveGameObjectList(_hider1List, false); // Выключим переданный список Скрывающих Полотен
            SetActiveGameObjectList(_enemy1List, true);  // Включим переданный список Врагов
        };

        _door2.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider2List, false);
            SetActiveGameObjectList(_enemy2List, true);
        };
        
        _door3.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider3List, false);
            SetActiveGameObjectList(_enemy3List, true);
        };

        _door4.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider4List, false);
            SetActiveGameObjectList(_enemy4List, true);
        };

        _doorSphere5.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider5List, false);
            SetActiveGameObjectList(_enemy5List, true);
        };

        _doorSphere6.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider6List, false);
            SetActiveGameObjectList(_enemy6List, true);
        };

        _doorSphere7.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(_hider7List, false);
            SetActiveGameObjectList(_enemy7List, true);
        };



        _barrelSphere7.OnBarrelInteractlActivated += (object sender, EventArgs e) =>// Подпишемся на событие Бочка Активированна.   
        {
            _sphere7.gameObject.SetActive(true); // При активации бочки сделаем Сферу взаимодействия АКТИВНОЙ
            _sphere7.UpdateInteractableAtGridPosition(); // Обновить у Сферы, Взаимодействие с Сеточной позицией
        };



        _sphere5.OnInteractSphereActivated += (object sender, EventArgs e) => // Подпишемся на событие Сфера Активированна.
        {
            InteractSphereAndDoor(_doorSphere5);      
        };

        _sphere6.OnInteractSphereActivated += (object sender, EventArgs e) => // Подпишемся на событие Сфера Активированна.
        {
            InteractSphereAndDoor(_doorSphere6);
        };

        _sphere7.OnInteractSphereActivated += (object sender, EventArgs e) => // Подпишемся на событие Сфера Активированна.
        {
            InteractSphereAndDoor(_doorSphere7);
        };
    }


    private void SetActiveGameObjectList(List<GameObject> gameObjectList, bool isActive) // Установить Активным или НЕ активынм Список объектов (в зависимости от переданной нам булевой переменной) 
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            gameObject.SetActive(isActive);
        }
    }

    private void InteractSphereAndDoor(DoorInteract doorInteract) // Взаимодействуют сфера и дверь
    {
        doorInteract.SetIsInteractable(true); // Включим взаимодействие с дверью
        doorInteract.UpdateStateDoor(true); //Обновим состояние двери в зависимочти от переданной булевой переменной
        OnInteractSphereAndDoor?.Invoke(this, doorInteract); // Запустим событие Сфера и Дверь Взаимодействуют (подписчик CameraManager для переключения камеры на дверь) // В аргумент передадим дверь с которой происходит взаимодействие
    }



    

   /* private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        if (e.toGridPosition.z == 5 && !_hasShownFirstHider) // Если игрок пройдет вверх 5 клеток(ячеек) и Еще не были показаны первые враги то...
        {
            _hasShownFirstHider = true; // Можем показать первый скрытый отряд врагов
            SetActiveGameObjectList(_hider1List, false); // Выключим полотно которое скрывает
            SetActiveGameObjectList(_enemy1List, true); // Включим 1 лист врагов
        }
    }*/
}
