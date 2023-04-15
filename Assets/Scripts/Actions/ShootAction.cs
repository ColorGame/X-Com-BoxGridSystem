using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;  // Событие - Любой Начал стрелять (когда любой юнит начнет стрелять мы запустим событие Event) // <Unit> вариант передачи целевого юнита для пули
                                                                    // static - обозначает что event будет существовать для всего класса не зависимо от того скольго у нас созданно Юнитов.
                                                                    // Поэтому для прослушивания этого события слушателю не нужна ссылка на какую-либо конкретную единицу, они могут получить доступ к событию через класс,
                                                                    // который затем запускает одно и то же событие для каждой единицы. 

    public event EventHandler<OnShootEventArgs> OnShoot; // Начал стрелять (когда юнит начнет стрелять мы запустим событие Event) // <OnShootEventArgs> вариант передачи целевого юнита для пули

    public class OnShootEventArgs : EventArgs // Расширим класс событий, чтобы в аргументе события передать нужных юнитов
    {
        public Unit targetUnit; // Целевой юнит в кого стреляем
        public Unit shootingUnit; // Стереляющий юнит это кто стреляет
    }

    private enum State
    {
        Aiming,     // Прицеливание
        Shooting,   // Стрельба
        Cooloff,    // Остывание (небольшая задержка прежде чем мы закончим действие)
    }

    [SerializeField] private LayerMask _obstaclesAndDoorLayerMask; //маска слоя препятствия и двери (появится в ИНСПЕКТОРЕ) НАДО ВЫБРАТЬ Obstacles и DoorInteract // ВАЖНО НА ВСЕХ СТЕНАХ В ИГРЕ УСТАНОВИТЬ МАСКУ СЛОЕВ -Obstacles, а на дверях -DoorInteract

    private State _state; // Состояние юнита
    private int _maxShootDistance = 7;
    private float _stateTimer; //Таймер состояния
    private Unit _targetUnit; // Юнит в которого стреляем целимся
    private bool _canShootBullet; // Может стрелять пулей


    private void Update()
    {
        if (!_isActive) // Если не активны то ...
        {
            return; // выходим и игнорируем код ниже
        }

        _stateTimer -= Time.deltaTime; // Запустим таймер для переключения состояний

        switch (_state) // Переключатель активурует кейс в зависимости от _state
        {
            case State.Aiming:

                Vector3 aimDirection = (_targetUnit.GetWorldPosition() - transform.position).normalized; // Направление прицеливания, еденичный вектор
                float rotateSpeed = 10f; //НУЖНО НАСТРОИТЬ// чем больше тем быстрее
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed); // поворт юнита.

                break;
            case State.Shooting:
                if (_canShootBullet) // Если могу стрелять пулей то ...
                {
                    Shoot();
                    _canShootBullet = false;
                }
                break;
            case State.Cooloff: // Блок пустой но можно добавить анимацию попадания или промоха
                break;
        }

        if (_stateTimer <= 0) // По истечению времени вызовим NextState() которая в свою очередь переключит состояние. Например - у меня было State.Aiming: тогда в case State.Aiming: переключу на State.Shooting;
        {
            NextState(); //Следующие состояние
        }
    }

    private void NextState() //Автомат переключения состояний
    {
        switch (_state)
        {
            case State.Aiming:
                _state = State.Shooting;
                float shootingStateTime = 0.1f; // Для избежания магических чисель введем переменную  Продолжительность Состояния Выстрел
                _stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                _state = State.Cooloff;
                float cooloffStateTime = 0.5f; // Для избежания магических чисель введем переменную  Продолжительность Состояния Охлаждения //НАДО НАСТРОИТЬ// Продолжительность анимации выстрела(наведение камеры))
                _stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete(); // Вызовим базовую функцию ДЕЙСТВИЕ ЗАВЕРШЕНО
                break;
        }

        //Debug.Log(_state);
    }

    private void Shoot() // ВЫстрел
    {
        //Вариант использования кода для тряски эрана. Но это делает код зависимым от наличия ScreenShake. Поэтому реализуем по другому
        //ScreenShake.Instance.Shake(5);
        OnAnyShoot?.Invoke(this, new OnShootEventArgs // создаем новый экземпляр класса OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootingUnit = _unit
        }); // Запустим событие ЛЮБОЙ Начал стрелять и в аргумент передадим в кого стреляют и кто стреляет (Подписчики ScreenShakeActions ДЛЯ РЕАЛИЗАЦИИ ТРЯСКИ ЭКРАНА и UnitRagdollSpawner- для определения направления поражения)

        OnShoot?.Invoke(this, new OnShootEventArgs // создаем новый экземпляр класса OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootingUnit = _unit
        }); // Запустим событие Начал стрелять и в аргумент передадим в кого стреляют и кто стреляет (UnitAnimator-подписчик)
                
        _targetUnit.Damage(40); // ДЛЯ ТЕСТА УЩЕРБ БУДЕТ 40. В дальнейшем будем брать этот показатель из оружия //НАДО НАСТРОИТЬ//
    }

    public override string GetActionName() // Получим имя для кнопки
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList() //Получить Список Допустимых Сеточных Позиция для Действий // переопределим базовую функцию
    {
        GridPosition unitGridPosition = _unit.GetGridPosition(); // Получим позицию в сетке юнита
        return GetValidActionGridPositionList(unitGridPosition);
    }

    //Отличается от метода выше сигнатурой.
    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition) //Получить Список Допустимых Сеточных Позиция для Действий.
                                                                                            //Получить Список Допустимых целей вокруг позиции Юнита
                                                                                            //В аргумент передаем сеточную позицию Юнита                                                                                            
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++) // Юнит это центр нашей позиции с координатами unitGridPosition, поэтому переберем допустимые значения в условном радиусе _maxShootDistance
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // Смещенная сеточная позиция. Где началом координат(0,0) является сам юнит 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition; // Тестируемая Сеточная позиция

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // Проверим Является ли testGridPosition Допустимой Сеточной Позицией если нет то переходим к след циклу
                {
                    continue; // continue заставляет программу переходить к следующей итерации цикла 'for' игнорируя код ниже
                }
                // Для области выстрела сделаем ромб а не квадрат
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // Сумма двух положительных координат сеточной позиции
                if (testDistance > _maxShootDistance) //Получим фигуру из ячеек в виде ромба // Если юнит в (0,0) то ячейка с координатами (5,4) уже не пройдет проверку 5+4>7
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) // Исключим сеточное позицию где нет юнитов (нам нужны ячейки с юнитами мы будем по ним шмалять)
                {
                    // Позиция сетки пуста, нет Юнитов
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);   // Получим юнита из нашей тестируемой сеточной позиции 
                                                                                                // GetUnitAtGridPosition может вернуть null но в коде выше мы исключаем нулевые позиции, так что проверка не нужна
                if (targetUnit.IsEnemy() == _unit.IsEnemy()) // Если тестируемый юнит враг и наш юнит тоже враг то (если они оба в одной команде то будем игнорировать этих юнитов)
                {
                    // Оба подразделения в одной "команде"
                    continue;
                }

                // ПРОВЕРИМ НА ПРОСТРЕЛИВАЕМОСТЬ до цели
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition); // Переведем в мировые координаты переданную нам сеточную позицию Юнита  
                Vector3 shototDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized; //Нормализованный Вектор Направления стрельбы
                
                float unitShoulderHeight = 1.7f; // Высота плеча юнита, в дальнейшем будем реализовывать приседание и половинчатые укрытия
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shototDirection,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
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

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition); // Получим юнита в которого целимся и сохраним его

        _state = State.Aiming; // Активируем состояние Прицеливания 
        float aimingStateTime = 1f; // Для избежания магических чисель введем переменную  Продолжительность Состояния Прицеливания
        _stateTimer = aimingStateTime;

        _canShootBullet = true;

        ActionStart(onActionComplete); // Вызовим базовую функцию СТАРТ ДЕЙСТВИЯ и передадим делегат // Вызываем этот метод в конце после всех настроек т.к. в этом методе есть EVENT и он должен запускаться после всех настроек
    }

    public Unit GetTargetUnit() // Раскроем _targetUnit
    {
        return _targetUnit;
    }

    public int GetMaxShootDistance() // Раскроем _maxShootDistance
    {
        return _maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //Получить действие вражеского ИИ  для переданной нам сеточной позиции// Переопределим абстрактный базовый метод //EnemyAIAction создан в каждой Допустимой Сеточнй Позиции, наша задача - настроить каждую ячейку в зависимости от состоянии юнита который там стоит
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition); // Получим юнита для этой позиции это наша цель

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            //actionValue = 100 +Mathf.RoundToInt(1- targetUnit.GetHealthNormalized()) *100,  // Реализуем логику для стрельбы по самому поврежденному игроку .
            // Например если юнит полностью здоров то GetHealthNormalized() вернет 1  тогда (1-1)*100 = 0 в итоге actionValue останеться прежним 100
            // но если осталось половину жизни то GetHealthNormalized() вернет 0,5 тогда (1-0,5)*100 = 50 и actionValue станет равным 150 более высокая значимость действия 
            // ЛОГИКА НЕ РАБОТАЕТ КОГДА У ЮНИТОВ РАЗНОЕ МАКСИМАЛЬНОЕ ЗДОРОВЬЕ
            // Реализую другую логику
            actionValue = 100 + Mathf.RoundToInt(AttackScore(targetUnit))
        };
    }

    public float AttackScore(Unit unit) // Оценка атаки // В первую очередь Бьем слабых, но потом при равных значениях здоровья, добиваем тех кто был самым сильным
    {
        int health = unit.GetHealth();
        int healthMax = unit.GetHealthMax();

        float unitPerHealthPoint = 100 / healthMax;  //чем выше здоровье, тем ниже будет этот балл
        return (healthMax - health) * unitPerHealthPoint + unitPerHealthPoint; // Итак… если у всех целей максимальное здоровье (health=healthMax), то БОЛЬШЕ очков получит юнит с меньшим максимальным здоровьем (самому хилому наваляют в первую очередь).
                                                                               // Если после повреждений у двух юнитов одинаковое здоровье например 20 но у первого healthMax=100 а у второго 120 то наваляют второму т.к. у него больше максимальное здоровье и он получит больше очков значения
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) // Получить Количество Целей На Позиции
    {
        return GetValidActionGridPositionList(gridPosition).Count; // Получим количество целей из списка Допустимых целей
    }
}
