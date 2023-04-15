using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Пространство имен для textMeshPro

public class GridDebugObject : MonoBehaviour // Отладка объекта сетки (Отображение сеточных координат и список юнитов в ячейке)
{

    [SerializeField] private TextMeshPro _textMeshPro; // В инспекторе перетащить текстовый дочерний объект префаба

    private object _gridObject; // Создадим переменную для хранения объекта сетки // Вместо типа GridObject напишем object - Универсальный тип что бы работать с другими типами объектов

    public virtual void SetGridObject(object gridObject) // Устаноить объект сетки // virtual- если понадобиться переопределить где нибудь 
    {
        _gridObject = gridObject; // Установим переданный нам объект как Сеточный объект
    }

    protected virtual void Update() // В конечной игре обновлять содержимое сетки лучше через Event когда юнит входит или выходит из ячейки сетки
    {
        _textMeshPro.text = _gridObject.ToString(); // В Update будем обновлять визуальное отображение координат сетки
    }
}
