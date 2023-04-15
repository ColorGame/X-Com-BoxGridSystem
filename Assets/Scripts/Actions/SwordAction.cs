using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootAction;

public class SwordAction : BaseAction // ������� �������� ���
{
    public static event EventHandler<OnSwordEventArgs> OnAnySwordHit;   // ������� - ����� ����� ���� ����� (����� ����� ���� ������ ��������� ����� �� �������� ������� Event) // <Unit> ������� �������� �������� ����� ��� ����
                                                                        // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������.
                                                                        // ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����,
                                                                        // ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 

    public event EventHandler OnSwordActionStarted;     // �������� ��� ��������
    public event EventHandler OnSwordActionCompleted;   // �������� ��� �����������
    public class OnSwordEventArgs : EventArgs // �������� ����� �������, ����� � ��������� ������� �������� ������ ������
    {
        public Unit targetUnit; // ������� ���� � ���� �������
        public Unit hittingUnit; // ���� ������� �������� ���� �����
    }
    private enum State
    {
        SwingingSwordBeforeHit, //����� ����� ����� ������
        SwingingSwordAfterHit,  //����� ���� ����� �����
    }


    private State _state; // ��������� �����
    private float _stateTimer; //������ ���������
    private Unit _targetUnit;// ���� � �������� �������� �������


    private int _maxSwordDistance = 1; //������������ ��������� ��������� ����� //����� ���������//



    private void Update()
    {
        if (!_isActive) // ���� �� ������� �� ...
        {
            return; // ������� � ���������� ��� ����
        }

        _stateTimer -= Time.deltaTime; // �������� ������ ��� ������������ ���������

        switch (_state) // ������������� ���������� ���� � ����������� �� _state
        {
            case State.SwingingSwordBeforeHit:
                
                Vector3 aimDirection = (_targetUnit.GetWorldPosition() - transform.position).normalized; // ����������� ������������, ��������� ������
                float rotateSpeed = 10f; //����� ���������//
                
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed); // ������ �����.

                break;

            case State.SwingingSwordAfterHit:
                break;
        }

        if (_stateTimer <= 0) // �� ��������� ������� ������� NextState() ������� � ���� ������� ���������� ���������. �������� - � ���� ���� State.Aiming: ����� � case State.Aiming: ��������� �� State.Shooting;
        {
            NextState(); //��������� ���������
        }
    }

    private void NextState() //������� ������������ ���������
    {
        switch (_state)
        {
            case State.SwingingSwordBeforeHit:
                _state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f; // ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� ���� ����� ����� //����� ���������//
                _stateTimer = afterHitStateTime;
                SwordHit();               
                break;

            case State.SwingingSwordAfterHit:

                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);  // �������� ������� �������� ��� �����������

                ActionComplete(); // ������� ������� ������� �������� ���������
                break;
        }

        //Debug.Log(_state);
    }

    private void SwordHit() // ���� �����
    {        
        OnAnySwordHit?.Invoke(this, new OnSwordEventArgs // ������� ����� ��������� ������ OnShootEventArgs
        {
            targetUnit = _targetUnit,
            hittingUnit = _unit
        }); // �������� ������� ����� ����� ���� ����� � � �������� ��������� � ���� ������� � ��� �������� ���� (���������� ScreenShakeActions ��� ���������� ������ ������ � UnitRagdollSpawner- ��� ����������� ����������� ���������)
   
        _targetUnit.Damage(100); // ������� �������� ����� ������� ����     //����� ���������// � ���������� ����� ����� ���� ���������� �� ������
    }


    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition) //�������� �������� ���������� �� // ������������� ����������� ������� �����
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200, // �������� ��� �������� ������������ //����� ���������//
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()// �������� ������ ���������� �������� ������� ��� �������� // ������������� ������� �������                                                                       
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = _unit.GetGridPosition(); // ������� ������� � ����� �����

        for (int x = -_maxSwordDistance; x <= _maxSwordDistance; x++) // ���� ��� ����� ����� ������� � ������������ unitGridPosition, ������� ��������� ���������� �������� � �������� ������� _maxSwordDistance
        {
            for (int z = -_maxSwordDistance; z <= _maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z); // ��������� �������� �������. ��� ������� ���������(0,0) �������� ��� ���� 
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition; // ����������� �������� �������

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) // �������� �������� �� testGridPosition ���������� �������� �������� ���� ��� �� ��������� � ���� �����
                {
                    continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) // �������� �������� ������� ��� ��� ������ (��� ����� ������ � ������� �� ����� �� ��� �������)
                {
                    // ������� ����� �����, ��� ������
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);   // ������� ����� �� ����� ����������� �������� ������� 
                                                                                                // GetUnitAtGridPosition ����� ������� null �� � ���� ���� �� ��������� ������� �������, ��� ��� �������� �� �����
                if (targetUnit.IsEnemy() == _unit.IsEnemy()) // ���� ����������� ���� ���� � ��� ���� ���� ���� �� (���� ��� ��� � ����� ������� �� ����� ������������ ���� ������)
                {
                    // ��� ������������� � ����� "�������"
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
        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition); // ������� ����� � �������� ������� � �������� ���

        _state = State.SwingingSwordBeforeHit; // ���������� ��������� ������������  ����� ����� ����� ������
        float beforeHitStateTime = 0.7f; //�� �����.  ��� ��������� ���������� ������ ������ ����������  ����������������� ��������� ����� ����� ����� ������ ..//����� ���������//
        _stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty); // �������� ������� �������� ��� �������� ��������� UnitAnimator

        ActionStart(onActionComplete); // ������� ������� ������� ����� �������� // �������� ���� ����� � ����� ����� ���� �������� �.�. � ���� ������ ���� EVENT � �� ������ ����������� ����� ���� ��������

    }
    public int GetMaxSwordDistance()
    {
        return _maxSwordDistance;
    }

    public Unit GetTargetUnit() // �������� _targetUnit
    {
        return _targetUnit;
    }

}
