using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject   // ����� ���� �������� ���������� ������ // �������� ����������� ������ GridDebugObject � ���� ��������� � ������ ������
                                                            //(����������� ������ ������, ��� ������� ���� G H F)
{
    [SerializeField] private TextMeshPro _gCostText; // � ���������� ���������� ��������� �������� ������ �������
    [SerializeField] private TextMeshPro _hCostText;
    [SerializeField] private TextMeshPro _fCostText;
    [SerializeField] private SpriteRenderer _isWalkableSpriteRenderer; // ������� ������������ ������������ ������

    private PathNode _pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode)gridObject; // ��������� ���������� ��� ������ ��� �������� ������ ���� PathNode
    }

    protected override void Update()
    {
        base.Update();
        _gCostText.text = "G: " + _pathNode.GetGCost().ToString();// � Update ����� ��������� ���������� ����������� ���������� ��� ������� ����
        _hCostText.text = "H: " + _pathNode.GetHCost().ToString();
        _fCostText.text = "F: " + _pathNode.GetFCost().ToString();
        _isWalkableSpriteRenderer.color = _pathNode.GetIsWalkable() ? Color.green : Color.red; // ���� ������� ��������� �� ������� � � ��������� ������ ������� �������
    }
}
