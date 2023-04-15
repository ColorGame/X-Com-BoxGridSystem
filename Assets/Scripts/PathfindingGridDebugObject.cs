using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject   // Поиск Пути Сеточный Отладочный Объект // Является расширением класса GridDebugObject и тоже создается в каждой ячейку
                                                            //(Отображение данных ячейки, для расчета пути G H F)
{
    [SerializeField] private TextMeshPro _gCostText; // В инспекторе перетащить текстовый дочерний объект префаба
    [SerializeField] private TextMeshPro _hCostText;
    [SerializeField] private TextMeshPro _fCostText;
    [SerializeField] private SpriteRenderer _isWalkableSpriteRenderer; // Квадрат отоброжающий проходимость ячейки

    private PathNode _pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode)gridObject; // Установим переданный нам объект как Сеточный объект типа PathNode
    }

    protected override void Update()
    {
        base.Update();
        _gCostText.text = "G: " + _pathNode.GetGCost().ToString();// В Update будем обновлять визуальное отображение параметров для расчета пути
        _hCostText.text = "H: " + _pathNode.GetHCost().ToString();
        _fCostText.text = "F: " + _pathNode.GetFCost().ToString();
        _isWalkableSpriteRenderer.color = _pathNode.GetIsWalkable() ? Color.green : Color.red; // Если ячейкка проходима то зеленая а в противном случае сделать красной
    }
}
