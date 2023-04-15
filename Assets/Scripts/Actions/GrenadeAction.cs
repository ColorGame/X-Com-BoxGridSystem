using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction // Граната ДЕйствие. Наследует BaseAction
{
    [SerializeField] private Transform _grenadeProjectilePrefab; // Префаб Снаряд Гранаты // В префабе юнита закинуть префаб гранаты

    [SerializeField] private LayerMask _obstaclesAndDoorLayerMask; //маска слоя препятствия и двери (появится в ИНСПЕКТОРЕ) НАДО ВЫБРАТЬ Obstacles и DoorInteract // ВАЖНО НА ВСЕХ СТЕНАХ В ИГРЕ УСТАНОВИТЬ МАСКУ СЛОЕВ -Obstacles, а на дверях -DoorInteract


    private int _maxThrowDistance = 7; //Максимальная дальность броска //НУЖНО НАСТРОИТЬ//
    private GrenadeProjectile _grenadeProjectile;

    protected override void Awake()
    {
        base.Awake();

        _grenadeProjectile = _grenadeProjectilePrefab.GetComponent<GrenadeProjectile>();
    }

    private void Update()
    {
        if (!_isActive) // Если не активны то ...
        {
            return; // выходим и игнорируем код ниже
        }

        //Если оставить эту строку здесь то мы сможем кидать следующую гранаты не дождавшись пока первая граната долетит до цели (как с автомата пока не кончаться очки действия)
        //Поэтому данную функцию будем вызывать через делегат на самой гранате когда она взорвется
        //ActionComplete(); // 
    }


    public override string GetActionName() // Присвоить базовое действие //целиком переопределим базовую функцию
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //Получить действие вражеского ИИ // Переопределим абстрактный базовый метод
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0, //Поставим низкое значение действия. Будет бросать гранату если ничего другого сделать не может, 
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()// Получить Список Допустимых Сеточных Позиция для Действий // переопределим базовую функцию                                                                       
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = _unit.GetGridPosition(); // Получим позицию в сетке юнита

        for (int x = -_maxThrowDistance; x <= _maxThrowDistance; x++) // Юнит это центр нашей позиции с координатами unitGridPosition, поэтому переберем допустимые значения в условном радиусе _maxSwordDistance
        {
            for (int z = -_maxThrowDistance; z <= _maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // Смещенная сеточная позиция. Где началом координат(0,0) является сам юнит 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition; // Тестируемая Сеточная позиция

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // Проверим Является ли testGridPosition Допустимой Сеточной Позицией если нет то переходим к след циклу
                {
                    continue; // continue заставляет программу переходить к следующей итерации цикла 'for' игнорируя код ниже
                }

                // Для области метания гранаты сделаем ромб а не квадрат
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Сумма двух положительных координат сеточной позиции
                if (testDistance > _maxThrowDistance) //Получим фигуру из ячеек в виде ромба // Если юнит в (0,0) то ячейка с координатами (5,4) уже не пройдет проверку 5+4>7
                {
                    continue;
                }

                /*if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) //Исключим сеточные позиции куда нельзя пройти или на них есть объект с тегом (Obstacles -Препятствия)  (позиции между юнитом и тестируемой позицией)
                {
                    continue;
                }*/

                int pathfindingDistanceMultiplier = 10; // множитель расстояния определения пути (в классе Pathfinding задаем стоимость смещения по клетке и она равна прямо 10 по диогонали 14, поэтому умножем наш множитель на количество клеток)
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > _maxThrowDistance * pathfindingDistanceMultiplier) //Исключим сеточные позиции - Если растояние до тестируемой клетки больше расстояния которое Юнит может преодолеть за один ход
                {
                    // Длина пути слишком велика
                    continue;
                }

                // ПРОВЕРИМ НА возможность броска через препятствия
                Vector3 worldTestGridPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);   // Получим мировые координаты тестируемой сеточной позиции 
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition); // Переведем в мировые координаты переданную нам сеточную позицию Юнита  
                Vector3 grenadeDirection = (worldTestGridPosition - unitWorldPosition).normalized; //Нормализованный Вектор Направления броска Гранаты

                float unitShoulderHeight = 1.7f; // Высота плеча юнита, в дальнейшем будем реализовывать приседание и половинчатые укрытия
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        grenadeDirection,
                        Vector3.Distance(unitWorldPosition, worldTestGridPosition),
                        _obstaclesAndDoorLayerMask)) // Если луч попал в препятствие то (Raycast -вернет bool переменную)
                {
                    // Мы заблоктрованны препятствием
                    continue;
                }

                validGridPositionList.Add(testGridPosition); // Добавляем в список те позиции которые прошли все тесты
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)  // Переопределим TakeAction (Применить Действие (Действовать). (Делегат onActionComplete - по завершении действия). в нашем случае делегату передаем функцию ClearBusy - очистить занятость
    {
        Transform grenadeProjectileTransform = Instantiate(_grenadeProjectilePrefab, _unit.GetWorldPosition(), Quaternion.identity); // Создадим префаб гранаты в позиции игрока 
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>(); // Возьмем у гранаты компонент GrenadeProjectile
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviorComplete); // И вызовим функцию Setup() передав в нее целевую позицию (сеточныая позиция курсора мыши) и передадим в делегат функцию OnGrenadeBehaviorComplete ( при взрыве гранаты будем вызывать эту функцию)

        ActionStart(onActionComplete); // Вызовим базовую функцию СТАРТ ДЕЙСТВИЯ // Вызываем этот метод в конце после всех настроек т.к. в этом методе есть EVENT и он должен запускаться после всех настроек
    }

    private void OnGrenadeBehaviorComplete() // Промежуточный метод который возвращает ActionComplete() . Хотя можно использовать ActionComplete() напрямую но можно запутаться в названиях
    {
        ActionComplete(); // эта функция выполняет - Очистить занятость или стать свободным - активировать кнопки UI
    }

    public int GetMaxThrowDistance()//Раскроем _maxSwordDistance
    {
        return _maxThrowDistance;
    }

    public int GetDamageRadiusInCells() => _grenadeProjectile.GetDamageRadiusInCells(); // Сквозная функция
}

// https://community.gamedev.tv/t/grenade-can-be-thrown-through-wall/205331 БРОСАНИЕ ГРАНАТЫ ЧЕРЕЗ СТЕНЫ