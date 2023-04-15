using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode // Узел поиска пути // Будет создается в каждой ячейки сетки, И будет хранить в себе частные данные для расчета поиска пути G H F
                      // Стандартный класс C#// Будем использовать конструктор для создания нашей сетки поэтому он не наследует MonoBehaviour
                      // класс нападоби GridObject
{

    private GridPosition _gridPosition;
    private int _gCost;  // Стоимость смещения до ближайщей ячейки. по диоганали вычисляется из теоремы пифагора. Длина гипотенузы в прямоугольном треугольнике(корень квадратный из суммы квадратов сторон)
                         // В стартовой позиции G = 0. После смещениря в другую ячейку по прямой эта величина увеличиться на G = 0 + gCost. Смещяемся еще раз, допустим по диоганале тогда G = 0 + gCost +корень квадратный из суммы квадратов-gCost  
    private int _hCost;  // Стоимость смещения до цели по кратчайшему пути (грубо говоря длина по прямой)// Чем ближе к цели тем меньше будет это число
    private int _fCost;  // Сумма g+h
    private PathNode _cameFromPathNode; // Получено из PathNode // В каждой ячейки нам нужна ссылка на ячейку откуда мы прибыли
    private bool _isWalkeble = true; // Можно ходить // Для настроики непроходимых узлов

    public PathNode(GridPosition gridPosition) // Конструктор
    {
        _gridPosition = gridPosition;
    }

    public override string ToString() // Переопределим ToString(). Чтобы она возвращала позицию в сетке и юнита в этой ячейке (позже можно расширить диапазон возвращаемых данных)
    {
        return _gridPosition.ToString();
    }

    public int GetGCost()
    {
        return _gCost;
    }

    public int GetHCost()
    {
        return _hCost;
    }

    public int GetFCost()
    {
        return _fCost;
    }

    public void SetGCost(int gCost)
    {
        _gCost = gCost;
    }

    public void SetHCost(int hCost)
    {
        _hCost = hCost;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }

    public void ResetCameFromPathNode() // Сброс Пришел С Узла Пути ( сбросим ссылку на предыдущий Узел Пути, чтобы результаты последнего пути не влияли на расчеты следующего поиска пути)
    {
        _cameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNode pathNode) // Установить пришел  С Узла Пути
    {
        _cameFromPathNode = pathNode;
    }

    public PathNode GetCameFromPathNode() // Получить пришел  С Узла Пути
    {
        return _cameFromPathNode;
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    public bool GetIsWalkable()
    {
        return _isWalkeble;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        _isWalkeble= isWalkable;
    }
}
