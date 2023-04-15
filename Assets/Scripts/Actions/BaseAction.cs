using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour    //������� �������� ���� ���� ����� ����������� ������ ������ 
                                                    // �� ����� ��������� ����������� �������� ������� ��������� ���� �����. ���-�� �������� �� ������� ��������� BaseAction ������� ���
                                                    // abstract - �� ��������� ������� Instance (���������) ����� ������.
{
    public static event EventHandler OnAnyActionStart; // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������. ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 
                                                       // �� �������� ������� Event ���  ������� ������ ��������.
    public static event EventHandler OnAnyActionCompleted; // �� �������� ������� Event ���  ���������� ������ ��������.

    protected Unit _unit; // ���� �� ������� ����� ��������� ��������
    protected bool _isActive; // ������� ����������. ��� �� ��������� ����������� ���������� ���������� ��������
                              // (� ����� ������ �� �������� ��������� ��� �������� Spin() �  transform.forward)

    //���� ������������ ���������� ������� Action ������ - //public delegate void ActionCompleteDelegate(); //���������� �������� // ��������� ������� ������� �� ��������� �������� � ���������� �������
    protected Action _onActionComplete; //(�� ���������� ��������)// �������� ������� � ������������ ���� - using System;
                                        //�������� ��� ������� ��� ������������ ���������� (� ��� ����� ��������� ������ ������� �� ���������).
                                        //Action- ���������� �������. ���� ��� �������. ������� Func<>. 
                                        //�������� ��������. ����� ���������� ������� � ������� �� �������� ��������, ����� �����, � ������������ ����� ����, ����������� ����������� �������.
                                        //�������� ��������. ����� �������� �������� ������� �� ������� ������

    protected virtual void Awake() // protected virtual- ���������� ��� ����� �������������� � �������� �������
    {
        _unit = GetComponent<Unit>();
    }

    public abstract string GetActionName(); // ������� ��� �������� // abstract - ��������� ������������� � ������ ��������� � � ������� ������ ����� ������ ����.

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete); //Generic ��������� �������� (�����������) � �������� �������� �������� ������� ��� �������� � ������� onActionComplete (��� ���������� ��������, � ����� ������ ��� ClearBusy() // �������� ��������� ��� ����� ��������� - ������������ ������ UI ) 

    public abstract List<GridPosition> GetValidActionGridPositionList(); //�������� ������ ���������� �������� ������� ��� ��������

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) //(���������) �������� �� ���������� �������� ������� ��� �������� //������� virtual- ���� ������������ �������������� ��� ������
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition); // ���� ���� _gridPosition ����������� � ����� ���������� �������, �� ��������  ������
    }

    public virtual int GetActionPointCost() // �������� ������ ����� �� �������� (��������� ��������) //������� virtual- ���� ������������ �������������� ��� ������ 
    {
        return 1; // �� ��������� ��������� ���� �������� =1
    }

    protected void ActionStart(Action onActionComplete) //����� �������� � �������� �������� ������� onActionComplete (��� ���������� ��������) // � ����� ���� �� ������������ ������� � ������ TakeAction() � �� ���� ��������� �������
    {
        _isActive = true;
        _onActionComplete = onActionComplete; // �������� ���������� �������

        OnAnyActionStart?.Invoke(this, EventArgs.Empty); // �������� �������  // ����� ������ ���� ��������� ����� ���� ��� �������� �������� � ��� ��������� //�������� ����� ����� ActionStart() ���������� ����� ���� ��� ��� ���� �������� ��������� ��� ����� �������� ���� ����� � ����� �������
    }

    protected void ActionComplete() //�������� ���������
    {
        _isActive = false;
        _onActionComplete(); // ������� ���������� �������.(� ����� ������ ��� ClearBusy() // �������� ��������� ��� ����� ��������� - ������������ ������ UI  

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty); // �������� �������
    }

    public Unit GetUnit() // �������� �������� Unit 
    {
        return _unit;
    }

    public EnemyAIAction GetBestEnemyAIAction() // ������� ������ �������� ���������� ��
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>(); // ������ �������� ���������� ��

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList(); //�������� ������ ���������� �������� ������� ��� ��������

        foreach (GridPosition gridPosition in validActionGridPositionList) // ��������� ��� ������ ����� �� ������������ ����������� ������
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition); // ����������� �������� ���������� �� ��� ����� ���������� �������� � ���� ������� �����
            enemyAIActionList.Add(enemyAIAction); // ������� ������������� �������� � ������ (��� ������� �������� ����� ���� ������)
        }

        if (enemyAIActionList.Count > 0) // �������� ��� ������ �� ������
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue); //�����������. ��������� ������� ������� �������� ����� �������� ����������(������� �������� ����� ������) // (Comparison<T> ������� - ������������ �����, ������������ ��� ������� ������ ����. ������ �������� -�������� ����� �����, ������� ���������� ������������� �������� ����������(������ � ������) a � b),  

            // ��� ��������� // ������ ��������� ������ �������� ��� ����� (https://stackoverflow.com/questions/74245814/can-someone-help-me-understand-anonymous-functions-in-c)
            /*int EnemyAIActionComparison(EnemyAIAction a, EnemyAIAction b)
            {
                if (a.actionValue > b.actionValue) return +1;
                if (a.actionValue < b.actionValue) return -1;
                return 0; // ��� �����
            }
            //��������� �� ����� ��������, ��������� ����� ���������(����� �������� ������ ����), �� ����� �� ������ ��������
            int EnemyAIActionComparison(EnemyAIAction x, EnemyAIAction y)
            {
                return x.actionValue - y.actionValue;
            }*/
            // ������ ������� ������ � ���������� ��������� actionValue
            return enemyAIActionList[0]; // ������ ����� ���, ��� ����� ������� ������
        }
        else
        {
            // ��� ��������� �������� �� �����
            return null;
        }

    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition); //�������� �������� ���������� �� � ������ �������� ��� _gridPosition // abstract - ��������� ������������� � ������ ��������� � � ������� ������ ����� ������ ����.
}
