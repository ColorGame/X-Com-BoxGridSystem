using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour // ������������ �������� �����
{

    private enum State // ��������� �����
    {
        WaitingForEnemyTurn,// �������� ���������� ����(���� ������)
        TakingTurn,         // ��������� ��� (������� ���)
        Busy,               // �������
    }

    private State _state; // ������� ���������

    private void Awake()
    {
        _state = State.WaitingForEnemyTurn; // ��������� ��������� "�������� ���������� ����" �� ���������, �.�. ����� ����� ������ ������
    }

    private float _timer;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // ���������� �� ������� ��� �������
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) // ��������� ��� ��� ����� ���� ����� ����� �� ���������� ���������� (������ �������� �� �����)
        {
            return;
        }

        switch (_state) // ������� ��������
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0) // ������ ���� ����� �� ���������� ��������� �������� ...
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))   //�������� �� ����� ����������� ��������� �������� ���������� ������������� ���������.� �������� �������� ������� ������� ������ ��� ����� � �������� ��������� TakingTurn,
                                                                    // � ���� ����� �� ��������� _timer ���������� �������� ���� �� ��������� ��� ��������� �������� ��
                    {
                        _state = State.Busy; // ���������� �� ��������� "�������" ���� ���� ��� ��������
                    }
                    else
                    {
                        // � ������ ��� �������� ������� ����� ���������,  ��������� ��� ��������
                        TurnSystem.Instance.NextTurn(); // �������� � ���������� ����
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn() // ���������� ��������� ��������� ��� (������� ���)
    {
        _timer = 0.5f; // ����� �� ���� ���������
        _state = State.TakingTurn; // �������� ��������� ��������� ��� (������� ���)
    }

    private void TurnSystem_OnTurnChanged(object seder, EventArgs emptu) // �� ����� ����� ���� ��������� ������
    {
        if (!TurnSystem.Instance.IsPlayerTurn()) // �������� ��� ��� �� ��� ������
        {
            _state = State.TakingTurn; // �������� ��������� ��������� ��� (������� ���)
            _timer = 2;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)   // ����������� ��������� �������� ���������� ������������� ���������. � �������� �������� ������� onEnemyAIActionComplete (�������� ���������� ������������� ��������� ���������)
                                                                        // ������� �� ������ ������ � �������� ��������� ��������
    {        
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) // � ����� ��������� ������ � ������ ������
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))  // �������� ����� �� �� ����������� �������� �������� ��
            {
                return true;
            }
        }
        return false; // ���� ������ �� ������ �� ������ ����
    }

    //������� ����� �� � ������ ����������
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) // ��������� �������� ���������� ������������� ��������� ��� enemyUnit. � �������� �������� ������� onEnemyAIActionComplete (�������� ���������� ������������� ��������� ���������) � ���������� �����
    {
        EnemyAIAction bestEnemyAIAction = null; // ������� �������� ���������� �� �� ��������� = null 
        BaseAction bestBaseAction = null; // ������ ������� �������� �� ��������� = null 

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionsArray()) // ��������� ������ ������� �������� � ������ ������
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) // ������� � enemyUnit �� ����� ��������� ���� ��������, ����� ��������� �������� ? ���� �� ����� ��...
            {
                // ���� �� ����� ��������� ���� ��������� ��� ��������
                continue; // continue ���������� ��������� ���������� � ��������� �������� ����� 'for' ��������� ��� ����
            }

            if (bestEnemyAIAction == null) // � ������ ����� ������ �������� ���������� �� �� �����������, ������� ��������� 1-� �������� � ������ ������ �� ���������
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction(); // ������� ������ �������� ���������� �� ��� ������� �������� ��������
                bestBaseAction = baseAction; //������(������������) ������� �������� �������� ������
            }
            else // ���� ��� ����������� "������ �������� ���������� ��" �� ������� � ������� � ������� ����� �����
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction(); // ����������� ������ �������� ���������� ��
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) // ���� ����������� �������� �� ������� � �� �������� �������� ������ ����������� �������� �� ��������� ��� ������
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }
        //��������� ���������� ����� ��������
        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction)) // ��������� ��������� ���� ��������, ����� ��������� �������� 
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete); //� ���������� �������� ������� ����� "��������� �������� (�����������)" � ��������� � ������� ������� SetStateTakingTurn  ������� ������ ��� ����� � �������� ��������� TakingTurn
            return true; // ���� ����� ��� �� ������� �� ������ ������
        }
        else
        {
            return false; // ���������� ���������� � ������� ���� 
        }        
    }
}
