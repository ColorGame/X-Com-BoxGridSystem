using System.Collections.Generic;
using UnityEngine;

// НАСТРОИМ ПОРЯДОК ВЫПОЛНЕНИЯ СКРИПТА Pathfinding, добавим в Project Settings/ Script Execution Order и поместим выполнение Pathfinding выше Default Time, чтобы Pathfinding запустился РАНЬШЕ до того как ктонибудь совершит поиск пути 

public class Pathfinding : MonoBehaviour // Поиск пути // Логика которая будет обрабатывать данные для поиска пути (похож на класс LevelGrid)
{

    public static Pathfinding Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                               // instance - экземпляр, У нас будет один экземпляр Pathfinding можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.



    private const int MOVE_STRAIGHT_COST = 10; // Стоимость движения прямо (для удобства взяли 10 а не 1 что бы не использовать float)
    private const int MOVE_DIAGONAL_COST = 14; // Стоимость движения по диоганали ( расчитывается по теореме пифагора корень квадратный из суммы квадратов катетов прямоугольного треугольника. И опять для удобства взяли 14 а не 1,4 что бы не использовать float)

    //переменные для ДРУГОЙ РЕАЛИЗАЦИИ МЕТОДА GetNeighbourList() БОЛЕЕ КОРОТКАЯ
    /*// Координаты сеточных позиций для поиска соседних ячеек
    private GridPosition UP = new(0, 1);
    private GridPosition DOWN = new(0, -1);
    private GridPosition RIGHT = new(1, 0);
    private GridPosition LEFT = new(-1, 0);*/

    [SerializeField] private Transform _pathfindingGridDebugObject; // Префаб отладки сетки //Передоваемый тип должен совподать с типом аргумента метода CreateDebugObject
    [SerializeField] private LayerMask _obstaclesLayerMask; // маска слоя препятствия (появится в ИНСПЕКТОРЕ) НАДО ВЫБРАТЬ Obstacles // ВАЖНО НА ВСЕХ СТЕНАХ В ИГРЕ УСТАНОВИТЬ МАСКУ СЛОЕВ -Obstacles кроме дверей

    private int _width;     // Ширина
    private int _height;    // Высота
    private float _cellSize;// Размер ячейки
    private GridSystem<PathNode> _gridSystem; // Частная сеточная система с типом PathNode

    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one Pathfinding!(Там больше, чем один Pathfinding!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр Pathfinding прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;
    }

    public void Setup(int width, int height, float cellSize) // Настроим Узлы поиска путей
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridSystem = new GridSystem<PathNode>(_width, _height, _cellSize,                          // ПОСТРОИМ СЕТКУ 10 на 10 и размером 2 еденицы и в каждой ячейки создадим объект типа PathNode
                (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition)); //в четвертом параметре аргумента зададим функцию ананимно через лямбду => new PathNode(_gridPosition) И ПЕРЕДАДИМ ЕЕ ДЕЛЕГАТУ. (лямбда выражение можно вынести в отдельный метод)
        
       // _gridSystem.CreateDebugObject(_pathfindingGridDebugObject); // Создадим наш префаб в каждой ячейки// Отладочный объект можно убирать т.к. настроика завершена

