using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Добавим GridSystemVisual для запуска после времени по умолчанию, поскольку мы хотим, чтобы визуальные эффекты запускались после всего остального.
// (Project Settings/ Script Execution Order и поместим выполнение GridSystemVisual НИЖЕ Default Time)
public class GridSystemVisual : MonoBehaviour //Сеточная система визуализации  Визуализация возможных ходов по сетке 
{
    public static GridSystemVisual Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                                    // instance - экземпляр, У нас будет один экземпляр UnitActionSystem можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    [Serializable] // Чтобы созданная структура могла отображаться в инспекторе
    public struct GridVisualTypeMaterial    //Визуал сетки Тип Материала // Создадим структуру можно в отдельном классе. Наряду с классами структуры представляют еще один способ создания собственных типов данных в C#
    {                                       //В данной структуре объединям состояние сетки с материалом
        public GridVisualType gridVisualType;
        public Material materialGrid;
    }

    public enum GridVisualType //Визуальные состояния сетки
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }


    [SerializeField] private Transform _gridSystemVisualSinglePrefab; // Брефаб визуализации 
    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList; // Список тип материала визуального состояния сетки (Список из кастомного типа данных) визуального состояния сетки // В инспекторе под каждое состояние перетащить соответствующий материал сетки



    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray; // Двумерный массив


    private void Awake() //Для избежания ошибок Awake Лучше использовать только для инициализации и настроийки объектов
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one UnitActionSystem!(Там больше, чем один UnitActionSystem!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр UnitActionSystem прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;
    }

    private void Start()
    {
        _gridSystemVisualSingleArray = new GridSystemVisualSingle[ // создаем массив определенного размером width на height
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform gridSystemVisualSingleTransform = Instantiate(_gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity); // Создадим наш префаб в каждой позиции сетки

                _gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>(); // Сохраняем компонент GridSystemVisualSingle в двумерный массив где x,z это будут индексы массива.
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // подпишемся на событие Выбранное Действие Изменено (когда меняется активное действие в блоке кнопок мы запустим событие Event)
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition; // подпишемся на событие Любой Юнит Перемещен в Сеточной позиции

        MouseWorld.OnMouseGridPositionChanged += MouseWorld_OnMouseGridPositionChanged;// подпишемся на событие Сеточная Позиция Мыши Изменена
              
        UpdateGridVisual();
    }

   

    private void MouseWorld_OnMouseGridPositionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPosition() // Скрыть все позиции сетки
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                _gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType, bool showFigureRhombus) // Показать возможный Диапазон Сеточных Позиций для стрельбы (в аргументе передаем Сеточную позицию, Радиус стрельбы, Тип состояния Визуала Сетки, Булевая переменная если надо отобразить в виде Ромба то передаем true, если в виде КВАДРАТА то - false )
    {
        // По аналогии как в ShootAction в методе "public override List<GridPosition> GetValidActionGridPositionList()"

        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)  // Юнит это центр нашей позиции с координатами unitGridPosition, поэтому переберем допустимые значения в условном радиусе range
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // Смещенная сеточная позиция. Где началом координат(0,0) является сам юнит 
                GridPosition testGridPosition = gridPosition + offsetGridPosition; // Тестируемая Сеточная позиция

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // Проверим Является ли testGridPosition Допустимой Сеточной Позицией если нет то переходим к след циклу
                {
                    continue; // continue заставляет программу переходить к следующей итерации цикла 'for' игнорируя код ниже
                }

                if (showFigureRhombus)
                {
                    // Для области выстрела сделаем ромб а не квадрат
                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Сумма двух положительных координат сеточной позиции
                    if (testDistance > range) //Получим фигуру из ячеек в виде ромба // Если юнит в (0,0) то ячейка с координатами (5,4) уже не пройдет проверку 5+4>7
                    {
                        continue;
                    }
                }                

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType); // Покажем возможный Диапазон стрельбы
    }       

    public void ShowGridPositionList(List<GridPosition> gridPositionlist, GridVisualType gridVisualType)  //Покажем Список GridPosition (в аргументе передается список GridPosition и состояние визуализации сетки gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionlist) // в цикле перебереи список и Покажем(включим) только те позиции которые нам передали
        {
            _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                Show(GetGridVisualTypeMaterial(gridVisualType)); // В аргумент Show предадим материал в зависимости от переданного нам события
        }
    }

    public void UpdateGridVisual() // Обнавление визуала сетки
    {
        HideAllGridPosition(); // Скрыть все позиции сетки

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit(); //Получим Выбранного Юнита

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction(); // Получим Выбранное Действие

        GridVisualType gridVisualType;  // Создадим перем типа GridVisualType

        switch (selectedAction) // Переключатель состояний визуала сетки в зависимости от Выбранного Действия
        {
            default: // Этот кейс будет выполняться по умолчанию если нет соответствующих selectedAction
            case MoveAction moveAction: // Во время ХОТЬБЫ -БЕЛЫЙ
                gridVisualType = GridVisualType.White;
               
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)
                break;

            case SpinAction spinAction: // Во время ПОВОРОТА -ГОЛУБОЙ
                gridVisualType = GridVisualType.Blue;
               
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)
                break;

            case ShootAction shootAction: // Во время СТРЕЛЬБЫ -КРАСНЫЙ
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft,true); // Покажем диапазон стрельбы
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)

                break;

            case GrenadeAction grenadeAction:// Во время кидания ГРАНАТЫ -ЖЕЛТЫЙ
                gridVisualType = GridVisualType.Yellow;

                List<GridPosition> validActionGridPositionList = selectedAction.GetValidActionGridPositionList(); // Допустимая сеточная позиция для действия Гранаты
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()); // Сеточная позиция мыши

                ShowGridPositionList(validActionGridPositionList, gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)

                if (validActionGridPositionList.Contains(mouseGridPosition)) // Если Сеточная позиция мыши входит в Допустимый диапазон то ...
                {
                    ShowGridPositionRange(mouseGridPosition, grenadeAction.GetDamageRadiusInCells(), GridVisualType.Red, false); // Покажем радиус действия гранаты цвет КРАСНЫЙ
                }
                break;

            case SwordAction swordAction: // Во время удара МЕЧОМ -красный
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft, false); // Покажем диапазон удара
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)
                break;

            case InteractAction interactAction : // Во время ВЗАИМОДЕЙСТВИЯ -ГОЛУБОЙ
                gridVisualType = GridVisualType.Blue;
                
                ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)
                break;
        }

        //ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType); // Покажем(включим) только те позиции которые нам передали (в аргумент передаем список допустимых позиций сетки выбранного действия, и Тип состояния визуализации который нам выдал switch)
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        UpdateGridVisual();

        //Основное изменение, которое вы можете сделать, это перестать обновлять траекторию всякий раз, когда юнит перемещает позицию,
        //вместо этого вычислять траекторию только тогда, когда Юнит достигает конечной точки.
        //Это устранит заикание при движении устройства, и оно будет ощущаться намного плавнее.
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) //(Вернуть Материал в зависимости от Состояния) Получить Тип Материала для Сеточной Визуализации в зависимости от переданного в аргумент Состояния Сеточной Визуализации
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in _gridVisualTypeMaterialList) // в цикле переберем Список тип материала визуального состояния сетки 
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType) // Если  Состояние сетки(gridVisualType) совподает с переданным нам состояние то ..
            {
                return gridVisualTypeMaterial.materialGrid; // Вернем материал соответствующий данному состоянию сетки
            }
        }

        Debug.LogError("Не смог найти GridVisualTypeMaterial для GridVisualType " + gridVisualType); // Если не найдет соответсвий выдаст ошибку
        return null;
    }
}
