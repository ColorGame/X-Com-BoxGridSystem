using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour // Визуализация выбора юнита
{
    [SerializeField] private Unit _unit; // Юнит к которому прикриплен данный визуа.

    private MeshRenderer _meshRenderer; // Будем включать и выкл. MeshRenderer что бы скрыть или показать наш визуальный объект

    private void Awake() //Для избежания ошибок Awake() Лучше использовать только для инициализации и настроийки объектов
    {
        _meshRenderer = GetComponent<MeshRenderer>(); 
    }

    private void Start() // А в методе Start() использовать для взаимодествия и получения внешних ссылок
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // подписываемся на Event из UnitActionSystem (становимся слушателями). Обозначает что мы выполняем функцию UnitActionSystem_OnSelectedUnitChanged()
                                                                                                   // Будет выполняться каждый раз когда мы меняем выбранного юнита.
        UpdateVisual(); // Что бы при старте визуал был включен только у выбранного игрока
    }

    // метод лучше назвать также как и Event
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty) //sender - отправитель // Подписка должна иметь туже сигнатуру что и функция отправителя OnSelectedUnitChanged
    {
        UpdateVisual();
    }

    private void UpdateVisual() // (Обновление визуала) Включение и выключение визуализации выбора.
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == _unit) // Если выбранный юнит равен юниту на котором лежит этот скрипт то
        {
            _meshRenderer.enabled = true; // включим круг
        }
        else
        {
            _meshRenderer.enabled = false; // выключим круг
        }
    }

    private void OnDestroy() // Встроенная функция в MonoBehaviour и вызывается при уничтожении игрового объекта
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged; // Отпишемя от события чтобы не вызывались функции в удаленных объектах.(Исключение MissingReferenceException: Объект типа 'MeshRenderer' был уничтожен, но вы все еще пытаетесь получить к нему доступ.)
    }
}