        // В цикле настроим все ячейки на возможность проходимости, будем стрелять лучом из каждой позиции и если луч сталкнется с препядствием _obstaclesAndDoorLayerMask, установим эту ячейку не проходимой
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); // Позиция сетке
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition); // Получим мировые координаты
                float raycastOffsetDistance = 5f; // Дистанция смещения луча

                //Выстрелим ЛУЧ. ЛУч не может стрелять внутри колайдера(внутри стены), поэтому сместим его вниз, и будем стрелять вверх, лучом размер которого в два раза больше чем смещение вниз, луч будет взаимодействовать с выбранной маской слоя
                // МОЖНО СДЕЛАТЬ НАСТРОЙКИ в UNITY и не смещать выстрел луча. Project Settings/Physics/Queries Hit Backfaces - поставить галочку, и тогда можно стрелять из нутри колайдера
                if (Physics.Raycast(
                     worldPosition + Vector3.down * raycastOffsetDistance,
                     Vector3.up,
                     raycastOffsetDistance * 2,
                     _obstaclesLayerMask)) // Если луч попал в препятствие то установим ячейку НЕ ПРОХОДИМОЙ (Raycast -вернет bool переменную)
                {
                    GetNode(x, z).SetIsWalkable(false);
                }                
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength) // Найти путь // Если мы используете ключевое слово "out", то функция должна установить значение для этой переменной pathLength-длина пути, то есть она должна вывести значение.
    {
        List<PathNode> openList = new List<PathNode>();     // Создадим список "Открытый Список" - ВСЕ УЗЛЫ КОТОРЫЕ ПРЕДСТОИТ НАЙТИ 
        List<PathNode> closedList = new List<PathNode>();   // Создадим список "Закрытый Список" - ВСЕ УЗЛЫ, В КОТОРЫХ УЖЕ БЫЛ ПРОИЗВЕДЕН ПОИСК

        PathNode startNode = _gridSystem.GetGridObject(startGridPosition); // Получим стартовый узел, вернем GridObject типа PathNode в переданной нам startGridPosition
        PathNode endNode = _gridSystem.GetGridObject(endGridPosition); // Получим конечный узел, вернем GridObject типа PathNode в переданной нам endGridPosition

        openList.Add(startNode); //Добавим в список начальный узел

        for (int x = 0; x < _gridSystem.GetWidth(); x++) // В цикле пройдем все узлы сеточной системы и сбросим параметры
        {
            for (int z = 0; z < _gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); // Позиция сетке

                PathNode pathNode = _gridSystem.GetGridObject(gridPosition); // Получим объект сетки типа PathNode

                pathNode.SetGCost(int.MaxValue); // Установим параметр G максимальным числом
                pathNode.SetHCost(0);            // Установим параметр H =0
                pathNode.CalculateFCost();       // Расчитаем параметр F
                pathNode.ResetCameFromPathNode(); // Сбросим ссылку на предыдущий Узел Пути 
            }
        }

        startNode.SetGCost(0); // G -это стоимость перехода от предыдущего узла к настоящему который тестируем. Мы еще не откуда не перемещались поэтому G=0
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition)); // Установим вычисленный H компонент
        startNode.CalculateFCost();

        while (openList.Count > 0) // если в Открытом списке есть элементы то это означает что есть узлы для поиска. Цикл будет работать пока не переберет все ячейки
        {
            PathNode currentNode = GetLowestFCostPathNode(openList); // Получим узел пути с наименьшей стоимостью F из openList и зделаем его Текущим Узлом

            if (currentNode == endNode) // Проверяем равен ли наш текущий узел конечному узлу
            {
                // Достигли конечного узла
                pathLength = endNode.GetFCost(); // Вернем стоимость длины пути
                return CalculatePath(endNode); // Вернем вычисленый путь
            }

            openList.Remove(currentNode); // Удалим текущий узел из открытого списка
            closedList.Add(currentNode);  // И добавим в закрытый список // ЭТО ОЗНАЧАЕТ ЧТО МЫ ИСКАЛИ ПО ЭТОМУ УЗЛУ

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) // Переберем всех соседние узлы
            {
                if (closedList.Contains(neighbourNode)) // Проверяем что бы соседний узел небыл в "Закрытом списке"
                {
                    //МЫ УЖЕ ИСКАЛИ ПО ЭТОМУ УЗЛУ
                    continue;
                }

                //  ПРОПУСТИМ УЗЛЫ НЕ ДОСТУПНЫЕ ДЛЯ ХОТЬБЫ
                if (!neighbourNode.GetIsWalkable()) // Проверим, соседний узел - доступен для хотьбы // Есди Не доступен то добавим в "Закрытый Список" и перейдем к следующему узлу
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition()); // Предварительная стоимость G = текущая G + стоимость перемещения от текущего к соседнему узлу

                if (tentativeGCost < neighbourNode.GetGCost()) // Если Предварительная стоимость G меньше стоимости G соседнего узла(по умолчанию она имеет значение MaxValue ) (Мы нашли луший путь что бы попасть в этот соседний узел)
                {
                    neighbourNode.SetCameFromPathNode(currentNode); // Установить - на соседний узел пришел - С текущего Узла Пути
                    neighbourNode.SetGCost(tentativeGCost); // Установим на соседний узем расчитанный параметр G
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition)); // Установим параметр H от текущего до конечного
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) // Если открытый список не содержит этого соседнего узла то добавим его
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // ПУТЬ НЕ НАЙДЕН
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB) // Вычислим приблизительное растояние между любыми 2мя сеточными позициями ЭТО БУДЕТ ПАРАМЕТР "H"
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB; //Может быть отрицательным (вычислить приращение координат между первой и второй точкой - на сколько надо сместить относительно первой точки)

        // для движение только по прямой
        //int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z); // получим сумму координат по модулю (не учитывает движение по диогонали только по прямой. Например из точки (0,0) в точку (3,2) двигался три раза в право и два раза вверх итого 5)

        int xDistsnce = Mathf.Abs(gridPositionDistance.x); //перемещение с (0,0) на (1,1) можем заменить 1 диоганалью, вместо ДВУХ перемещений по прямой // Если с (0,0) на (2,1) то диогональ будет все равно ОДНА. Поэтому для расчета количества диоганалей надо взять минимальное число из полученных (x,z)
        int zDistsnce = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistsnce - zDistsnce); // Оставшееся растояние будет перемещением по прямой, берем по модулю
        int CalculateDistance = MOVE_DIAGONAL_COST * Mathf.Min(xDistsnce, zDistsnce) + MOVE_STRAIGHT_COST * remaining; // вернем вычесленное расстояние в системе GridPosition как сумму смещений по диогонали и смещений по прямой
        return CalculateDistance;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList) // Получить узел пути с наименьшей стоимостью F  в аргумент передадим список узлов пути// для оптимизации поиска чтобы не искать по всем подрят
                                                                         // Если несколько ячеек имеют одинаковое наименьшее F то вернется первый попавшийся в списке
                                                                         // В нашем случае мы перебераем ячейки против часовой (зависит от формирования списка в методе GetNeighbourList())
    {
        PathNode lowestFCostPathNode = pathNodeList[0]; // Возьмем первый элемент в списке
        for (int i = 0; i < pathNodeList.Count; i++) // переберем в цикле другие элементы
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) // Если параметр F нового элемента меньше текущего то сделаем его lowestFCostPathNode
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z) // Получить Узел с координатами по GridPosition(x,z)
    {
        return _gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) // Получить список соседей для currentNode
    {
        List<PathNode> neighbourList = new List<PathNode>(); // Инициализируем новый список соседей

        GridPosition gridPosition = currentNode.GetGridPosition();

        // Сделаем ПРОВЕРКИ чтобы не уйти за границы сетки иначе получем ошибку ссылку на нулевой объект

        if (gridPosition.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0)); // Добавим узел слева
            if (gridPosition.z - 1 >= 0)
            {
                //Left Down
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1)); // Добавим узел слева в низу
            }

            if (gridPosition.z + 1 < _gridSystem.GetHeight())
            {
                //Left Up
                neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1)); // Добавим узел слева в верху
            }
        }

        if (gridPosition.x + 1 < _gridSystem.GetWidth())
        {
            //Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0)); // Добавим узел справа

            if (gridPosition.z - 1 >= 0)
            {
                //Right Down
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1)); // Добавим узел справа в низу
            }

            if (gridPosition.z + 1 < _gridSystem.GetHeight())
            {
                //Right Up
                neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1)); // Добавим узел справа в верху
            }
        }

        if (gridPosition.z - 1 >= 0)
        {
            //Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1)); // Добавим узел снизу
        }
        if (gridPosition.z + 1 < _gridSystem.GetHeight())
        {
            //Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1)); // Добавим узел сверху
        }
        return neighbourList;
    }


    //2 ДРУГАЯ РЕАЛИЗАЦИЯ МЕТОДА GetNeighbourList() БОЛЕЕ КОРОТКАЯ
    /* private List<PathNode> GetNeighbourList(PathNode currentNode) // Получить список соседей для currentNode
     {
         List<PathNode> neighbourList = new List<PathNode>(); // Инициализируем новый список соседей

         GridPosition _gridPosition = currentNode.GetGridPosition(); // Получим сеточную позицию центра поиска

         GridPosition[] neigboursPositions = //Создадим массив сеточных позиций соседних ячеик
         {
         _gridPosition + UP,
         _gridPosition + UP + RIGHT,
         _gridPosition + RIGHT,
         _gridPosition + RIGHT + DOWN,
         _gridPosition + DOWN,
         _gridPosition + DOWN + LEFT,
         _gridPosition + LEFT,
         _gridPosition + LEFT + UP
         };

         foreach (GridPosition p in neigboursPositions) // В цикле проверим на допустипость этих сеточных позиций
         {
             if (_gridSystem.IsValidGridPosition(p))
             {
                 neighbourList.Add(GetNode(p.x, p.z));
             }                
         }

         return neighbourList;
     }*/

    private List<GridPosition> CalculatePath(PathNode endNode) //Вычисление пути (будем разматывать клубок в обратном направлении)
    {
        List<PathNode> pathNodeList = new List<PathNode>(); // инициализируем список Узлов пути
        pathNodeList.Add(endNode);
        PathNode currentNod = endNode;

        while (currentNod.GetCameFromPathNode() != null) //В цикле добавим подключенные узлы в список. Подключенные узлы - это то с которого на него пришли (у всех узлов 1 ссылка на соседний с которого пришли). Последий узел будет стартовый и у него GetCameFromPathNode() = null - цикл прирвется
        {
            pathNodeList.Add(currentNod.GetCameFromPathNode());//Добавим в список наш подключенный узел
            currentNod = currentNod.GetCameFromPathNode(); // Подключенный узел становиться текущим
        }

        pathNodeList.Reverse(); // Т.к. мы начали с конца надо перевернуть наш список что бы получить тропинку от старта к финишу
        // Переведем наш список "PathNode-узлов" в список "GridPosition - позиций сетки"
        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable) // Установить что Можно или Нельзя (в зависимости от isWalkable)  ходить по переданной в аргумент Сеточной Позиции
    {
       _gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition) // Можно ходить по переданной в аргумент Сеточной Позиции
    {
        return _gridSystem.GetGridObject(gridPosition).GetIsWalkable(); 
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition) // Имеем путь ?
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null; // Если есть путь от startGridPosition к endGridPosition то вернется true если пути нет вернктся false (out int pathLength добавили что бы соответствовала сигнатуре)
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition) // Получить длину пути (параметр F -endGridPosition)  
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

}
