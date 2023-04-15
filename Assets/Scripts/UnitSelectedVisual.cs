using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour // ������������ ������ �����
{
    [SerializeField] private Unit _unit; // ���� � �������� ���������� ������ �����.

    private MeshRenderer _meshRenderer; // ����� �������� � ����. MeshRenderer ��� �� ������ ��� �������� ��� ���������� ������

    private void Awake() //��� ��������� ������ Awake() ����� ������������ ������ ��� ������������� � ���������� ��������
    {
        _meshRenderer = GetComponent<MeshRenderer>(); 
    }

    private void Start() // � � ������ Start() ������������ ��� ������������� � ��������� ������� ������
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // ������������� �� Event �� UnitActionSystem (���������� �����������). ���������� ��� �� ��������� ������� UnitActionSystem_OnSelectedUnitChanged()
                                                                                                   // ����� ����������� ������ ��� ����� �� ������ ���������� �����.
        UpdateVisual(); // ��� �� ��� ������ ������ ��� ������� ������ � ���������� ������
    }

    // ����� ����� ������� ����� ��� � Event
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty) //sender - ����������� // �������� ������ ����� ���� ��������� ��� � ������� ����������� OnSelectedUnitChanged
    {
        UpdateVisual();
    }

    private void UpdateVisual() // (���������� �������) ��������� � ���������� ������������ ������.
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == _unit) // ���� ��������� ���� ����� ����� �� ������� ����� ���� ������ ��
        {
            _meshRenderer.enabled = true; // ������� ����
        }
        else
        {
            _meshRenderer.enabled = false; // �������� ����
        }
    }

    private void OnDestroy() // ���������� ������� � MonoBehaviour � ���������� ��� ����������� �������� �������
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged; // �������� �� ������� ����� �� ���������� ������� � ��������� ��������.(���������� MissingReferenceException: ������ ���� 'MeshRenderer' ��� ���������, �� �� ��� ��� ��������� �������� � ���� ������.)
    }
}
