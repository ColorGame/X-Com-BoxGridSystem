using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject // Объект сетки (В будущем будет содержать список всех юнитов которые находятся в позиции сетки) 
                        // Стандартный класс C#// Будем использовать конструктор для создания нашей сетки поэтому он не наследует MonoBehaviour/
                        // GridObject создается в каждой ячейки сетки. Является оболочкой для хранения Юнитов
{

    private GridSystem<GridObject> _gridSystem; // Сеточная система .В дженерик предаем тип GridObject// Частная сеточная система которая создала этот объект (это расширение например для сетки 2-го этажа)
    private GridPosition _gridPosition; // Положение объекта в сетке
    private List<Unit> _unitList; // Список юнитов. Чтобы GridObject мог содержать в себе несколько юнитов
    private IInteractable _interactable; // IInteractable(интерфейс взаимодействия) (будет частью GridObject как и юнит) ИНТЕРФЕЙС позволяет взаимодействовать с любым объектом (дверь, сфера, кнопка...) - который реализует этот интерфейс

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition) // Конструктор 
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _unitList = new List<Unit>();
    }

    public override string ToString() // Переопределим ToString(). Чтобы она возвращала позицию в сетке и юнита в этой ячейке (позже можно расширить диапазон возвращаемых данных)
    {
        string unitSting = ""; // Начнем с пустой строки
        foreach (Unit unit in _unitList) // Переберем юнитов из списка и Добавим их к нашей строке
        {
            unitSting += unit + "\n"; //"\n" -перенос на новую строку
        }
        return _gridPosition.ToString() + "\n" + unitSting;
    }

    public void AddUnit(Unit unit) // Добавить юнита в список
    {
        _unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit) // Удалить юнита из списка
    {
        _unitList.Remove(unit);
    }

    public List<Unit> GetUnitList() // Получить список юнитов
    {
        return _unitList;
    }

    public bool HasAnyUnit() //Есть ли какойнибудь юнит в данном экземпляре GridObject
    {
        return _unitList.Count > 0; // Вернет истину если в списке есть хотябы один юнит
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit()) // Если Есть какойнибудь юнит в данном экземпляре GridObject
        {
            return _unitList[0]; // Вернем первого юнита из списка
        }
        else
        {
            return null;
        }
    }

    public IInteractable GetInteractable() // Получить Интерфейс Взаимодействия
    {
        return _interactable;
    }

    public void SetInteractable(IInteractable interactable) // Установить Интерфейс Взаимодействия
    {
        _interactable = interactable;
    }

    public void ClearInteractable() // Очистить Интерфейс Взаимодействия
    {
        _interactable = null;
    }
}
