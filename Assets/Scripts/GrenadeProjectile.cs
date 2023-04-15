using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// В префабе гранаты у TRAIL незабудь поставить галочку Autodestruct
public class GrenadeProjectile : MonoBehaviour // Гранатный снаряд
{

    public static event EventHandler OnAnyGrenadeExploded; // static - обозначает что event будет существовать для всего класса не зависимо от того скольго у нас созданно гранат. Поэтому для прослушивания этого события слушателю не нужна ссылка на какую-либо конкретную гранату, они могут получить доступ к событию через класс, который затем запускает одно и то же событие для каждой гранаты. 
                                                           // Мы запустим событие Event когда Любая граната взорволась

    [SerializeField, Min(0.1f)] private float _moveSpeed = 15f; // Скорость перемещения 
    [SerializeField, Min(0)] private int _damageAmount = 30; // Величина урона
    [SerializeField, Min(0)] private int _damageRadiusInCells = 1; // Радиус повреждения в Ячейках сетки (отсчитывается от центра, если хотим что бы взрыв распростронялся на одну ячейку от центральной то радиус должен = 1,5 (0,5 это половина центральной ячейки halfCentralCell - будем прибавлять отдельно) (если хотим распространить взрыв на 2 ячейки не считая центра то радиус = 2,5 ячейки. Для 3 ячеек радиус 3,5)
    [SerializeField] private AnimationCurve _damageMultiplierAnimationCurve; //Анимацтонная кривая множителя повреждения

    [SerializeField] private Transform _grenadeExplosionVfxPrefab; // в инспекторе закинуть систему частиц (искры от гранаты) //НЕЗАБУДЬ ПОСТАВИТЬ ГАЛОЧКУ У TRAIL самоуничтожение(Destroy) после проигрывания
    [SerializeField] private TrailRenderer _trailRenderer; // в инспекторе закинуть трэил гранаты он лежит в самой пули // у TRAIL незабудь поставить галочку Autodestruct
    [SerializeField] private AnimationCurve _arcYAnimationCurve; // Анимацтонная кривая для настройки дуги полета гранаты

    private Vector3 _targetPosition;//Позиция цели
    private Vector3 _moveDirection; //Вектор направление движения гранаты. Для оптимизации вычислим один раз т.к. она не меняется и будем использовать в Update()
    private float _totalDistance;   //Вся дистанция. Дистанция до цели (между гранатой и целью). Для оптимизации вычислим один раз, а в Update() для вычисления текущего растояния до цели будем отнимать от _totalDistance проиденый за кадр шаг moveStep (Vector3.Distance-затратный метод)
    private float _currentDistance; //Текущее расстояние до цели
    private Vector3 _positionXZ;    //Переменная котороя хранит позицию по оси X (Y-будем менять анимационной кривой)


    private Action _onGrenadeBehaviorComplete;  //(На Гранате Действие ЗАвершено)// Объявляю делегат в пространстве имен - using System;
                                                //Сохраним наш делегат как обыкновенную переменную (в ней будет храниться функия которую мы передадим).
                                                //Action- встроенный делегат. Есть еще встроен. делегат Func<>. 
                                                //СВОЙСТВО Делегата. После выполнения функции в которую мы передали делегата, можно ПОЗЖЕ, в определенном месте кода, выполниться сохраненный делегат.
                                                //СВОЙСТВО Делегата. Может вызывать закрытую функцию из другого класса

