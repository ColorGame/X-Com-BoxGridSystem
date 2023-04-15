using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour // ���� ���� ����� �������� �� ������� �� ����� � ���� ��������, ��������� �����
{

    private const int ACTION_POINTS_MAX = 3; //���� ���������//

    // ��� // ������� 2 //� UnitActionSystemUI
    public static event EventHandler OnAnyActionPointsChanged;  // static - ���������� ��� event ����� ������������ ��� ����� ������ �� �������� �� ���� ������� � ��� �������� ������. ������� ��� ������������� ����� ������� ��������� �� ����� ������ �� �����-���� ���������� �������, ��� ����� �������� ������ � ������� ����� �����, ������� ����� ��������� ���� � �� �� ������� ��� ������ �������. 
                                                                // �� �������� ������� Event ��� ��������� ����� �������� � ������(Any) ����� � �� ������ � ����������.

    public static event EventHandler OnAnyUnitSpawned; // ������� ����� ���������(���������) ����
    public static event EventHandler OnAnyUnitDead; // ������� ����� ������� ����


    [SerializeField] private bool _isEnemy; //� ���������� � ������� ����� ��������� �������

    // ������� ������
    private GridPosition _gridPosition;
    private HealthSystem _healthSystem; 
    private BaseAction[] _baseActionsArray; // ������ ������� �������� // ����� ������������ ��� �������� ������
    private int _actionPoints = ACTION_POINTS_MAX; // ���� ��������

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>(); 
        _baseActionsArray = GetComponents<BaseAction>(); // _moveAction � _spinAction ����� ����� ��������� ������ ����� �������
    }

    private void Start()
    {
        // ����� Unit ����������� �� ��������� ���� ��������� � ����� � ��������� ���� � GridObject(�������� �����) � ������ ������
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //������� ������� ����� �� �����. ��� ����� ����������� ������� ������� ����� � ������� �� �����
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this); // ������ � LevelGrid ������� ������ � ������������ ���������� � ������� AddUnitAtGridPosition

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged; // ������������� �� Event // ����� ����������� (����� ����� ��������) ������ ��� ����� ������� ����� ����.

        _healthSystem.OnDead += HealthSystem_OnDead; // ������������� �� Event. ����� ����������� ��� ������ �����

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ���������(���������) ����. ������� ��������� ������� ����� ����������� ��� ���� ��������� ������
    }
       

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position); //������� ����� ������� ����� �� �����.
        if (newGridPosition != _gridPosition) // ���� ����� ������� �� ����� ���������� �� ��������� �� ...
        {
            // ������� ��������� ����� �� �����
            GridPosition oldGridPosition = _gridPosition; // �������� ������ ������� ��� �� �������� � event
            _gridPosition = newGridPosition; //������� ������� - ����� ������� ����������� �������
           
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition); //� UnitMovedGridPosition ��������� �������. ������� ��� ������ �������� � ������ . ����� �� ��������� ������� ����� ����������� � ���� ��� �� ���������
        }
    }

    public T GetAction<T>() where T : BaseAction //�������� ��� ��������� ������ ���� �������� �������� // �������� ����� � GENERICS � ��������� ������ �  BaseAction
    {
        foreach (BaseAction baseAction in _baseActionsArray) // ��������� ������ ������� ��������
        {
            if (baseAction is T) // ���� T ��������� � ����� ������ baseAction �� ...
            {
                return baseAction as T; // ������ ��� ������� �������� ��� � // (T)baseAction; - ��� ���� ����� ������
            }
        }
        return null; // ���� ��� ���������� �� ������ ����
    }
        
    public GridPosition GetGridPosition() // �������� �������� �������
    {
        return _gridPosition;
    }


    public Vector3 GetWorldPosition() // �������� ������� �������
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionsArray() // �������� ������ ������� ��������
    {
        return _baseActionsArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) // ��������� ��������� ���� ��������, ����� ��������� �������� // ���� ����� ��������� ������ ��� ������ ������
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }


    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) //�� ����� ��������� ���� ��������, ����� ��������� �������� ? 
    {
        if (_actionPoints >= baseAction.GetActionPointCost()) // ���� ����� �������� ������� ��...
        {
            return true; // ����� ��������� ��������
        }
        else
        {
            return false; // ��� ����� �� �������
        }

        /*// �������������� ������ ���� ����
        return _actionPoints >= baseAction.GetActionPointCost();*/
    }

    private void SpendActionPoints(int amount) //��������� ���� �������� (amount- ���������� ������� ���� ���������)
    {
        _actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� UnitActionSystemUI)
    }

    public int GetActionPoints() // �������� ���� ��������
    {
        return _actionPoints;
    }


    public void TurnSystem_OnTurnChanged(object sender, EventArgs empty) // ������� ���� �������� �� ������������
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || // ���� ��� ���� � ��� ������� (�� ������� ������) ��� ��� �� ����(�����) � ������� ������ ��...
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            _actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty); // ��������� ������� ����� ���������� ����� ��������.(��� // ������� // 2 //� UnitActionSystemUI)
        }
    }

    public bool IsEnemy() // �������� ����
    {
        return _isEnemy;
    }

    public void Damage(int damageAmount) // ����. ������ ������ ����� �������� ������ ���� ������� � ��������� ��������� damageAmount
    {
        _healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) // ����� ����������� ��� ������ �����

    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this); // ������ �� �������� ������� �������� �����

        Destroy(gameObject); // ��������� ������� ������ � �������� ���������� ������ ������

        // ������� ������ ��������� ����� ���� ������� ��� ���������� �����        

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty); // �������� ������� ����� ������� ����. ������� ��������� ������� ����� ����������� ��� ������ �������� �����      
    }

    public float GetHealthNormalized() // �������� ��� ������
    {
        return _healthSystem.GetHealthNormalized();
    }

    public int GetHealth() // �������� ��� ������
    {
        return _healthSystem.GetHealth();
    }
    
    public int GetHealthMax() // �������� ��� ������
    {
        return _healthSystem.GetHealthMax();
    }

    public bool IsDead()
    {
        return _healthSystem.IsDead();
    }
}
