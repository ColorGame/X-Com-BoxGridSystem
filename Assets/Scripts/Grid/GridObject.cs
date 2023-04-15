using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject // ������ ����� (� ������� ����� ��������� ������ ���� ������ ������� ��������� � ������� �����) 
                        // ����������� ����� C#// ����� ������������ ����������� ��� �������� ����� ����� ������� �� �� ��������� MonoBehaviour/
                        // GridObject ��������� � ������ ������ �����. �������� ��������� ��� �������� ������
{

    private GridSystem<GridObject> _gridSystem; // �������� ������� .� �������� ������� ��� GridObject// ������� �������� ������� ������� ������� ���� ������ (��� ���������� �������� ��� ����� 2-�� �����)
    private GridPosition _gridPosition; // ��������� ������� � �����
    private List<Unit> _unitList; // ������ ������. ����� GridObject ��� ��������� � ���� ��������� ������
    private IInteractable _interactable; // IInteractable(��������� ��������������) (����� ������ GridObject ��� � ����) ��������� ��������� ����������������� � ����� �������� (�����, �����, ������...) - ������� ��������� ���� ���������

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition) // ����������� 
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
        _unitList = new List<Unit>();
    }

    public override string ToString() // ������������� ToString(). ����� ��� ���������� ������� � ����� � ����� � ���� ������ (����� ����� ��������� �������� ������������ ������)
    {
        string unitSting = ""; // ������ � ������ ������
        foreach (Unit unit in _unitList) // ��������� ������ �� ������ � ������� �� � ����� ������
        {
            unitSting += unit + "\n"; //"\n" -������� �� ����� ������
        }
        return _gridPosition.ToString() + "\n" + unitSting;
    }

    public void AddUnit(Unit unit) // �������� ����� � ������
    {
        _unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit) // ������� ����� �� ������
    {
        _unitList.Remove(unit);
    }

    public List<Unit> GetUnitList() // �������� ������ ������
    {
        return _unitList;
    }

    public bool HasAnyUnit() //���� �� ����������� ���� � ������ ���������� GridObject
    {
        return _unitList.Count > 0; // ������ ������ ���� � ������ ���� ������ ���� ����
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit()) // ���� ���� ����������� ���� � ������ ���������� GridObject
        {
            return _unitList[0]; // ������ ������� ����� �� ������
        }
        else
        {
            return null;
        }
    }

    public IInteractable GetInteractable() // �������� ��������� ��������������
    {
        return _interactable;
    }

    public void SetInteractable(IInteractable interactable) // ���������� ��������� ��������������
    {
        _interactable = interactable;
    }

    public void ClearInteractable() // �������� ��������� ��������������
    {
        _interactable = null;
    }
}
