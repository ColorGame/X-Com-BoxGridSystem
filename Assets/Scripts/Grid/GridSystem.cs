using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystem<TGridObject>  // Сеточная система // Стандартный класс C#// Будем использовать конструктор для создания нашей сетки поэтому он не наследует MonoBehaviour/
                                      //<TGridObject> - Generic, для того чтобы GridSystem могла работать не только с GridObject но и с др. передоваемыми ей типами Объектов Сетки
                                      // Generic - позволит исользовать часть кода GridSystem для ПОИСКА пути (при этом нам не придется дублировать код и делать похожий класс)

{

    private int _width;     // Ширина
    private int _height;    // Высота
    private float _cellSize;// Размер ячейки
    private TGridObject[,] _gridObjectArray; // Двумерный массив объектов сетки


    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition,TGridObject>  createGridObject)  // Конструктор
                                                                                                                                        // Func - это встроенный ДЕЛЕГАТ (третий параметр в аргументе это тип<TGridObject> который возвращает наш делегат и назавем его createGridObject)
    {
        _width = width; // если бы мы назвали не _width а width то писали код так // this.width = width;
        _height = height;
        _cellSize = cellSize;

        _gridObjectArray = new TGridObject[width, height]; // создаем массив сетки определенного размером width на height
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                _gridObjectArray[x, z] = createGridObject (this, gridPosition); // Вызовим делегат createGridObject и в аргумент передадим нашу GridSystem и позиции сетки. Сохраняем его в каждой ячейким сетки в двумерном массив где x,z это будут индексы массива.

                // для теста                
                //Debug.DrawLine(GetWorldPosition(_gridPosition), GetWorldPosition(_gridPosition) + Vector3.right* .2f, Color.white, 1000); // для теста нарисуем маленькие линии в центре каждой ячейки сетки
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) // Получить мировое положение
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) // Получить сеточное положение (положение относительно нашей созданной сетки)
    {
        return new GridPosition
            (
            Mathf.RoundToInt(worldPosition.x / _cellSize),  // Применяем Mathf.RoundToInt для преоброзования float в int
            Mathf.RoundToInt(worldPosition.z / _cellSize)
            );
    }

    public void CreateDebugObject(Transform debugPrefab) // Создать объект отладки ( public что бы вызвать из класса Testing и создать отладку сетки)   // Тип Transform и GameObject взаимозаменяемы т.к. у любого GameObject есть Transform и у каждого Transform есть прикрипленный GameObject
                                                         // В основном для работы нам нужен Transform игрового объекта. Если в аргументе указать тип GameObject, тогда в методе, если бы мы хотели после создани GameObject изменить его масштаб, нам придется делать дополнительный шаг "debugGameObject.Transform.LocalScale..."
                                                         // Поэтому для краткости кода в аргументе указываем тип Transform.
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z); // Позиция сетке

                Transform debugTransform =GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);  // Созданим экземпляр отладочного префаба(debugPrefab) в каждой ячейки сетки // Т.к. нет расширения MonoBehaviour мы не можем напрямую использовать Instantiate только через GameObject.Instantiate
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>(); // У созданного объкта возьмем компонент GridDebugObject
                gridDebugObject.SetGridObject(GetGridObject(gridPosition)); // Вызываем медот SetGridObject() и передаем туда объекты сетки находящийся в позиции _gridPosition // GetGridObject(_gridPosition) as GridObject - временно определим <TGridObject> как GridObject

                // debugTransform.GetComponentInChildren<TextMeshPro>().text = _gridPosition.ToString(); // Это тестовое задание для отображения координат внутри сетки( но лучше игроку не показывать debugPrefab) и делать через GridDebugObject
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition) // Вернет объекты которые находятся в данной позиции сетки .Сделаем публичной т.к. будем вдальнейшем вызывать из вне.
    {
        return _gridObjectArray[gridPosition.x, gridPosition.z]; // x,z это индексы массива по которым можем вернуть данные массива
    }

    public bool IsValidGridPosition(GridPosition gridPosition) // Является ли Допустимой Сеточной Позицией
    {
        return  gridPosition.x >=0 && 
                gridPosition.z >=0 && 
                gridPosition.x<_width && 
                gridPosition.z<_height; 
        // Проверяем что переданные нам значения больше 0 и меньше ширины и высоты нашей сетки
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public float GetCellSize()
    {
        return _cellSize;
    }

}
