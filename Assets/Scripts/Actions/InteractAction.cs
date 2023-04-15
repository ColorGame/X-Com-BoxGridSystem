using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class InteractAction : BaseAction // �������� ��������������
{

    private int _maxInteractDistance = 1; // ��������� ��������������


    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }
        
        //��� �� �� �� ������ ��������� � ����� ��������� ����� �� ���������� ��������� �������� - ������� �������� ��� ��������
        //����� �������� ������ ������� ����� ������� �� ����� ����� ����� �� ��� ����������� ������
        //ActionComplete(); //�������� ���������

    }

    public override string GetActionName() // ������� ��� ��� ������
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //�������� �������� ���������� ��  ��� ���������� ��� �������� �������// ������������� ����������� ������� ����� //EnemyAIAction ������ � ������ ���������� ������� �������, ���� ������ - ��������� ������ ������ � ����������� �� ��������� ����� ������� ��� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList() // �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������  
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>(); 

        GridPosition unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxInteractDistance; x <= _maxInteractDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxInteractDistance
        {
            for (int z = -_maxInteractDistance; z <= _maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);  // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;  // ����������� �������� �������

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue;
                }

                /*//�������� ����������� �������� ������� �� ������� �����
                DoorInteract door = LevelGrid.Instance.GetDoorAtGridPosition(testGridPosition);

                if (door == null)
                {
                    // � ���� ������� ����� ��� �����
                    continue;
                }*/
                // �������� ��������� �������������� ��� �� �� ����� ���������������� �� ������ � ������
                IInteractable interactable  = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

                if (interactable == null)
                {
                    // � ���� ������� ����� ��� ������� ��������������
                    continue;
                }
                

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) // ������������� TakeAction (��������� �������� (�����������). (������� onActionComplete - �� ���������� ��������). � ����� ������ �������� �������� ������� ClearBusy - �������� ���������
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition); // ������� IInteractable(��������� ��������������) �� ���������� �������� ������� // ��� ��� ������� ����� ������ �� ������� (�����, �����, ������...) - ��� �� �� ���������� ���� ���������

        interactable.Interact(OnInteractComplete); //���������� �������������� � ���������� IInteractable(��������� ��������������) � ��������� ������ - ��� ���������� �������������� (���� ������� ����� �������� ���� �����)

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    private void OnInteractComplete() //��� ���������� ��������������
    {
        ActionComplete(); //�������� ���������
        //��� �� �� �� ������ ��������� � ����� ��������� ����� �� ���������� ��������� �������� - ������� �������� ��� ��������
        //����� �������� ������ ������� ����� ������� �� ����� ����� ����� �� ��� ����������� ������
        
    }

}

