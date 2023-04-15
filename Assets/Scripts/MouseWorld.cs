using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseWorld : MonoBehaviour // Класс отвечающий за положение курсора мыши
{

    public static MouseWorld Instance { get; private set; }   //(ПАТТЕРН SINGLETON) Это свойство которое может быть заданно (SET-присвоено) только этим классом, но может быть прочитан GET любым другим классом
                                                              // instance - экземпляр, У нас будет один экземпляр MouseWorld можно сдел его static. Instance нужен для того чтобы другие методы, через него, могли подписаться на Event.

    public static event EventHandler OnMouseGridPositionChanged; // Событие позиция мыши на сетке изменилось

    [SerializeField] private LayerMask _mousePlaneLayerMask; // маска слоя плоскости мыши (появится в ИНСПЕКТОРЕ)

    private GridPosition _mouseGridPosition;

    private void Awake()
    {
        // Если ты акуратем в инспекторе то проверка не нужна
        if (Instance != null) // Сделаем проверку что этот объект существует в еденичном екземпляре
        {
            Debug.LogError("There's more than one MouseWorld!(Там больше, чем один MouseWorld!) " + transform + " - " + Instance);
            Destroy(gameObject); // Уничтожим этот дубликат
            return; // т.к. у нас уже есть экземпляр MouseWorld прекратим выполнение, что бы не выполнить строку ниже
        }
        Instance = this;
    }

    private void Start()
    {
        _mouseGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());  // Установим при старте сеточную позицию мышм // НЕЛЬЗЯ ВЫЗЫВАТЬ в Awake() т.к. в результате гонки возникает нулевая ошибка (кто проснется раньше InputManager или MouseWorld неизвестно)
    }

    // Для теста, светящий шар следует за курсором мыши.
    /*private void Update()
    {
        transform.position = MouseWorld.GetPosition(); // Так можно вызывать из ЛЮБОГО МЕСТА
    }*/

    private void Update()
    {
        GridPosition newMouseGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());
        if (_mouseGridPosition != newMouseGridPosition)
        {
            _mouseGridPosition = newMouseGridPosition;
            OnMouseGridPositionChanged?.Invoke(this, EventArgs.Empty); // Создадим событие
        }
    }

    public static Vector3 GetPosition() // Получить позицию (static обозначает что метод принадлежит классу а не кокому нибудь экземпляру)
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition()); // Луч от камеры в точку на экране где находиться курсор мыши
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance._mousePlaneLayerMask); // Instance._mousePlaneLayerMask - можно задать как смещение битов слоев 1<<6  т.к. mousePlane под 6 номером
        return raycastHit.point; // Если луч попадет в колайдер то Physics.Raycast будет true, и raycastHit.point вернет "Точку удара в мировом пространстве, где луч попал в коллайдер", а если false то можно вернуть какоенибудь другое нужное значение(в нашем случае вернет нулевой вектор).
    }


}
