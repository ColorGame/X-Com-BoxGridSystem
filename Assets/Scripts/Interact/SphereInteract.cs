using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereInteract : MonoBehaviour, IInteractable // Взаимодействие с Сферой // Расширим класс интерфейсом 
{


    public event EventHandler OnInteractSphereActivated; //Создадим событие когда - Сфера Взаимодействия Активированна

    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private bool _isGreen; //Он зеленый (для отслеживания состояния шара)

    private GridPosition _gridPosition; // Сеточная позиция шара    
    private Action _onInteractionComplete; // Делегат Взаимодействие Завершено// Объявляю делегат в пространстве имен - using System;
                                           //Сохраним наш делегат как обыкновенную переменную (в ней будет храниться функия которую мы передадим).
                                           //Action- встроенный делегат. Есть еще встроен. делегат Func<>. 
                                           //СВОЙСТВО Делегата. После выполнения функции в которую мы передали делегата, можно ПОЗЖЕ, в определенном месте кода, выполниться сохраненный делегат.
                                           //СВОЙСТВО Делегата. Может вызывать закрытую функцию из другого класса
    private bool _isActive;
    private float _timer; // Таймер который не будет позволять непрерывно взаимодействовать с шаром



    private void Start()
    {
        if (gameObject.activeSelf) // Если шар в активном состоянии то ...
        {
            UpdateInteractableAtGridPosition(); // Обновить Взаимодействие с Сеточной позицией
        }

        if (_isGreen) // Установить визуал в соответствии с заданным параметром
        {
            SetColorGreen();
        }
        else
        {
            SetColorRed();
        };
    }

    public void UpdateInteractableAtGridPosition() // Обновить Взаимодействие с Сеточной позицией
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); // Определим сеточную позицию шара
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this); // И в полученную сеточную позицию установим наш Шар с Интерфейсом Interactable(взаимодействия)
    }



    private void Update()
    {
        if (!_isActive) // Если происходит взаимодействие с шаром то она активна и выполняется код ниже
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

    private void SetColorGreen()
    {
        _isGreen = true;
        _meshRenderer.material = _greenMaterial;
    }

    private void SetColorRed()
    {
        _isGreen = false;
        _meshRenderer.material = _redMaterial;
    }

    public void Interact(Action onInteractionComplete) // Взаимодействие. В аргумент предаем Делегат Взаимодействие Завершено 
    {
        _onInteractionComplete = onInteractionComplete; // Сохраним полученный делегат
        _isActive = true; // Переключаем Шар в Активное состояние
        _timer = 1.5f; // Задаем время активного состояния  //НУЖНО НАСТРОИТЬ//

        if (_isGreen) // При взаимодействии изменим цвет. Если шар зеленный то переключим на крассный
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
            OnInteractSphereActivated?.Invoke(this, EventArgs.Empty); // Запустим событие - Сфера Взаимодействия Активированна
        }

    }
}
