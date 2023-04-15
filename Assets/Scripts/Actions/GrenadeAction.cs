using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction // ������� ��������. ��������� BaseAction
{
    [SerializeField] private Transform _grenadeProjectilePrefab; // ������ ������ ������� // � ������� ����� �������� ������ �������

    [SerializeField] private LayerMask _obstaclesAndDoorLayerMask; //����� ���� ����������� � ����� (�������� � ����������) ���� ������� Obstacles � DoorInteract // ����� �� ���� ������ � ���� ���������� ����� ����� -Obstacles, � �� ������ -DoorInteract


    private int _maxThrowDistance = 7; //������������ ��������� ������ //����� ���������//
    private GrenadeProjectile _grenadeProjectile;

    protected override void Awake()
    {
        base.Awake();

        _grenadeProjectile = _grenadeProjectilePrefab.GetComponent<GrenadeProjectile>();
    }

    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        //���� �������� ��� ������ ����� �� �� ������ ������ ��������� ������� �� ���������� ���� ������ ������� ������� �� ���� (��� � �������� ���� �� ��������� ���� ��������)
        //������� ������ ������� ����� �������� ����� ������� �� ����� ������� ����� ��� ���������
        //ActionComplete(); // 
    }


    public override string GetActionName() // ��������� ������� �������� //������� ������������� ������� �������
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0, //�������� ������ �������� ��������. ����� ������� ������� ���� ������ ������� ������� �� �����, 
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()// �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������                                                                       
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxThrowDistance; x <= _maxThrowDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxSwordDistance
        {
            for (int z = -_maxThrowDistance; z <= _maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                // ��� ������� ������� ������� ������� ���� � �� �������
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z); // ����� ���� ������������� ��������� �������� �������
                if (testDistance > _maxThrowDistance) //������� ������ �� ����� � ���� ����� // ���� ���� � (0,0) �� ������ � ������������ (5,4) ��� �� ������� �������� 5+4>7
                {
                    continue;
                }

                /*if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) //�������� �������� ������� ���� ������ ������ ��� �� ��� ���� ������ � ����� (Obstacles -�����������)  (������� ����� ������ � ����������� ��������)
                {
                    continue;
                }*/

                int pathfindingDistanceMultiplier = 10; // ��������� ���������� ����������� ���� (� ������ Pathfinding ������ ��������� �������� �� ������ � ��� ����� ����� 10 �� ��������� 14, ������� ������� ��� ��������� �� ���������� ������)
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > _maxThrowDistance * pathfindingDistanceMultiplier) //�������� �������� ������� - ���� ��������� �� ����������� ������ ������ ���������� ������� ���� ����� ���������� �� ���� ���
                {
                    // ����� ���� ������� ������
                    continue;
                }

                // �������� �� ����������� ������ ����� �����������
                Vector3 worldTestGridPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);   // ������� ������� ���������� ����������� �������� ������� 
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition); // ��������� � ������� ���������� ���������� ��� �������� ������� �����  
                Vector3 grenadeDirection = (worldTestGridPosition - unitWorldPosition).normalized; //��������������� ������ ����������� ������ �������

                float unitShoulderHeight = 1.7f; // ������ ����� �����, � ���������� ����� ������������� ���������� � ������������ �������
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        grenadeDirection,
                        Vector3.Distance(unitWorldPosition, worldTestGridPosition),
                        _obstaclesAndDoorLayerMask)) // ���� ��� ����� � ����������� �� (Raycast -������ bool ����������)
                {
                    // �� �������������� ������������
                    continue;
                }

                validGridPositionList.Add(testGridPosition); // ��������� � ������ �� ������� ������� ������ ��� �����
                //Debug.Log(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)  // ������������� TakeAction (��������� �������� (�����������). (������� onActionComplete - �� ���������� ��������). � ����� ������ �������� �������� ������� ClearBusy - �������� ���������
    {
        Transform grenadeProjectileTransform = Instantiate(_grenadeProjectilePrefab, _unit.GetWorldPosition(), Quaternion.identity); // �������� ������ ������� � ������� ������ 
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>(); // ������� � ������� ��������� GrenadeProjectile
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviorComplete); // � ������� ������� Setup() ������� � ��� ������� ������� (��������� ������� ������� ����) � ��������� � ������� ������� OnGrenadeBehaviorComplete ( ��� ������ ������� ����� �������� ��� �������)

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������
    }

    private void OnGrenadeBehaviorComplete() // ������������� ����� ������� ���������� ActionComplete() . ���� ����� ������������ ActionComplete() �������� �� ����� ���������� � ���������
    {
        ActionComplete(); // ��� ������� ��������� - �������� ��������� ��� ����� ��������� - ������������ ������ UI
    }

    public int GetMaxThrowDistance()//�������� _maxSwordDistance
    {
        return _maxThrowDistance;
    }

    public int GetDamageRadiusInCells() => _grenadeProjectile.GetDamageRadiusInCells(); // �������� �������
}

// https://community.gamedev.tv/t/grenade-can-be-thrown-through-wall/205331 �������� ������� ����� �����