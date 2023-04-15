using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // ��� ������ � ���������������� �����������

public class UnitActionSystemUI : MonoBehaviour // ������� �������� UI ����� // ����������� ��������� ������ ��� ������ ����� // ����� � Canvas
{

    [SerializeField] private Transform _actionButtonPrefab; // � ���������� ������� ������ ������
    [SerializeField] private Transform _actionButtonContainerTransform; // � ���������� ���������  ��������� ��� ������( ���������� � ����� � Canvas)
    [SerializeField] private TextMeshProUGUI _actionPointsText; // ������ �� ����� �����

    private List<ActionButtonUI> _actionButtonUIList; // ������ ������ ��������

    private void Awake()
    {
        _actionButtonUIList = new List<ActionButtonUI> (); // �������� ��������� ������
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged; // ������������� �� Event �� UnitActionSystem (���������� �����������). ���������� ��� �� ��������� ������� UnitActionSystem_OnSelectedUnitChanged()
                                                                                                   // ����� ����������� ������ ��� ����� �� ������ ���������� �����. //OnSelectedUnitChanged - ��������� ���� �������
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged; // ������������� �� Event// ����� ����������� ������ ��� ����� �� ������ ������� �������� // ��������� �������� ��������

        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted; // ������������� �� Event// ����� ����������� ������ ��� ��� ������ ��������. // �������� ������

        //2//3//{ ��� ��������� �������� ������ ������ ����� ����� ���������
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged; // ������������ �� Event � �������� UnitActionSystem_OnBusyChanged, ��� ������� ������� �� ������� ������� �������� // ��������� ��������
        //2//3//}
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // ������������� �� Event // ����� ����������� (��������� ������ ����� ��������) ������ ��� ����� ������� ����� ����.
        // ������� 2 //{
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged; //������������� �� ����������� Event // ������ ����������� ������ ��� ��� ��������� ����� �������� � ������(Any) ����� � �� ������ � ����������.
        // ������� 2 //}

        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    /*//2//{ ������ ������ ������ ������ ����� ����� ���������
    private void Show() // ��������
    {
        gameObject.SetActive(true);
    }

    private void Hide() // ������
    {
        gameObject.SetActive(false);
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy) // ���� ����� �� ������ ���� ��� �� �������� ������
        {
            Hide();
        }
        else
        {
            Show();
        }
    } //2//}*/

    //3//{ ������ ������ ������ ������ ����� ����� ���������
    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList) // � ����� ���������� ��������� ������
        {
            actionButtonUI.HandleStateButton(isBusy);
        }


    } //3//}


    private void CreateUnitActionButtons() // ������� ������ ��� �������� ����� 
    {
        foreach (Transform buttonTransform in _actionButtonContainerTransform) // ������� ��������� � ��������
        {
            Destroy (buttonTransform.gameObject); // ������ ������� ������ ������������� � Transform
        }

        _actionButtonUIList.Clear (); // ������� ����� ������

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit(); // ������� ���������� �����

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionsArray()) // � ����� ��������� ������ ������� �������� � ���������� �����
        {
            Transform actionButtonTransform = Instantiate(_actionButtonPrefab, _actionButtonContainerTransform); // ��� ������� baseAction �������� ������ ������ � �������� �������� - ��������� ��� ������
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>(); // � ������ ������ ��������� ActionButtonUI
            actionButtonUI.SetBaseAction(baseAction); //������� � ��������� ������� �������� (����� ������)
            
            _actionButtonUIList.Add (actionButtonUI); // ������� � ������ ���������� ��������� ActionButtonUI
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty) //sender - ����������� // �������� ������ ����� ���� ��������� ��� � ������� ����������� OnSelectedUnitChanged
    {
        CreateUnitActionButtons(); // ������� ������ ��� �������� ����� 
        UpdateSelectedVisual(); 
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs empty)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs empty) // �������� �������� - ��� �������� ��� ���� ��� ��������� � ���� �� ��������
    {
        UpdateActionPoints();
    }
    private void UpdateSelectedVisual() //���������� ������������ ������( ��� ������ ������ ������� �����)
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints() // ���������� ����� �������� (��� �������� ��������)
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();// ������� ���������� �����
        
        _actionPointsText.text = "Action Points; " + selectedUnit.GetActionPoints(); //������� ����� ������� � ���� ���������� �����
    }

    // �������� // ����� ���������� ������. ����� ����� � ������ Unit � ���������� ������ ����� � ���� ������, ������� ���� � ���� �������. ��� ����������� ����� ��� ������ ����������, ����� ����� ���������� ������ � ���������� ��� �� ���������� ���� �������� "0" � �� ����� �� "2".
    // ������� 1 //- �������� ������� ���������� ������� UnitActionSystemUI , ������� � Project Settings/ Script Execution Order � �������� ���� Deafault Time � �����
    private void TurnSystem_OnTurnChanged(object sender, EventArgs empty) // ����� ���� ������� - ��� �������� ��� ���� �������� ��������������, ������� ��.
    {
        UpdateActionPoints();

        // ����� ����� ����������� ���������� ������ �� ����� ���� �����
    }

    // ������� 2 //{
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs empty) //��������� ��������� ����� �������� � ������(Any) ����� � �� ������ � ����������. ������� ��.
    {
        UpdateActionPoints();
    }// ������� 2 //}
}
