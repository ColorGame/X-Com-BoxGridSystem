using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� //- �������� ������� ���������� ������� UnitManager � Unit , ������� � Project Settings/ Script Execution Order � �������� ���������� UnitManager ���� Default Time), ����� UnitManager ���������� ������ ��� �������� Unit
// (��� �� ������� ��������� ��������� � UnitManager � ����� ��������� ���� ������� � Unit)
// ����� ���� ������ Unit ��������� ������ �� ���� �� ���������� � ������ ������ �.�. ��� ��� �� ����������
public class UnitManager : MonoBehaviour // �������� (�������������) ������
{

    public static UnitManager Instance { get; private set; }   //(������� SINGLETON) ��� �������� ������� ����� ���� ������� (SET-���������) ������ ���� �������, �� ����� ���� �������� GET ����� ������ �������
                                                               // instance - ���������, � ��� ����� ���� ��������� UnitActionSystem ����� ���� ��� static. Instance ����� ��� ���� ����� ������ ������, ����� ����, ����� ����������� �� Event.

    public static event EventHandler OnAnyUnitDeadAndRemoveList; // ������� ����� ���� ���� � ������ �� ������

    private List<Unit> _unitList;       // ������ ������ (�����)
    private List<Unit> _friendlyUnitList;// ������������� ������ ������
    private List<Unit> _enemyUnitList;  // ��������� ������ ������


    private void Awake()
    {
        // ���� �� �������� � ���������� �� �������� �� �����
        if (Instance != null) // ������� �������� ��� ���� ������ ���������� � ��������� ����������
        {
            Debug.LogError("There's more than one UnitManager!(��� ������, ��� ���� UnitManager!) " + transform + " - " + Instance);
            Destroy(gameObject); // ��������� ���� ��������
            return; // �.�. � ��� ��� ���� ��������� UnitManager ��������� ����������, ��� �� �� ��������� ������ ����
        }
        Instance = this;

        // �������� ������������� �������
        _unitList = new List<Unit>();
        _friendlyUnitList = new List<Unit>();
        _enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned; // ���������� �� ������� (����� ���������(���������) ����)  // ��������� � �������� �������� �� ������������� �� ������� ������� ����������� ���� � ������ � ��� ������ ��� ������  (������� ������ � �����)
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;  // ���������� �� ������� (����� ������� ����)
    }

    private void Unit_OnAnyUnitSpawned(object sender, System.EventArgs e) // ��� �������� ������ ����������� �� �� �������
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        //Debug.Log(unit + " spawner"); // ��� �����

        _unitList.Add(unit); // ������� ����� � ����� ������

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _enemyUnitList.Add(unit); // ������� ��� � ������ ��������� ������
        }
        else// ���� ���
        {
            _friendlyUnitList.Add(unit); // ������� ��� � ������ ������������� ������
        }

    }
    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit; // (Unit)sender - ������ ������// ������� ����� ������� �������� ������������

        //Debug.Log(unit + " dead"); // ��� �����

        _unitList.Remove(unit); // ������� ����� �� ������ ������

        if (unit.IsEnemy()) // ���� ����������� ���� �� ...
        {
            _enemyUnitList.Remove(unit); // ������ ��� �� ������ ��������� ������
        }
        else// ���� ���
        {
            _friendlyUnitList.Remove(unit); // ������ ��� �� ������ ������������� ������
        }

        OnAnyUnitDeadAndRemoveList?.Invoke(this, EventArgs.Empty); // �������� ��������
    }

    public List<Unit> GetUnitList() // ������� ������ ��� ������
    {
        return _unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return _friendlyUnitList;
    }


    public List<Unit> GetEnemyUnitList()
    {
        return _enemyUnitList;
    }


}
