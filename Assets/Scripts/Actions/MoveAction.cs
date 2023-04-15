using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction // �������� ����������� ��������� ����� BaseAction // ������� � ��������� ����� // ����� �� ������ �����
{

    public event EventHandler OnStartMoving; // ����� ��������� (����� ���� ������ �������� �� �������� ������� Event)
    public event EventHandler OnStopMoving; // ��������� �������� (����� ���� �������� �������� �� �������� ������� Event)

    [SerializeField] private int maxMoveDistance = 5; // ������������ ��������� �������� � �����

    private List<Vector3> _positionList; // ������� ������� ������ ���������� ���� (� ������������ �������)
    private int _currentPositionIndex; // ������� ������� ������


    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }
        // ���� ��������� �� ������ ����� �� _positionList, ������ ��������� ������ ����� targetPosition
        Vector3 targetPosition = _positionList[_currentPositionIndex]; // ������� �������� ����� ������� �� ����� � �������� ��������
       
        Vector3 moveDirection = (targetPosition - transform.position).normalized; // ����������� ��������, ��������� ������

        float rotateSpeed = 15f; //��� ������ ��� �������
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed); // ������ �����.

        float stoppingDistance = 0.2f; // ��������� ��������� //����� ���������//
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)  // ���� ��������� ������ ��� ��������� ��������� �� ���������� ���������
        {
            float moveSpead = 4f; //����� ���������//
            transform.position += moveDirection * moveSpead * Time.deltaTime;
        }
        else // �� �������� ����
        {
            _currentPositionIndex++; // �������� ������ �� �������
            
            if(_currentPositionIndex >= _positionList.Count ) // ���� �� ����� �� ����� ������ �����...
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty); //�������� ������� ��������� ��������



                ActionComplete(); // ������� ������� ������� �������� ���������
            }            
        }
    }

    // ������������� TakeAction (��������� �������� (�����������)) // �� ������������� Move � TakeAction
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) // �������� � ������� �������. � �������� �������� �������� �������  � �������. ������� �� ��� �������� ����� ������� �������
    {
       List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(_unit.GetGridPosition(), gridPosition, out int pathLength); // ������� ������ ���� ������� ����� �� �������� ��������� ��������� ����� �� �������� (out int pathLength �������� ��� �� ��������������� ���������)

        _currentPositionIndex = 0; // �� ��������� ���������� � ����

        // ���� ������������� ���������� ������ GridPosition � ������� ���������� Vector3
        _positionList = new List<Vector3>(); // �������������� ������ �������

        foreach (GridPosition pathGridPosition in pathGridPositionList) // ��������� ���������� ������ pathGridPositionList, ����������� �� � ������� ���������� � ������� � _positionList
        {
            _positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition)); // ����������� pathGridPosition � ������� � ������� � _positionList
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ��������� 

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    public override List<GridPosition> GetValidActionGridPositionList() //�������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = _unit.GetGridPosition(); // ������� ������� � �����
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� maxMoveDistance
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                if (unitGridPosition == testGridPosition) // �������� �������� ������� ��� ���������� ��� ����
                {
                    // ���� ������ �� ������� ����� ���� :(
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ���������� ������ �����
                {
                    // ������� ������ ������ ������ :(
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) //�������� �������� ������� ��� ������ ������ (���� ����������� ����� �������)
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) //�������� �������� ������� ���� ������ ������ ��� �� ��� ���� ������ � ����� (Obstacles -�����������) (������� ����� ������ � ����������� ��������)
                {
                    continue;
                }

                int pathfindingDistanceMultiplier = 10; // ��������� ���������� ����������� ���� (� ������ Pathfinding ������ ��������� �������� �� ������ � ��� ����� ����� 10 �� ��������� 14, ������� ������� ��� ��������� �� ���������� ������)
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier) //�������� �������� ������� - ���� ��������� �� ����������� ������ ������ ���������� ������� ���� ����� ���������� �� ���� ���
                {
                    // ����� ���� ������� ������
                    continue;
                }

                validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����
                //Debug.Log(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        int targetCountAtPosition = _unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition); // � ����� ������ ������ ShootAction � ������� � ���� "�������� ���������� ����� �� �������"
                                                                                                           //� �����, ��� ����� ������� ���� �� ������ ����� �����, ������� ����� ������������ ������ � �������� ������������� ������� ������������ ������� �����. ����� �� ������ ������� ������ � ��������������� ���� � �� ��������� ���� �������� � ������ (Move � Shoot)
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtPosition * 10, //������ � ����� ������� ����������� ���������� ����� ����� � ����������. �������� ���� � ��� ���� ������� �����, � ������� ��� ���������� �����, � ������ ������� �����, � ������� ���� ���� ���������� ����, �� �������� �� ������ ������� �����, ��������� �������� �������� �������� �� ���������� ���������� �����.
        };
        // ��������� �������� ���������� ��� ������ ����� ����� ��������� ������ �������� ��������, ���� �������� ����� ���������� ����� 20%, ���� ����� �������� ����������� ����������� �������� �� ������, �� ������� ��� ���������� �����.
        // �� ����� �� ��������� �������������� ��� ������� �� ����������� ������, � ������� ������ ��������, ��� ������� �� ����������� ������ � ����� ������� ���������
        // ����� ���� ����� ������������, �����, �������, ��� ���������� ����� ������ ����� ��������� �����, ����������� ������ ��� ������� ��������� ��������.
    }

    //����� ������������ ���� ������� ����� ����������.
    //https://community.gamedev.tv/t/more-aggressive-enemy/220615?_gl=1*ueppqc*_ga*NzQ2MDMzMjI4LjE2NzY3MTQ0MDc.*_ga_2C81L26GR9*MTY3OTE1NDA5Ni4zMS4xLjE2NzkxNTQ1MjYuMC4wLjA.



}