    private void Update()
    {
        float moveStep = _moveSpeed * Time.deltaTime; // Шаг перемещения за кадр

        _positionXZ += _moveDirection * moveStep; // Переместим снаряд по оси Х на один шаг

        _currentDistance -= moveStep; // Текущее растояние до цели. От изначальной дистанции каждый кадр будем отнимать пройденый шаг
        float currentDistanceNormalized = 1 - _currentDistance / _totalDistance;//Шкалу времени AnimationCurve (горизонтальная ось) заменим на нормализованное растояние до цели и сделаем инверсию(от 1 отнимем полученное значение). _currentDistance<=_totalDistance поэтому значение будет от 0 до 1.
                                                                                //В начале полета гранаты _currentDistance = _totalDistance, тогда currentDistanceNormalized = 1(это значение оси времени в анимационной кривой), при 1 нам вернется значение positionY конца графика а нам нужно в начале значение в момент времени 0 поэтому сделаем ИНВЕРСИЮ)
        float maxHeight = _totalDistance / 4f;// Высота полета гранаты сделаем зависимой от дальности полета (что бы при коротких бросках полет выглядел естественее) //НУЖНО НАСТРОИТЬ//
        float positionY = _arcYAnimationCurve.Evaluate(currentDistanceNormalized) * maxHeight; // Получим позицию У из анимационной кривой  и умножим на макс высоту полета

        transform.position = new Vector3(_positionXZ.x, positionY, _positionXZ.z); //Переместим гранату с учетом анимационной кривой

        float reachedTargetDistance = 0.2f; // достигнутое целевое расстояние
        if (_currentDistance < reachedTargetDistance) // Если граната достаточно близко то ШАРАХНЕМ ЕЮ
        {
            float halfCentralCell = 0.5f; // Половина центральной ячейки
            float damageRadius = (_damageRadiusInCells + halfCentralCell) * LevelGrid.Instance.GetCellSize(); // Радиус повреждения от гранаты = Радиус повреждения в Ячейках сетки(с учетом центральной ячейки) * размер ячейки
            Collider[] colliderArray = Physics.OverlapSphere(_targetPosition, damageRadius); //В зоне взрыва - вычислим и сохраним массив со всеми коллайдерами, соприкасающимися со сферой или находящиеся внутри нее.

            foreach (Collider collider in colliderArray)  // переберем массив колайдеров
            {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit))//У объекта к которому прикриплен collider ПОПРОБУЕМ получить компонент Unit // Если мы используете ключевое слово "out", то функция должна установить значение для этой переменной
                                                                        // TryGetComponent - возвращает true, если компонент< > найден.Возвращает компонент указанного типа, если он существует.
                {
                    /*//1// СПОСОБ УРОН НЕ ЗАВИСИТ ОТ РАССТОЯНИЯ
                    targetUnit.Damage(_damageAmount);
                    //1//*/

                    //2// СПОСОБ УРОН ЗАВИСИТ ОТ РАССТОЯНИЯ
                    float distanceToUnit = Vector3.Distance(targetUnit.GetWorldPosition(), _targetPosition); // Растояние от центра взрыва до юнита который попал в радиус взрыва
                    float distanceToUnitNormalized = distanceToUnit / damageRadius; // Шкалу времени AnimationCurve (горизонтальная ось) заменим на нормализованное растояние до юнита (distanceToUnit<=damageRadius поэтому значение будет от 0 до 1. Если Юнит находиться центре взрыва то distanceToUnit =0 тогда distanceToUnitNormalized тоже = 0, тогда анимационный график вернет значение вертикальной оси в нулевой момент времени это значение будет =1)
                    int damageAmountFromDistance = Mathf.RoundToInt(_damageAmount * _damageMultiplierAnimationCurve.Evaluate(distanceToUnitNormalized)); //Величина повреждения от растояния. Округлим до целого и переведем в int т.к. Damage() принимает целые числа

                    targetUnit.Damage(damageAmountFromDistance); // применим урон к юниту попавшему в радиус взрыва
                    //2//
                }

                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))   //У объекта к которому прикриплен collider ПОПРОБУЕМ получить компонент DestructibleCrate // Если мы используете ключевое слово "out", то функция должна установить значение для этой переменной
                                                                                                            // TryGetComponent - возвращает true, если компонент< > найден.Возвращает компонент указанного типа, если он существует.
                {
                    destructibleCrate.Damage(); // Если есть ящик разрушим его // ЗДЕСЬ МОЖНО РЕАЛИЗОВАТЬ ИНТЕРФЕЙС РАЗРУШЕНИЯ что бы граната могла разрушать все объекты которые реализуют этот интерфейс
                }

            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);// Вызовим событие

            _trailRenderer.transform.parent = null; // Отсоеденим трэйл от родителя что бы он еще жил. А в инсепкторе поставим галочку Autodestruct - уничтожение после завершения ортрисовки

            Instantiate(_grenadeExplosionVfxPrefab, _targetPosition, Quaternion.LookRotation(Vector3.up)); //Создадим частьички взрыва . Развернем что бы ось Z смотрела вверх т.к. у нас область взрыва это полусфера

            Destroy(gameObject);

            _onGrenadeBehaviorComplete(); // Вызовим сохраненный делегат который нам передала функция Setup(). В нашем случае это ActionComplete() он снимает занятость с кнопок UI
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviorComplete) // Настройка гранаты. В аргумент передаем целевую позицию  В аргумент будем передовать делегат типа Action (onGrenadeBehaviorComplete - На Гранате Действие ЗАвершено)
    {
        _onGrenadeBehaviorComplete = onGrenadeBehaviorComplete; // Сохраним полученый делегат

        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition); // Получим целевую позицию из переданной нам позиции сетки

        // Предварительные вычисления для оптимизации 

        _positionXZ = transform.position; // Сохраним текущую позиции по оси Х при этом обнулим У состовляющию
        _positionXZ.y = 0;

        _totalDistance = Vector3.Distance(_positionXZ, _targetPosition);  //Вычислим дистанцию между гранатой и целью (чтобы не вычеслять каждый кадр в update)
        _currentDistance = _totalDistance; // Текцщие расстояние в начале равно всему расстоянию

        _moveDirection = (_targetPosition - _positionXZ).normalized; //Вычислим вектор Направление движения гранаты (чтобы не вычеслять каждый кадр в update т.к. оно не меняется)
    }
    public int GetDamageRadiusInCells() //Раскроем _damageRadiusInCells
    {
        return _damageRadiusInCells;
    }

    /*#if UNITY_EDITOR //Зависящая от платформы компиляция. Позволяют разбивать наш скрипт на части для компиляции и выполнения части кода исключительно для одной из поддерживаемых платформ.
        private void OnDrawGizmos() // Для рисования вспогательных объектов в сцене, в нашем случае круг действия гранаты
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(_targetPosition, Vector3.up , _damageRadiusInCells * LevelGrid.Instance.GetCellSize(), 4f);
        }
    #endif // При создании билда этот кусок кода не будет в него включаться а будет работать только в EDITOR(редактор)*/

}
